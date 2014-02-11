using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PushBlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Scripts.XNAGame.Social
{
    class SocialButtons
    {
        ScreenManager ScreenManager;

        Button facebookButton;
        Button twitterButton;
        Button thumbUpButton;

        public SocialButtons(ScreenManager screenManager)
        {
            this.ScreenManager = screenManager;
        }

        public void Initialize()
        {
            // Thumb Up
            Texture2D thumbUpTexture = ScreenManager.Game.Content.Load<Texture2D>("SocialLinks/ThumbUp");

            thumbUpButton = new Button()
            {
                Image = thumbUpTexture,
                Position = new Vector2(800 / 2 + 200,
                    480 / 2)
            };

            thumbUpButton.Clicked += thumbUpButton_Clicked;

            // Facebook 
            Texture2D facebookTexture = ScreenManager.Game.Content.Load<Texture2D>("SocialLinks/Facebook");

            facebookButton = new Button()
            {
                Image = facebookTexture,
                Position = new Vector2(800 / 2 + 270,
                    480 / 2 + 5)
            };

            facebookButton.Clicked += socialLinks_Clicked;

            // Twitter
            Texture2D twitterTexture = ScreenManager.Game.Content.Load<Texture2D>("SocialLinks/twitter");

            twitterButton = new Button()
            {
                Image = twitterTexture,
                Position = new Vector2(800 / 2 + 340,
                    480 / 2)
            };

            twitterButton.Clicked += socialLinks_Clicked;
        }


        void socialLinks_Clicked()
        {
            
        }

        void thumbUpButton_Clicked()
        {
            UnityEngine.Application.OpenURL("market://details?id=com.suslikgames.mrorange"); 
        }

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
        }

        public virtual void HandleInput(GameTime gameTime, InputState input)
        {
            //facebookButton.HandleInput(gameTime, input);
            //twitterButton.HandleInput(gameTime, input);
            thumbUpButton.HandleInput(gameTime, input);
        }

        void DrawAdBlock()
        {
            //  ScreenManager.SpriteBatch.Draw(elementRenderer.Texture, Vector2.Zero, Color.White);
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            //facebookButton.Draw(ScreenManager.SpriteBatch);
            //twitterButton.Draw(ScreenManager.SpriteBatch);
            thumbUpButton.Draw(ScreenManager.SpriteBatch);

            DrawAdBlock();
        }
    }
}
