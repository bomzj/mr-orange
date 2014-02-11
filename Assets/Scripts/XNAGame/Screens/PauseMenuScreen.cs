using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using PushBlock.UI;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagementSample;
using Microsoft.Xna.Framework.Input;

namespace PushBlock.Screens
{
    class PauseMenuScreen : GameScreen
    {
        TextBlock title;
        List<Button> buttons = new List<Button>();
        InputAction backAction;

        public PauseMenuScreen()
        {
            TransitionOnTime = TransitionOffTime = new TimeSpan(0, 0, 0, 0, 500);

            backAction = new InputAction(
              new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
              true);
        }

        void LoadContent()
        {
            // Load fonts
            SpriteFont titleFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/Green");

            // Create Menu Title
            title = new TextBlock()
            {
                Font = titleFont,
                Text = "Paused",
            };
            
            SpriteFont buttonFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/LightGray");
            Texture2D buttonImage = ScreenManager.Game.Content.Load<Texture2D>("buttons/button");

            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Resume", Position = new Vector2(400, 155) });
            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Options", Position = new Vector2(400, 240) });
            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Levels", Position = new Vector2(400, 325) });
            buttons.Add(new Button() { Image = buttonImage, Font = buttonFont, Text = "Menu", Position = new Vector2(400, 410) });

            buttons[0].Clicked += ResumeGame;
            buttons[1].Clicked += ShowOptions;
            buttons[2].Clicked += NavigateToChooseLevelMenu;
            buttons[3].Clicked += NavigateToMainMenu;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
            }

        }

        void NavigateToMainMenu()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                ScreenManager.RemoveScreen(screen);
            }

            ScreenManager.AddScreen(new SummerBackgroundScreen(), ControllingPlayer);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
            
            ScreenManager.Game.SuppressDraw();
        }

        void NavigateToChooseLevelMenu()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                ScreenManager.RemoveScreen(screen);
            }

            ScreenManager.AddScreen(new SummerBackgroundScreen(), ControllingPlayer);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
            ScreenManager.AddScreen(new ChooseLevelScreen(), ControllingPlayer);

            ScreenManager.Game.SuppressDraw();
        }

        void ShowOptions()
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), ControllingPlayer);
            ScreenManager.Game.SuppressDraw();
        }

        void ResumeGame()
        {
            ExitScreen();
        }

        void UpdateButtonsLocations()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Update title transition
            Vector2 position = new Vector2(800 / 2, 50);
            position.Y -= transitionOffset * title.Height;
            title.Position = position;
            title.FontColor = Color.White * (1 - transitionOffset); ;


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
                ResumeGame();
            }

            foreach (Button button in buttons)
            {
                button.HandleInput(gameTime, input);
            }
        }

        public override void Update(  GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateButtonsLocations();

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
