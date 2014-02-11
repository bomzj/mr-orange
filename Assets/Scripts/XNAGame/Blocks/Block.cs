using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using PushBlock.FarseerPhysics;

namespace PushBlock.Blocks
{
    enum BlockType { MrOrange, Ground, Orange, Breakable, Unbreakable }

    class Block
    {
        protected static Random random = new Random(DateTime.Now.Millisecond);

        protected Body blockBody;
        protected Texture2D blockTexture;
        
        // This is physical body centroid
        public  Vector2 origin;

        public BlockType Type { get; private set; }

        public string BlockName { get { return blockTexture.UnityTexture.name ; } }

        public Block(Texture2D blockTexture, Body blockBody, BlockType blockType)
        {
            blockBody.OnCollision += new OnCollisionEventHandler(blockBody_OnCollision);
            this.blockBody = blockBody;
            this.blockTexture = blockTexture;

            Type = blockType;
        }
        
        bool blockBody_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Vector2 point = blockBody.Position;
            point += random.Next(2) == 0 ? new Vector2(.5f, .5f) : new Vector2(-.5f, -.5f);
            blockBody.ApplyLinearImpulse(-blockBody.LinearVelocity / 100, point);

            return true;  
        }

        public void Explode()
        {
            
            //string blockName = blockTexture.Name;
            //if (blockName.Contains("green")) particleSystem = greenParticles;
            //else if (blockName.Contains("red")) particleSystem = redParticles;
            //else if (blockName.Contains("blue")) particleSystem = blueParticles;
            //else if (blockName.Contains("ball")) particleSystem = purpleParticles;
        }

        public virtual void Update( GameTime gameTime)
        {
            // if hero outside the screen then hero 'touches' ground
            //if (blockBody.Position.Y > ConvertUnits.ToSimUnits(480)) blockBody.Dispose();
            
          

        }

        public virtual void Draw( GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 position = ConvertUnits.ToDisplayUnits(blockBody.Position);
            spriteBatch.Draw(blockTexture, position, null, Color.White, blockBody.Rotation,
                            origin, 
                            //new Vector2(blockTexture.Width / 2, blockTexture.Height / 2), 
                            1, SpriteEffects.None, 0);
        }
    }
}
