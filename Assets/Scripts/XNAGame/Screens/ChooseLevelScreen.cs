using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PushBlock.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PushBlock.Helpers;
using GameStateManagement;
using GameStateManagementSample;
using System.Diagnostics;


namespace PushBlock.Screens
{
    class ChooseLevelScreen : GameScreen
    {
        const int totalLevels = 30;
        const int levelsPerPage = 10;
        const int levelsPerRow = 5;
        const int totalPages = totalLevels / levelsPerPage;
        const int levelsRowsPerPage = levelsPerPage / levelsPerRow;
        
        const int scrollSpaceBetweenPages = 600;
        int scrollSpace;
        readonly TimeSpan scrollTimeBetweenPages = TimeSpan.FromMilliseconds(500);
        TimeSpan scrollTime;
        

        readonly int openLevels;
        
        Vector2 levelButtonStartPosition = new Vector2(160, 140);
        int spaceBetweenButtonsByX = 120;
                
        List<Button> buttons = new List<Button>();
        List<Button> navigationButtons = new List<Button>();
               
        int currentPage = 0;
        
        Texture2D levelButtonTexture;
        Texture2D levelButtonDisabledTexture;

        Texture2D smallGoldStarTexture;
        Texture2D smallGrayStarTexture;

        Texture2D arrowRightButtonTexture;
        Texture2D arrowLeftButtonTexture;

        Texture2D buttonTexure;
        SpriteFont buttonFont;

        TextBlock title;

        InputAction backAction;
        
        public ChooseLevelScreen()
        {
            if (!string.IsNullOrEmpty(Settings.GetValue("OpenLevelNumber")))
                openLevels = int.Parse(Settings.GetValue("OpenLevelNumber"));
            else openLevels = 1;

            TransitionOnTime = TransitionOffTime = scrollTimeBetweenPages;

            backAction = new InputAction(
               new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
               true);
        }

        void LoadContent()
        {
            buttonTexure = ScreenManager.Game.Content.Load<Texture2D>("buttons/button");
            buttonFont =  ScreenManager.Game.Content.Load<SpriteFont>("fonts/LightGray");
            
            levelButtonTexture = ScreenManager.Game.Content.Load<Texture2D>("buttons/levelButton");
            levelButtonDisabledTexture = Helper.GetGrayscaleTexture(levelButtonTexture);

            smallGoldStarTexture = ScreenManager.Game.Content.Load<Texture2D>("Other/SmallStar");
            smallGrayStarTexture = Helper.GetGrayscaleTexture(smallGoldStarTexture);
            
            arrowRightButtonTexture = ScreenManager.Game.Content.Load<Texture2D>("buttons/RightScroll");
            arrowLeftButtonTexture = Helper.GetHorizontallyFlippedTexture(arrowRightButtonTexture);

            SpriteFont menuTitleFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/Green");
            title = new TextBlock() { Font=menuTitleFont, Text="Choose Level" };
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
                CreateLevelButtons();
                CreateNavigationButtons();
            }
        }

        Button CreateLevelButton(Vector2 position, int levelNumber)
        {
            LevelButton button = new LevelButton();
            button.Position = position;
            button.Text = levelNumber.ToString();
            button.Font = buttonFont;
            button.GoldStarTexture = smallGoldStarTexture;
            button.GrayStarTexture = smallGrayStarTexture;
            PushBlock.Helpers.HighScoreEntry highscore = HighScore.GetSavedHighScore(levelNumber);
            button.GoldStarsCount = highscore != null ? highscore.GoldStars : 0;

            // Open or closed level
            if (openLevels >= levelNumber)
            {
                button.Image = levelButtonTexture;
                button.Clicked += () => GameplayScreen.LoadAndPlayLevel(levelNumber, ScreenManager);
            }
            else
            {
                button.Image = levelButtonDisabledTexture;
            }

            return button;
        }

        void CreateLevelButtons()
        {
            for (int page = 0; page < totalPages; page++)
            {
                // Set button's base position depending on page number
                Vector2 position = levelButtonStartPosition + page * new Vector2(scrollSpaceBetweenPages, 0);

                for (int row = 0; row < levelsRowsPerPage; row++)
                {
                    // Add Y offset for button's position
                    position += new Vector2(0, row * spaceBetweenButtonsByX + 20);

                    for (int column = 0; column < levelsPerRow; column++)
                    {
                        int levelNumber = page * levelsPerPage + row * levelsPerRow + column + 1;
                        
                        // Add X offset for button's position
                        Vector2 totalPostion = position + new Vector2(column * spaceBetweenButtonsByX, 0);

                        buttons.Add(CreateLevelButton(totalPostion, levelNumber));
                    }
                }
            }

            
        }

        void UpdateLevelButtonLocations(  GameTime gameTime)
        {
            if (scrollTime != TimeSpan.Zero)
            {
                float transitionPosition = (float)(scrollTime.TotalSeconds / scrollTimeBetweenPages.TotalSeconds);
                float transitionOffset = (float)Math.Pow(transitionPosition, 2);

                int scrollVelocity = (int)(scrollSpace / scrollTime.TotalSeconds);
                Vector2 scrollStep = new Vector2((int)(scrollVelocity * gameTime.ElapsedGameTime.TotalSeconds), 0);

                if (Math.Abs(scrollStep.X) > Math.Abs(scrollSpace)) scrollStep.X = scrollSpace;


                // update each menu entry's location in turn
                foreach (Button button in buttons)
                {
                    button.Position += scrollStep;

                    if (buttons.IndexOf(button) / levelsPerPage == currentPage)
                    {
                        if (ScreenState == GameStateManagement.ScreenState.TransitionOff)
                        {
                            // Transition off
                            //button.Color = Color.White * transitionOffset;
							button.Color = Color.White;// * transitionOffset;
							button.Color.A = (byte)(Color.White.A * transitionOffset);
                        }
                        else
                        {
                            // Transition On
                            button.Color = Color.White;// * (1 - transitionOffset);
							button.Color.A = (byte)(Color.White.A * (1- transitionOffset));
                        }

                       
                    }
                    else if (ScreenState == GameStateManagement.ScreenState.Active)
                    {
                        // Transition off 
                        button.Color = Color.White;// * transitionOffset;
						button.Color.A = (byte)(Color.White.A * transitionOffset);
                    }
					
					

                }

                scrollSpace -= (int)scrollStep.X;
                scrollTime -= gameTime.ElapsedGameTime;
                if (scrollTime.TotalSeconds < 0) scrollTime = TimeSpan.Zero;
            }
            else if (ScreenState == GameStateManagement.ScreenState.TransitionOn)
            {
                // Adjust buttons' start position and transparency for sliding effect
                foreach (Button button in buttons)
                {
                    button.Position -= new Vector2(256, 0);
                    button.Color = Color.White * 0;
                }
                scrollSpace = 256;
                scrollTime = scrollTimeBetweenPages;
            }
            else if (ScreenState == GameStateManagement.ScreenState.TransitionOff)
            {
                scrollSpace = 512;
                scrollTime = scrollTimeBetweenPages;
            }
        }

        void ScrollLeft()
        {
            if (currentPage > 0)
            {
                currentPage--;

                if (scrollTime == TimeSpan.Zero) scrollTime = scrollTimeBetweenPages;

                scrollSpace += scrollSpaceBetweenPages;
            }
        }

        void ScrollRight()
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;

                if (scrollTime == TimeSpan.Zero) scrollTime = scrollTimeBetweenPages;

                scrollSpace -= scrollSpaceBetweenPages;

            }
        }

        void CreateNavigationButtons()
        {
            Button backButton = new Button() { Image = arrowLeftButtonTexture, Position = new Vector2(50, 230) };
            Button nextButton = new Button() { Image = arrowRightButtonTexture, Position = new Vector2(750, 230) };
            Button menuButton = new Button() 
                { 
                    Image = ScreenManager.Game.Content.Load<Texture2D>("buttons/back"), 
                    Position = new Vector2(50, 430), 
                };

            backButton.Clicked += ScrollLeft;
            nextButton.Clicked += ScrollRight;
            menuButton.Clicked += () => { this.ExitScreen(); };

            navigationButtons.Add(backButton);
            navigationButtons.Add(nextButton);
            navigationButtons.Add(menuButton);
        }

        void UpdateNavigationButtons(GameTime gameTime)
        {
            navigationButtons[0].Color = currentPage == 0 ? Color.White * 0 : Color.White;
            navigationButtons[1].Color = currentPage == totalPages - 1 ? Color.White * 0 : Color.White;
            

            if (ScreenState != GameStateManagement.ScreenState.Active)
            {
                float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
                navigationButtons[0].Color *= 1 - transitionOffset;
                navigationButtons[1].Color *= 1 - transitionOffset;

                navigationButtons[2].Color = Color.White * (1 -transitionOffset);
            }
        }

        void UpdateMenuTitle()
        {
            // Update menu title transition
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 position = new Vector2(800 / 2, 50);
            position.Y -= transitionOffset * 50;//title.Height;
            title.Position = position;
            title.FontColor = Color.White * (1 - transitionOffset);
        }

        public override void HandleInput(  GameTime gameTime, InputState input)
        {
            if (backAction.Evaluate(input))
            {
                ExitScreen();
            }

            foreach (Button button in buttons)
            {
                // Handle input for current page buttons only when button is active (scrolling movement is off)
                if (scrollTime == TimeSpan.Zero
                    && buttons.IndexOf(button) / levelsPerPage == currentPage
                    && button.Image == levelButtonTexture)
                {
                    button.HandleInput(gameTime, input);
                }
            }

            foreach (Button button in navigationButtons)
            {
                button.HandleInput(gameTime, input);
            }
        }

        public override void Update(  GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // update navigation is here because of Draw executes first then Update
            UpdateMenuTitle();
            UpdateNavigationButtons(gameTime);
            UpdateLevelButtonLocations(gameTime);

            // Base Update screen transitions and etc
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(  GameTime gameTime)
        {
            title.Draw(ScreenManager.SpriteBatch);
            foreach (Button button in buttons) button.Draw(ScreenManager.SpriteBatch);
            foreach (Button button in navigationButtons) button.Draw(ScreenManager.SpriteBatch);
        }
    }
}
