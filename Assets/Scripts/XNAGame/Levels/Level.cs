using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GLEED2D;
using FarseerPhysics.Dynamics;
using PushBlock.Helpers;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics;
using PushBlock.Blocks;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common.PolygonManipulation;
using GameStateManagement;
using System.Diagnostics;
using Particles2DPipelineSample;

namespace PushBlock
{
    class Level : IDisposable
    {
        public const int LevelsCount = 30;
        
        public int LevelNumber { get; private set; }
        
        public float AwesomeAwardTime { get; private set; }

        public World World { get { return world; } }
        
        World world;

        public Block[] Blocks { get { return blocks.ToArray(); } }

        List<Block> blocks = new List<Block>();

        public MrOrange MrOrange { get; private set; }

        public TimeSpan ElapsedTime { get; private set; }

        // Particles
        ParticleSystem explosionParticles;

        TimeSpan timeBetweenPlayQuietDropSound = new TimeSpan(0, 0, 0, 0, 100);
        TimeSpan timeTillPlayQuietDropSound;
              

        #region Initialization

        public Level(Stream levelXmlStream,   Game game)
        {
            // create physics world
            world = new World(new Microsoft.Xna.Framework.Vector2(0, 3f));
            
            CreateLevelWorld(levelXmlStream, game.Content);

            // subscribing to event for adding dynamic bouncing of blocks
            World.ContactManager.BeginContact += new BeginContactDelegate(OnBeginContact);

            // Create particle system
            explosionParticles = new ParticleSystem(Helper.Game, "Particles/BlockExplosion");
            explosionParticles.Initialize();
            explosionParticles.DrawOrder = 3;
            game.Components.Add(explosionParticles);
        }

        public static Level LoadLevel(int levelNum, Game game)
        {
            UnityEngine.TextAsset textAsset = (UnityEngine.TextAsset)UnityEngine.Resources.Load(
                string.Format("Content/Levels/{0}", levelNum), 
                typeof(UnityEngine.TextAsset));  

            using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            {
                return new Level(stream, game);
            }
        }

        #endregion

        #region Create Level World

        public void CreateLevelWorld(Stream data, ContentManager content)
        {
            // Deserialize level from xml
            XmlSerializer xs = new XmlSerializer(typeof(GLEED2D.Level));
            GLEED2D.Level gleedLevel = (GLEED2D.Level)xs.Deserialize(data);
           
            
            try
            {
                AwesomeAwardTime = float.Parse((string)gleedLevel.CustomProperties["AwesomeAwardTime"].value);
            }
            catch
            {
                AwesomeAwardTime = 0;    
            }
            
            // set level num
            LevelNumber = int.Parse(gleedLevel.Name);

            foreach (Layer layer in gleedLevel.Layers)
            {
                foreach (TextureItem item in layer.Items.OfType<TextureItem>())
                {
                    CreateLevelObject(item, content);
                }
            }
        }

        Body CreateBodyFromTexture(Texture2D polygonTexture, out Vector2 origin)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[(polygonTexture.Width) * (polygonTexture.Height)];

            //Transfer the texture data to the array
            var colors = polygonTexture.UnityTexture.GetPixels32();
            
            for (int i=0; i< data.Length; i++)
            {
                data[i] = Helper.ColorToUInt(colors[i]);
            }

            data = Helper.ReverseUnityTextureDataToUVOrder(data, polygonTexture.Width);

            //Find the vertices that makes up the outline of the shape in the texture
            Vertices textureVertices = PolygonTools.CreatePolygon(data, polygonTexture.Width, false);

            //The tool return vertices as they were found in the texture.
            //We need to find the real center (centroid) of the vertices for 2 reasons:

            ////1. To translate the vertices so the polygon is centered around the centroid.
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            //2. To draw the texture the correct place.
            origin = -centroid;

            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            List<Vertices> list = BayazitDecomposer.ConvexPartition(textureVertices);

            //scale the vertices from graphics space to sim space
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits((int)1));
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
            }

            //Create a single body with multiple fixtures
            Body compound = BodyFactory.CreateCompoundPolygon(World, list, 1f, polygonTexture);
            return compound;
        }

        void CreateLevelObject(TextureItem textureObj, ContentManager content)
        {
            // Get texture object
            string textureFileName = textureObj.texture_filename;
#if UNITY_ANDROID
            textureFileName = textureObj.texture_filename.Replace("\\", "/");
#endif
            string textureName = System.IO.Path.GetFileNameWithoutExtension(textureFileName).ToLower();
            Texture2D texture = content.Load<Texture2D>(string.Format("Blocks/{0}", textureName));

            // Get texture position and rotation, 480 its level's height
            Vector2 position = textureObj.Position + new Vector2(0, 480);// +new Vector2(texture.Width / 2, texture.Height / 2);
            float density = 1f;

            Body body;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
          
            if (textureName.Contains("ball"))
            {
                body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits((texture.Width - 2) / 2),
                                                  density, ConvertUnits.ToSimUnits(position), texture);
                
            }
            else if (textureName.Contains("triangle"))
            {
                body = CreateBodyFromTexture(texture, out origin);
                body.Position = ConvertUnits.ToSimUnits(position);//textureObj.Position + new Vector2(0, 480) + origin);
            }
            else
            {
                // majority of blocks are rectangles and dynamic
                body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(texture.Width - 2),
                                                            ConvertUnits.ToSimUnits(texture.Height - 2),
                                                            density, ConvertUnits.ToSimUnits(position), texture);
            }

            body.Friction = 0.2f;
            body.Restitution = 0.6f;
            body.Rotation = textureObj.Rotation;
            body.BodyType = BodyType.Dynamic;

            BlockType blockType = BlockType.Breakable;

            if (textureName.Contains("mrorange"))
            {
                body.Friction = 0.3f;
                body.Restitution = 0.35f;
                blockType = BlockType.MrOrange;
            }
            else if (textureName.Contains("orange"))
            {
                body.BodyType = BodyType.Static;
                body.Friction = 0.3f;
                body.Restitution = 0.35f;
                blockType = BlockType.Orange;
            }
            else if (textureName.Contains("ground"))
            {
                body.BodyType = BodyType.Static;
                body.Friction = 0.7f;
                blockType = BlockType.Ground;
            }
            else if (textureName.Contains("grey")) blockType = BlockType.Unbreakable;

            Block block;
            if (textureName.Contains("mrorange")) block = MrOrange = new MrOrange(body);
            else block = new Block(texture, body, blockType);
            
            block.origin = origin;
            body.UserData = block;
            blocks.Add(block);
        }
             
        #endregion

        bool OnBeginContact(Contact contact)
        {
            float maxVelocity = Math.Max(contact.FixtureA.Body.LinearVelocity.Length(), contact.FixtureB.Body.LinearVelocity.Length());

            Debug.WriteLine(maxVelocity);
            if (maxVelocity > 2.5f)
            {
                AudioManager.PlaySound("drop");
            }
            else if (maxVelocity > 1.5f) 
            {
                // Play medium volume drop sound
                AudioManager.PlaySound("drop", AudioManager.SoundVolume * 0.66f);
            }
            else if (maxVelocity > 0.5f)// && timeTillPlayQuietDropSound == TimeSpan.Zero) 
            {
                // Play quiet drop sound
                timeTillPlayQuietDropSound = timeBetweenPlayQuietDropSound;
                AudioManager.PlaySound("drop", AudioManager.SoundVolume * 0.33f);
            }

            return true;
        }

        public void HandleInput(  GameTime gameTime, InputState input)
        {
            // Handle user clicks on blocks
            if (input.IsNewMouseLeftButtonDown())
            {
                Vector2 worldPoint = ConvertUnits.ToSimUnits(input.CurrentMouseState.X, input.CurrentMouseState.Y);

                Fixture fixture = World.TestPoint(worldPoint);

                if (fixture != null)
                {
                    Body body = fixture.Body;
                    Block block = (Block)body.UserData;

                    if (block.Type == BlockType.Breakable)
                    {
                        // Remove block body from world
                        World.RemoveBody(body);

                        // Remove from level
                        blocks.Remove(block);

                        // Wake up bodies which are colliding with exploded block
                        ContactEdge contactEdge = body.ContactList;

                        while (contactEdge != null)
                        {
                            Contact contact = contactEdge.Contact;

                            contact.FixtureA.Body.Awake = true;
                            contact.FixtureB.Body.Awake = true;

                            contactEdge = contactEdge.Next;
                        }

                        // Create particle explosion 
                        string textureName = block.BlockName.ToLower();
                        Color particleColor1 = Color.White;
                        Color particleColor2 = Color.White;

                        if (textureName.Contains("green"))
                        {
                            particleColor1 = new Color(1, 161, 3);
                            particleColor2 = new Color(0, 210, 2);
                        }
                        else if (textureName.Contains("red"))
                        {
                            particleColor1 = new Color(189, 38, 37);
                            particleColor2 = new Color(249, 60, 59);
                        }
                        else if (textureName.Contains("blue"))
                        {
                            particleColor1 = new Color(7, 116, 176);
                            particleColor2 = new Color(37, 178, 255);
                        }
                        else if (textureName.Contains("ball"))
                        {
                            particleColor1 = new Color(197, 158, 0);
                            particleColor2 = new Color(236, 189, 0);
                        }
                        
                        explosionParticles.AddParticles(ConvertUnits.ToDisplayUnits(body.Position), Vector2.Zero, particleColor1);
                        explosionParticles.AddParticles(ConvertUnits.ToDisplayUnits(body.Position), Vector2.Zero, particleColor2);

                        // Play explosion sound
                        AudioManager.PlaySound("flop");
                    }

                }
            }
        }


        public void Update(  GameTime gameTime)
        {
            // Update physics
            World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Update all blocks behaviour included hero
            foreach (Block block in blocks) block.Update(gameTime);

            // remove blocks which are outside of the screen
            foreach (Body body in world.BodyList.Where(item => item.Position.Y > ConvertUnits.ToSimUnits(480)))
            {
                blocks.Remove((Block)body.UserData);
                World.RemoveBody(body);
            }

            ElapsedTime += gameTime.ElapsedGameTime;

            // Update sound drop time
            if (timeTillPlayQuietDropSound > TimeSpan.Zero)
                timeTillPlayQuietDropSound -= gameTime.ElapsedGameTime;
            else timeTillPlayQuietDropSound = TimeSpan.Zero;
        }


        public void Draw(  GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Block block in blocks) block.Draw(gameTime, spriteBatch);
        }

        #region IDisposable Members

        public void Dispose()
        {
            explosionParticles.Game.Components.Remove(explosionParticles);
        }

        #endregion
    }
}
