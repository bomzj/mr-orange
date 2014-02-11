using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameStateManagement;
using GameStateManagementSample;
using PushBlock.Helpers;
using Microsoft.Xna.Framework.Input;

namespace PushBlock.Screens
{
    class SplashScreen : GameScreen
    {
        TimeSpan showTime;
        TimeSpan elapsedTime;
        Sprite logo;
        float pauseAlpha;
        
        InputAction backAction;

        public SplashScreen()
        {
            showTime = TimeSpan.FromMilliseconds(2000);
            TransitionOnTime = TimeSpan.FromSeconds(3);
            TransitionOffTime = TimeSpan.FromSeconds(2);

            backAction = new InputAction(
              new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
              true);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                var logoTexture = ScreenManager.Content.Load<Texture2D>("SplashScreen/SuslikGamesLogo");
                var logoPosition = new Vector2(800 / 2,
                    480 / 2);

                logo = new Sprite(logoTexture, logoPosition);
            }
        }

        public override void Unload()
        {
            ScreenManager.AddScreen(new SummerBackgroundScreen(), ControllingPlayer);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
            ScreenManager.Game.SuppressDraw();
        }

        public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input)
        {
            // Hanlder back button
            if (backAction.Evaluate(input))
            {
                //ScreenManager.Game.Exit();
                UnityEngine.Application.Quit();
            }
        }

        public override void Update(  GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (ScreenState == GameStateManagement.ScreenState.Active)
            {
                if (elapsedTime > showTime)
                {
                    this.ExitScreen();
                }

                elapsedTime += gameTime.ElapsedGameTime;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(  GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(1f);
            logo.Draw(ScreenManager.SpriteBatch);

            float alpha = (float)Math.Pow(TransitionAlpha, 2);
			ScreenManager.FadeBackBufferToBlack(1f - alpha);
        }
    }
}
