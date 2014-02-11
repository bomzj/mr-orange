using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagementSample;
using PushBlock.UI;
using PushBlock.Helpers;
using Microsoft.Xna.Framework.Input;
using Assets.Scripts.XNAGame.Social;

namespace PushBlock.Screens
{
    class CreditsScreen : GameScreen
    {
        TextBlock menuTitle;
        
        TextBlock programmingTitle;
        TextBlock programmerName;
        
        TextBlock musicTitle;
        TextBlock musicianName;

        TextBlock newLevelsCommingSoon;
        int pulseDirection = 1;

        Button menuButton;

        bool gamePassed;

        InputAction backAction;

        SocialButtons socialButtons;

        public CreditsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(.5f);
            TransitionOffTime = TimeSpan.FromSeconds(.5f);

            backAction = new InputAction(
              new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
              true);
        }

        public CreditsScreen(bool gamePassed) :this()
        {
            this.gamePassed = gamePassed;
        }

        void LoadContent()
        {
            // Load fonts
            SpriteFont greenFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/Green");
            SpriteFont orangeFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/orange");
            SpriteFont buttonFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/LightGray");

            // Menu Title
            menuTitle = new TextBlock()
            {
                Font = greenFont,
                Text = gamePassed ? "The End" : "Credits",
            };

            if (!gamePassed)
            { 
                // Programming
                programmingTitle = new TextBlock()
                {
                    Font = orangeFont,
                    FontScale = 0.8f,
                    Text = "Programming and art",
                };

                programmerName = new TextBlock()
                {
                    Font = buttonFont,
                    FontScale = 1f,
                    Text = "Maksim Shamihulau",
                };

                // Music
                musicTitle = new TextBlock()
                {
                    Font = orangeFont,
                    FontScale = 0.8f,
                    Text = "Music",
                };

                musicianName = new TextBlock()
                {
                    Font = buttonFont,
                    FontScale = 1f,
                    Text = "Kevin MacLeod",
                };
            }

            // Create Buttons
            Texture2D buttonImage = ScreenManager.Game.Content.Load<Texture2D>("buttons/button");

            menuButton = new Button()
            {
                Image = buttonImage,
                Font = buttonFont,
                Text = gamePassed ? "Menu": "Back",
            };

            // Atach event handler
            if (!gamePassed)
            {
                menuButton.Clicked += OnBackClicked;

            }
            else
            {
                menuButton.Clicked += OnMenuClicked;

                newLevelsCommingSoon = new TextBlock()
                {
                    Font = orangeFont,
                    FontScale = 0.7f,
                    Position = new Vector2(800 / 2, 210),
                    LineSpacing = 15
                };
                newLevelsCommingSoon.MultilineText.AddRange(new[] 
                { 
                    "If you like the game please rate it", 
                    "and share with your friends in social networks!"
                });

                // social buttons
                socialButtons = new SocialButtons(ScreenManager);
                socialButtons.Initialize();
            }
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
            }
        }

        void OnBackClicked()
        {
            this.ExitScreen();
        }

        void OnMenuClicked()
        {
            foreach (GameScreen screen in ScreenManager.GetScreens())
            {
                ScreenManager.RemoveScreen(screen);
            }

            ScreenManager.AddScreen(new SummerBackgroundScreen(), ControllingPlayer);
            ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);

            ScreenManager.Game.SuppressDraw();
        }
        
        void UpdateScreenTransition()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Menu title
            Vector2 position = new Vector2(800 / 2, gamePassed ? 90 : 50);
            position.Y -= transitionOffset * 50;
            menuTitle.Position = position;
            menuTitle.FontColor = Color.White * TransitionAlpha;
            
            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            if (!gamePassed)
            { 
                // Programming
                programmingTitle.Position = new Vector2(position.X, position.Y + 80);
                programmingTitle.FontColor = Color.White * TransitionAlpha;

                programmerName.Position = new Vector2(position.X, position.Y + 130);
                programmerName.FontColor = Color.Gold * TransitionAlpha;

                // Music
                musicTitle.Position = new Vector2(position.X, position.Y + 200);
                musicTitle.FontColor = Color.White * TransitionAlpha;

                musicianName.Position = new Vector2(position.X, position.Y+250);
                musicianName.FontColor = Color.Gold * TransitionAlpha;
            }

            // Menu button
            position.Y = gamePassed ? 50 : 50; 
            position = new Vector2(800 / 2, position.Y + 330); 
            position.Y += transitionOffset * 50;
            menuButton.Position = position;
            menuButton.Color = Color.White * TransitionAlpha;

        }

        void PulseNewLevelsCommingSoonText(GameTime gameTime)
        {
            float scale = newLevelsCommingSoon.FontScale;
            scale = MathHelper.Clamp(scale +
                pulseDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * 0.12f,
                0.7f, 0.75f);

            if (scale == 0.75f) pulseDirection = -1;
            else if (scale == 0.7f) pulseDirection = 1;

            newLevelsCommingSoon.FontScale = scale;
        }

        public override void HandleInput(  GameTime gameTime, InputState input)
        {
            if (backAction.Evaluate(input))
            {
                if (gamePassed) OnMenuClicked();
                else ExitScreen();
            }

            menuButton.HandleInput(gameTime, input);
            if (gamePassed && ScreenState == ScreenState.Active)
            {
                socialButtons.HandleInput(gameTime, input);
            }
        }

        public override void Update(  GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateScreenTransition();
            
            if (gamePassed && ScreenState == ScreenState.Active)
            {
                //PulseNewLevelsCommingSoonText(gameTime);
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            menuTitle.Draw(ScreenManager.SpriteBatch);

            if (!gamePassed)
            { 
                programmingTitle.Draw(ScreenManager.SpriteBatch);
                programmerName.Draw(ScreenManager.SpriteBatch);

                musicTitle.Draw(ScreenManager.SpriteBatch);
                musicianName.Draw(ScreenManager.SpriteBatch);
            }

            menuButton.Draw(ScreenManager.SpriteBatch);

            if (gamePassed && ScreenState == ScreenState.Active)
            {
                newLevelsCommingSoon.Draw(ScreenManager.SpriteBatch);
                
                // social buttons
                socialButtons.Draw(gameTime);
            }
        }
    }
}
