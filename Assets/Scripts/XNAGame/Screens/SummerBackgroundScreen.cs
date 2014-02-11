#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using PushBlock.Sky;
using PushBlock.Helpers;
using PushBlock;
using Microsoft.Xna.Framework.Media;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class SummerBackgroundScreen : GameScreen
    {
        Texture2D blueBackground;
        Texture2D summerBackgroundWithTrees;
               
        SkyManager skyManager;

        bool isGameplayBackground;

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public SummerBackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public SummerBackgroundScreen(bool isGameplayBackground)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.isGameplayBackground = isGameplayBackground;
        }

        void LoadContent()
        {
            blueBackground = ScreenManager.Game.Content.Load<Texture2D>("backgrounds/BlueBackground");
            summerBackgroundWithTrees = ScreenManager.Game.Content.Load<Texture2D>("backgrounds/SummerBackgroundWithTrees");

            // Create sky
            skyManager = new SkyManager(ScreenManager.Game.Content);
            int tries = 0;
            int maxCloudsCount = 3;
            int cloudsCount = 0;

            while (tries < 6 && cloudsCount < maxCloudsCount)
            {
                bool cloudCreated = skyManager.GenerateCloud(new Rectangle(0, 0, 800, 180));
                if (cloudCreated) cloudsCount++;
                tries++;
            }

            if (!MediaPlayer.IsPlaying)
            {
                AudioManager.PlayMusic("beach party");
            }
        }
        
        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
            }
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            // Update sky
            if (Helper.Game.IsActive) skyManager.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, false);
        }
       

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            //Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, 800, 480);

            spriteBatch.Draw(blueBackground, new Vector2(0, 0), Color.White);
            
            int height = 480 - (summerBackgroundWithTrees.Height);
            if (isGameplayBackground) height -= 40;
            spriteBatch.Draw(summerBackgroundWithTrees, new Vector2(0, height), Color.White);

            // Draw sky
            skyManager.Draw(gameTime, spriteBatch);
        }


        #endregion
    }
}
