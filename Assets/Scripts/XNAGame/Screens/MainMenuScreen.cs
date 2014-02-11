using Microsoft.Xna.Framework;
using PushBlock.Screens;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using PushBlock.UI;
using PushBlock;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;

namespace GameStateManagementSample
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : GameScreen
    {
        Sprite title;
        List<Button> buttons = new List<Button>();

        InputAction backAction;

        #region Initialization

        public MainMenuScreen()
        {
            TransitionOnTime= TransitionOffTime = new TimeSpan(0, 0, 0, 0, 500);

            backAction = new InputAction(
               new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
               true);
        }

        #endregion

        void LoadContent()
        {
            title = new Sprite();
            title.Texture = ScreenManager.Game.Content.Load<Texture2D>("Other/MrOrangeTitle");
            title.Position = new Vector2(800 / 2  - title.Width / 2, 0);

            SpriteFont buttonFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/LightGray");
            Texture2D buttonImage = ScreenManager.Game.Content.Load<Texture2D>("buttons/button");
            
            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Play", Position=new Vector2(400, 165) });
            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Options", Position = new Vector2(400, 250) });
            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Credits", Position = new Vector2(400, 335) });
            //buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Exit", Position = new Vector2(400, 420) });

            buttons[0].Clicked += Play_Clicked;
            buttons[1].Clicked += Options_Clicked;
            buttons[2].Clicked += Credits_Clicked;
            //buttons[3].Clicked += Exit_Clicked;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
            }
        }

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void Play_Clicked()
        {
            UnityEngine.Debug.Log("Play_Clicked");
            ScreenManager.AddScreen(new ChooseLevelScreen(), null);
            ScreenManager.Game.SuppressDraw();
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void Options_Clicked()
        {
            UnityEngine.Debug.Log("Options_Clicked");
            ScreenManager.AddScreen(new OptionsMenuScreen(), null);
            ScreenManager.Game.SuppressDraw();
        }

        void Credits_Clicked()
        {
            UnityEngine.Debug.Log("Credits_Clicked");
            ScreenManager.AddScreen(new CreditsScreen(), null);
            ScreenManager.Game.SuppressDraw();
        }

        void Exit_Clicked()
        {
            //ScreenManager.Game.Exit();
#if UNITY_ANDROID
            UnityEngine.Application.Quit();
#endif
            
        }
      
        #endregion

        void UpdateScreenTransition()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Update title transition
            Vector2 position = new Vector2(800 / 2, title.Height /2);
            position.Y -= transitionOffset * title.Height;
            title.Position = position;
            title.Color = Color.White * TransitionAlpha;


            // update each menu entry's location in turn
            for (int i = 0; i < buttons.Count; i++)
            {
                Button button = buttons[i];
                position = button.Position;

                // each entry is to be centered horizontally
                position.X = 800 / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                button.Position = position;

                button.Color = Color.White * TransitionAlpha;
            }
        }

        public override void HandleInput(  GameTime gameTime, InputState input)
        {
            if (backAction.Evaluate(input))
            {
                Exit_Clicked();
            }

            foreach (Button button in buttons)
            {
                button.HandleInput(gameTime, input);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateScreenTransition();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(  GameTime gameTime)
        {
            // Draw Game title
            title.Draw(ScreenManager.SpriteBatch);

            foreach (Button button in buttons)
            {
                button.Draw(ScreenManager.SpriteBatch);
            }

        }
    }
}

