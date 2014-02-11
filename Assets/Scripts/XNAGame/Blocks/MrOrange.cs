using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using PushBlock.Screens;
using FarseerPhysics.Factories;
using PushBlock.FarseerPhysics;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using PushBlock.Blocks;
using PushBlock.Helpers;

namespace PushBlock
{
    class MrOrange : Block
    {
        Texture2D smileTexture;
        Texture2D closedEyesTexture;
        Texture2D droppedTexture;
        Texture2D winTexture;
        Texture2D loseTexture;

        Body mrOrangeBody;

        public bool IsOnGround { get; private set; }
        public bool IsOnOrange { get; private set; }

        TimeSpan timeTillCloseEyes;
        TimeSpan timeBetweenEyesBlinking = new TimeSpan(0,0,0,0,300);

        void OnGround()
        {
            this.IsOnGround = true;
            AudioManager.PlaySound("ooh");
        }

        void OnOrange()
        {
            IsOnOrange = true;
            AudioManager.PlaySound("yeah");
        }

        public MrOrange(Body mrOrangeBody)
            : base(Helper.Content.Load<Texture2D>("Blocks/MrOrange"), mrOrangeBody, BlockType.MrOrange)
        {
            this.mrOrangeBody = mrOrangeBody;
            
            smileTexture = Helper.Content.Load<Texture2D>("Hero/smile");
            var smileTexture1 = Helper.Content.Load<Texture2D>("Hero/smile");
            closedEyesTexture = Helper.Content.Load<Texture2D>("Hero/closedEyes");
            droppedTexture = Helper.Content.Load<Texture2D>("Hero/dropped");
            winTexture = Helper.Content.Load<Texture2D>("Hero/win");
            loseTexture = Helper.Content.Load<Texture2D>("Hero/lose");
        }

        public override void Update(  GameTime gameTime)
        {
            base.Update(gameTime);

            // if hero outside the screen then hero 'touches' ground
            if (mrOrangeBody.Position.Y > ConvertUnits.ToSimUnits(480) && !IsOnGround)
            {
                OnGround();
            }
            
            // Process hero contacts with ground and orange blocks
            ContactEdge contactEdge = mrOrangeBody.ContactList;

            bool isOnOrangeOnly = false;

            while (contactEdge != null)
            {
                Contact contact = contactEdge.Contact;
				
                if (contact.IsTouching)
                {
                    BlockType blockAType = ((Block)contact.FixtureA.Body.UserData).Type;
                    BlockType blockBType = ((Block)contact.FixtureB.Body.UserData).Type;

                    if (mrOrangeBody.LinearVelocity == Vector2.Zero)
                    {
                        if ((blockAType == BlockType.Ground || blockBType == BlockType.Ground)
                            && !IsOnGround) 
                        {
                            isOnOrangeOnly = false;
                            OnGround();
                        }
                        else if ((blockAType == BlockType.Orange || blockBType == BlockType.Orange)
                                && !IsOnOrange && !IsOnGround)
                        {
                            isOnOrangeOnly = true;
                            //OnOrange();
                        }
                    }
                }

                contactEdge = contactEdge.Next;
            }

            if (isOnOrangeOnly) OnOrange();

            if (timeTillCloseEyes + timeBetweenEyesBlinking < TimeSpan.Zero) timeTillCloseEyes = new TimeSpan(0, 0, random.Next(1, 4));
            
            timeTillCloseEyes -= gameTime.ElapsedGameTime;

        }

        public override void Draw(  GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            
            Vector2 positionOrigin = ConvertUnits.ToDisplayUnits(mrOrangeBody.Position);

            Texture2D texture = null;

            if (IsOnGround)
            {
                texture = loseTexture;
            }
            else if (IsOnOrange)
            {
                texture = winTexture;
            }
            else if (mrOrangeBody.LinearVelocity != Vector2.Zero)
            {
                texture = droppedTexture;
            }
            else if (timeTillCloseEyes + timeBetweenEyesBlinking < timeBetweenEyesBlinking)
            {
                texture = closedEyesTexture;
            }
            else texture = smileTexture;

            spriteBatch.Draw(texture, positionOrigin, null, Color.White, 0,
                            new Vector2(smileTexture.Width / 2, smileTexture.Height / 2), 1, SpriteEffects.None, 0);
            
        }
    }
}
