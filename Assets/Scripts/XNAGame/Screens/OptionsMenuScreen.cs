#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PushBlock;
using Microsoft.Xna.Framework.Media;
using PushBlock.UI;
using System;
#endregion

using GameStateManagement;
using PushBlock.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameStateManagementSample
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : GameScreen
    {
        #region Fields

        TextBlock menuTitleTextBlock;
        TextBlock soundTextBlock;
        TextBlock musicTextBlock;
        
        Slider soundSlider;
        Slider musicSlider;

        Button okButton;

        InputAction backAction;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
        {
            TransitionOnTime = TransitionOffTime = TimeSpan.FromMilliseconds(500);

            backAction = new InputAction(
              new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
              true);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                // Options menu title
                SpriteFont menuTitleFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/Green");  
                menuTitleTextBlock = new TextBlock() { Font = menuTitleFont, Text = "Options" };

                // Create Sound slider
                soundSlider = new Slider("Buttons/SliderBar", "Buttons/Slider",
                    new Vector2(800 / 2, 480 / 2),
                    ScreenManager.Game.Content);

                soundSlider.Value = AudioManager.SoundVolume;
                soundSlider.ValueChanged += new Action(soundSlider_ValueChanged);

                SpriteFont optionFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/Orange");

                soundTextBlock = new TextBlock()
                {
                    Font = optionFont,
                    FontScale = 1f,
                    Text = "Sound",
                    Position = new Vector2(800 / 2, 270),
                };

                // Create Music slider
                musicSlider = new Slider("Buttons/SliderBar", "Buttons/Slider", 
                    new Vector2(800 / 2, 480 / 2 + 100),
                    ScreenManager.Game.Content);

                musicSlider.Value = AudioManager.MusicVolume;
                musicSlider.ValueChanged += new Action(musicSlider_ValueChanged);

                musicTextBlock = new TextBlock()
                {
                    Font = optionFont,
                    FontScale = 1f,
                    Text = "Music",
                    Position = new Vector2(800 / 2, 270),
                };


                // Create OK Button
                SpriteFont buttonFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/LightGray");
                Texture2D buttonImage = ScreenManager.Game.Content.Load<Texture2D>("buttons/button");

                okButton = new Button()
                {
                    Image = buttonImage,
                    Font = buttonFont,
                    Text = "Ok",
                    Position = new Vector2(800 / 2, 380)
                };

                okButton.Clicked += () => 
                {
                    Settings.SetValue("MusicVolume", AudioManager.MusicVolume.ToString());
                    Settings.SetValue("SoundVolume", AudioManager.SoundVolume.ToString());
                    Settings.Save();

                    ExitScreen(); 
                };
            }

        }

        void musicSlider_ValueChanged()
        {
            AudioManager.MusicVolume = musicSlider.Value;
            if (musicSlider.Value == 0)
            {
                AudioManager.StopMusic();
            }
            else if (musicSlider.Value > 0 && !MediaPlayer.IsPlaying)
            {
                AudioManager.PlayMusic("beach party");
            }
        }

        void soundSlider_ValueChanged()
        {
            AudioManager.SoundVolume = soundSlider.Value;
        }

        #endregion

        #region Handle Input

        public override void HandleInput(  GameTime gameTime, GameStateManagement.InputState input)
        {
            
            if (backAction.Evaluate(input))
            {
                ExitScreen();
            }
            
            soundSlider.HandleInput(input);
            musicSlider.HandleInput(input);
            okButton.HandleInput(gameTime, input);
        }
        
        #endregion

        void UpdateScreenTransition()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Update title transition
            UpdateMenuTitle();

            Vector2 position = new Vector2(800 / 2, 0);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // Update Sound slider items
            soundTextBlock.Position = position - new Vector2(150, -160);
            soundTextBlock.FontColor = Color.White * TransitionAlpha;

            soundSlider.Position = position + new Vector2(80, +160);
            soundSlider.Color = Color.White * TransitionAlpha;

            // Update Music slider items
            musicTextBlock.Position = position - new Vector2(150, -250);
            musicTextBlock.FontColor = Color.White * TransitionAlpha;

            musicSlider.Position = position + new Vector2(80, +250);
            musicSlider.Color = Color.White * TransitionAlpha;

            // Update Button postition
            position = new Vector2(800 / 2, 380);
            position.Y += transitionOffset * 50;
            okButton.Position = position;
            okButton.Color = Color.White * TransitionAlpha;
        }

        void UpdateMenuTitle()
        {
            // Update menu title transition
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 position = new Vector2(800 / 2, 50);
            position.Y -= transitionOffset * 50;//title.Height;
            menuTitleTextBlock.Position = position;
            menuTitleTextBlock.FontColor = Color.White * (1 - transitionOffset);
        }

        public override void Update(  GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateScreenTransition();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(  GameTime gameTime)
        {
            menuTitleTextBlock.Draw(ScreenManager.SpriteBatch);

            // Modify the alpha to fade text out during transitions.
            soundSlider.Color = Color.White * this.TransitionAlpha;
            musicSlider.Color = Color.White * this.TransitionAlpha;

            soundTextBlock.Draw(ScreenManager.SpriteBatch);
            soundSlider.Draw(this.ScreenManager.SpriteBatch);

            //musicTextBlock.FontColor = new Color(125, 255, 125, 50);
            musicTextBlock.Draw(ScreenManager.SpriteBatch);
            musicSlider.Draw(this.ScreenManager.SpriteBatch);

            okButton.Draw(ScreenManager.SpriteBatch);
        }

    }
}
