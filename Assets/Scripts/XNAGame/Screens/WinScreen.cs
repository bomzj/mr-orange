using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PushBlock.UI;
using GameStateManagementSample;
using PushBlock.Helpers;
using System.Diagnostics;
using Particles2DPipelineSample;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using Assets.Scripts.XNAGame.Social;

namespace PushBlock.Screens
{
    class WinScreen : GameScreen
    {
        Texture2D goldStarTexture;
        Texture2D greyStarTexture;
        SpriteFont titleFont;

        TextBlock titleTextBlock;
        TextBlock scoreTextBlock;
        TextBlock newHighscoreTextBlock;
        int newHighscoreTextBlockScaleDirection = 1;

        HighScoreEntry newHighscore;
        HighScoreEntry oldHighscore;
        readonly TimeSpan TimeToCountScore = TimeSpan.FromMilliseconds(3000);
        TimeSpan scoreCountingTime;

        int displayedStarsCount;

        Level level;

        #region Buttons

        Button nextButton;
        Button replayButton;

        SocialButtons socialButtons;

        #endregion

        List<Sprite> goldStars = new List<Sprite>();
    
        ParticleSystem starDust;

        InputAction backAction;

        bool HasScreenDrawn;

        public WinScreen(Level level)
        {
            TransitionOnTime = TimeSpan.FromSeconds(.5f);
            TransitionOffTime = TimeSpan.FromSeconds(.5f);

            oldHighscore = HighScore.GetSavedHighScore(level.LevelNumber);
            newHighscore = HighScore.CalculateScore(level);

            if (oldHighscore != null)
            {
                if (newHighscore > oldHighscore) HighScore.AddHighScore(newHighscore);
            }
            else HighScore.AddHighScore(newHighscore);

            this.level = level;

            backAction = new InputAction(
               new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
               true);
        }

        void LoadContent()
        {
            // Load star textures
            goldStarTexture = ScreenManager.Game.Content.Load<Texture2D>("Other/BigStar");
            greyStarTexture = Helper.GetGrayscaleTexture(goldStarTexture);
             
            // Load fonts
            titleFont =  ScreenManager.Game.Content.Load<SpriteFont>("Fonts/Green");
            
            // Win Title
            titleTextBlock = new TextBlock() 
            { 
                Font = titleFont, 
                Text = "Completed", 
            };

            SpriteFont orangeFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/orange");

            // Score block
            scoreTextBlock = new TextBlock()
            {
                Font = orangeFont,
                FontScale = 0.8f,
                Text = "Score 0",
                IsCentered = false
            };

            // Highscore block
            newHighscoreTextBlock = new TextBlock()
            {
                Font = orangeFont,
                FontScale = 0.7f,
                Position = new Vector2(800 / 2 + 300, 60),
                LineSpacing= 5
            };
            newHighscoreTextBlock.MultilineText.AddRange(new[] { "New", "Highscore" });


            // Create Buttons
            SpriteFont buttonFont = ScreenManager.Game.Content.Load<SpriteFont>("fonts/LightGray");
            Texture2D buttonImage = ScreenManager.Game.Content.Load<Texture2D>("buttons/button");
            
            nextButton = new Button() 
            { 
                Image = buttonImage, 
                Font = buttonFont,
                Text="Next",
                Position = new Vector2(800 / 2, 380) 
            };

            if (level.LevelNumber == Level.LevelsCount)
            {
                nextButton.Clicked += OnCreditsClicked;
            }
            else nextButton.Clicked += PlayNextLevel;

            replayButton = new Button()
            {
                Image = buttonImage,
                Font = buttonFont,
                Text = "Replay",
                Position = new Vector2(800 / 2, 380)
            };

            replayButton.Clicked += ReplayLevel;

            // Create particle system once
            starDust = new ParticleSystem(ScreenManager.Game, "Particles/StarDust");
            starDust.Initialize();
            ScreenManager.Game.Components.Add(starDust);

           // Social buttons
            this.socialButtons = new SocialButtons(ScreenManager);
            this.socialButtons.Initialize();
        }

        public override void Unload()
        {
            level.Dispose();
            ScreenManager.Game.Components.Remove(starDust);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
            }
        }

        void SaveGameProgressAsync()
        {
            XNALauncher xnaLauncher = UnityEngine.MonoBehaviour.FindObjectOfType<XNALauncher>();
           
            Action saveAction = () =>
            {
                // Save passed levels
                Settings.Save();

                // Save achivements e.g. stars per level
                HighScore.Save();
            };

            xnaLauncher.ExecuteAsync(saveAction);
        }

        #region Button Events

        void OnCreditsClicked()
        {
            // Remove particles
            level.Dispose();
            ScreenManager.Game.Components.Remove(starDust);

            ScreenManager.AddScreen(new CreditsScreen(true), null);
            ScreenManager.Game.SuppressDraw();
        }

        void PlayNextLevel()
        {
            // Load new level
            int nextLevelNumber = level.LevelNumber + 1;

            // Save score/achivements
            int openLevelNumber = 1;

            if (!string.IsNullOrEmpty(Settings.GetValue("OpenLevelNumber")))
                openLevelNumber = int.Parse(Settings.GetValue("OpenLevelNumber"));

            Settings.SetValue("OpenLevelNumber", Math.Max(openLevelNumber, nextLevelNumber).ToString());

            // Save passed level number and scores for each level asynchronously
            SaveGameProgressAsync();

            UnityEngine.Debug.Log(level.LevelNumber % 3);

            // if this level is last go to menu, otherwise play next level
            if (nextLevelNumber <= Level.LevelsCount)
            {
#if UNITY_ANDROID
                // Show interstitial Chartboost ADs for every 3 level
                if (level.LevelNumber % 3 == 0)
                {
                    CBEventListener1.screenManager = this.ScreenManager;
                    CBEventListener1.levelToLoad = nextLevelNumber;

                    // Show async ads
                    Chartboost.CBBinding.showInterstitial(null);
                    
                }
                else
                {
                    GameplayScreen.LoadAndPlayLevel(nextLevelNumber, ScreenManager);
                }
                UnityEngine.Debug.Log("UNITY_ANDROID");
#else 
                UnityEngine.Debug.Log("no android");
                // For desktop , no ads logic
                GameplayScreen.LoadAndPlayLevel(nextLevelNumber, ScreenManager);
#endif
            }
            else
            {
                foreach (GameScreen item in ScreenManager.GetScreens()) ScreenManager.RemoveScreen(item);
                ScreenManager.AddScreen(new SummerBackgroundScreen(), PlayerIndex.One);
                ScreenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            }
            ExitScreen();
        }

        void ReplayLevel()
        {
            GameplayScreen.LoadAndPlayLevel(level.LevelNumber, ScreenManager);
        }


        #endregion

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // Hanlder back button
            if (backAction.Evaluate(input))
            {
                ReplayLevel();
            }

            nextButton.HandleInput(gameTime, input);
            replayButton.HandleInput(gameTime, input);
            socialButtons.HandleInput(gameTime, input);
        }
        
        void UpdateScreenTransition()
        {
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            int baseHeight = 60;

            // Update title transition
            Vector2 position = new Vector2(800 / 2, baseHeight);
            position.Y -= transitionOffset * 50;
            titleTextBlock.Position = position;
            titleTextBlock.FontColor = Color.White * TransitionAlpha;

            // Update Score position
            position = new Vector2(800 / 2
                - scoreTextBlock.Font.MeasureString("Score 325").X * scoreTextBlock.FontScale / 2,
                baseHeight + 200);
            
            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            scoreTextBlock.Position = position;
            scoreTextBlock.FontColor = Color.White * TransitionAlpha; 
            
            // Update play next button postition
            position = new Vector2(800 / 2 + 100, baseHeight + 290) ;
            position.Y += transitionOffset * 150;
            nextButton.Position = position;
            nextButton.Color = Color.White * TransitionAlpha;

            // Update replay button postition
            position = new Vector2(800 / 2 - 100, baseHeight + 290) ;
            position.Y += transitionOffset * 50;
            replayButton.Position = position;
            replayButton.Color = Color.White * TransitionAlpha;
        }

        void UpdateScore(  GameTime gameTime)
        {
            if (this.ScreenState == GameStateManagement.ScreenState.Active)
            {
                // Execute this block while score is calculated
                if (scoreCountingTime < TimeToCountScore)
                {
                    // Score tweening
                    double scoreRatio = MathHelper.Clamp((float)(scoreCountingTime.TotalSeconds / TimeToCountScore.TotalSeconds), 0, 1);

                    scoreTextBlock.Text = "Score " + (int)(newHighscore.Score * scoreRatio);
                    scoreCountingTime += gameTime.ElapsedGameTime;
                }
                else // Score calculation finished
                {
                    
                    bool starAnimationCompleted = goldStars.Count == newHighscore.GoldStars && newHighscore.GoldStars > 0 ?
                        goldStars[goldStars.Count-1].Scale == 1: false;
                    
                    if (starAnimationCompleted || newHighscore.GoldStars == 0)
                    {
                        switch (newHighscore.GoldStars)
                        {
                            case 1: titleTextBlock.Text = "Good"; break;
                            case 2: titleTextBlock.Text = "Excellent"; break;
                            case 3: titleTextBlock.Text = "Awesome!"; break;
                        }

                        // Show New Highscore achieved
                        if (oldHighscore != null && newHighscore > oldHighscore)
                        {
                            float scale = newHighscoreTextBlock.FontScale;
                            scale = MathHelper.Clamp(scale +
                                newHighscoreTextBlockScaleDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * 0.18f,
                                0.7f, 0.75f);

                            if (scale == 0.75f) newHighscoreTextBlockScaleDirection = -1;
                            else if (scale == 0.7f) newHighscoreTextBlockScaleDirection = 1;

                            newHighscoreTextBlock.FontScale = scale;
                        }
                    }
                }
            }
            
        }

        void UpdateGoldStars(  GameTime gameTime)
        {
            if (this.ScreenState == GameStateManagement.ScreenState.Active)
            {
                double timeSpan = TimeToCountScore.TotalMilliseconds / 3;
                int timeSpanIndex = (int)Math.Floor(scoreCountingTime.TotalMilliseconds / timeSpan);

                if (displayedStarsCount < timeSpanIndex
                    && newHighscore.GoldStars > goldStars.Count)
                {
                    Vector2 center = new Vector2(800,
                                    480) / 2;

                    Vector2 starPosition = center - new Vector2(105, 80) + new Vector2(105 * goldStars.Count, 0);
                    starDust.AddParticles(starPosition, Vector2.Zero, Color.White);
                    starDust.AddParticles(starPosition, Vector2.Zero, new Color(255, 171, 18));
                    starDust.AddParticles(starPosition, Vector2.Zero, Color.Yellow);

                    // Create new star
                    Sprite star = new Sprite(goldStarTexture, starPosition);
                    star.Scale = 0f;
                    goldStars.Add(star);

                    AudioManager.PlaySound("StarCollected");

                    displayedStarsCount++;
                }
            }
            
            // Increase scale factor from zero to one for each star
            foreach (Sprite star in goldStars)
            {
                float scale = (float)(2 * gameTime.ElapsedGameTime.TotalSeconds);
                star.Scale = MathHelper.Clamp(star.Scale + scale , 0, 1);
            }
        }

        public override void Update(  GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateScore(gameTime);
            UpdateGoldStars(gameTime);
            UpdateScreenTransition();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            // Need to skip first frame to sync with gameplay screen for grayscale capture
            if (!HasScreenDrawn)
            {
                HasScreenDrawn = true;
                return;
            }
            
            Vector2 center = new Vector2(800, 480) / 2;

            // Draw Title
            titleTextBlock.Draw(ScreenManager.SpriteBatch);

            // Draw Stars
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 starPosition = center - new Vector2(105, 80);

            if (ScreenState == ScreenState.TransitionOn)
                starPosition.X -= transitionOffset * 256;
            else
                starPosition.X += transitionOffset * 512;

            // Draw Star Placeholders
            for (int i = 0; i < 3; i++)
            {
                if (goldStars.Count > i && goldStars[i].Scale == 1)
                {
                    starPosition += new Vector2(105, 0);
                    continue;
                }

                ScreenManager.SpriteBatch.Draw(greyStarTexture, starPosition, null, Color.White * TransitionAlpha, 0,
                            new Vector2(goldStarTexture.Width / 2, goldStarTexture.Height / 2), 1f, SpriteEffects.None, 0);

                starPosition += new Vector2(105, 0);
            }

            foreach (Sprite star in goldStars)
            {
                star.Color = Color.White * TransitionAlpha;
                star.Draw(ScreenManager.SpriteBatch);
            }

            // Draw Score
            scoreTextBlock.Draw(ScreenManager.SpriteBatch);

            // Draw New Highscore achieved
            if (oldHighscore != null && newHighscore > oldHighscore)
            {
                bool starAnimationCompleted = goldStars.Count == newHighscore.GoldStars && newHighscore.GoldStars > 0 ?
                        goldStars[goldStars.Count - 1].Scale == 1 : false;

                // Show when calculation is finished
                if (starAnimationCompleted)
                {
                    newHighscoreTextBlock.Draw(ScreenManager.SpriteBatch);
                }
            }

            // Draw Buttons
            nextButton.Draw(ScreenManager.SpriteBatch);
            replayButton.Draw(ScreenManager.SpriteBatch);

            // Social buttons
            socialButtons.Draw(gameTime);
        }
    }
}
