using System;
using System.Threading;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GameStateManagement;
using PushBlock;
using PushBlock.Screens;
using PushBlock.Helpers;
using FarseerPhysics;

using System.IO;
using PushBlock.UI;
using Particles2DPipelineSample;
using PushBlock.FarseerPhysics;
using FarseerPhysics.Dynamics;
using System.Collections;
using System.Collections.Generic;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using System.Diagnostics;
using PushBlock.Blocks;
using System.ComponentModel;
using Assets.Scripts.XNAGame;

namespace GameStateManagementSample
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        SpriteFont whiteFont;
        float pauseAlpha;
        InputAction pauseAction;
        
        Level level;
        public int levelNumber;

        Texture2D grayscaleScreen;
        SummerBackgroundScreen backgroundScreen;

        bool isCoveredByOtherScreen;

        Button restartButton;
        Button menuButton;
        TextBlock levelTitle;

        // How To Play
        float howToPlayTextScaleDirection = 1;
        TextBlock howToPlayText;
        Sprite howToPlayArrow;

        bool levelCompleted;

        bool isPauseScreenDisplayed;

        #region Initialization

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
              new UnityEngine.KeyCode[] { UnityEngine.KeyCode.Escape },
              true);
        }

        public GameplayScreen(int levelNumber) : this()
        {
            this.levelNumber = levelNumber;
        }

        void LoadContent()
        {
            level = Level.LoadLevel(levelNumber, ScreenManager.Game);

            whiteFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/LightGray");
			
            backgroundScreen = new SummerBackgroundScreen(true);
            backgroundScreen.ScreenManager = ScreenManager;
            backgroundScreen.Activate(false);

            levelTitle = new TextBlock() 
            { 
                Font=ScreenManager.Game.Content.Load<SpriteFont>("Fonts/green"), 
                FontScale=0.7f,
                Text="Level " + levelNumber,
                Position=new Vector2(800/2, 25)
            };

            // Create buttons
            restartButton = new Button()
            {
                Image = ScreenManager.Game.Content.Load<Texture2D>("buttons/restart"),
                Position = new Vector2(765, 35),
            };
            restartButton.Clicked += RestartLevel;
            
            
            menuButton = new Button()
            {
                Image = ScreenManager.Game.Content.Load<Texture2D>("buttons/menu"),
                Position = new Vector2(35, 35),
            };
            menuButton.Clicked += OnPauseMenuClicked;

            // Set How To Play Hint if level is first
            if (level.LevelNumber == 1)
            {
                SpriteFont howToPlayFont = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/orange");
                
                howToPlayText = new TextBlock() 
                {
                    Font = howToPlayFont, 
                    FontScale=0.6f,
                    Position = new Vector2(220, 180),
                    LineSpacing = 10
                };
                howToPlayText.MultilineText.AddRange(new[] { "Click on block", "to remove it"});
                
                howToPlayArrow = new Sprite(ScreenManager.Game.Content.Load<Texture2D>("Other/OrangeArrow"),
                    new Vector2(300, 250), 1f, -2.35f);

                level.World.BodyRemoved += OnBodyRemoved;
            }
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                LoadContent();
            }
        }

        #endregion
              
                
        public static void LoadAndPlayLevel(int levelNumber, ScreenManager screenManager)
        {
            // Remove all screens
            foreach (GameScreen screen in screenManager.GetScreens())
            {
                screenManager.RemoveScreen(screen);
            }

            // Add gameplayscreen
            screenManager.AddScreen(new GameplayScreen(levelNumber), PlayerIndex.One);
        }

        public void ShowWinScreen()
        {
            ScreenManager.AddScreen(new WinScreen(level), null);
        }

        public void RestartLevel()
        {
            level.Dispose();
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new GameplayScreen(levelNumber), null);
        }

        void OnPauseMenuClicked()
        {
            ScreenManager.AddScreen(new PauseMenuScreen(), null);
            ScreenManager.Game.SuppressDraw();
            isPauseScreenDisplayed = true;
        }

        void OnBodyRemoved(Body body)
        {
            // Set second step of hint
            howToPlayText.MultilineText.Clear();
            howToPlayText.MultilineText.AddRange(new[] { "Put Mr Orange on", "the orange block" });
            howToPlayText.Position = new Vector2(620, 290);
            howToPlayArrow.Position = new Vector2(500,360);
            howToPlayArrow.Rotation = -1;
        }

        #region Update and Draw


        void UpdateHowToPlayNotification(GameTime gameTime)
        {
            if (level.MrOrange.IsOnOrange)
            {
                if (howToPlayText != null) howToPlayText.MultilineText.Clear();
                if (howToPlayArrow != null) howToPlayArrow = null;
            }
            else
            {
                // Set scale for message
                float scale = howToPlayText.FontScale;
                scale = MathHelper.Clamp(scale +
                    howToPlayTextScaleDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * 3 * 0.06f,
                    0.6f, 0.65f);

                if (scale == 0.65f) howToPlayTextScaleDirection = -1;
                else if (scale == 0.6f) howToPlayTextScaleDirection = 1;

                howToPlayText.FontScale = scale;

                // Set scale for arrow
                scale = howToPlayArrow.Scale;
                scale = MathHelper.Clamp(scale +
                    howToPlayTextScaleDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * 3 * 0.09f,
                    0.92f, 1f);

                howToPlayArrow.Scale = scale;
            }

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            // Update physics, animation
            if (!otherScreenHasFocus && !coveredByOtherScreen)  
            {
                if (!levelCompleted)
                {
                    if (level.MrOrange.IsOnGround) 
                    {
                        this.Perform(RestartLevel, 1000);
                        levelCompleted = true;
                    }
                    else if (level.MrOrange.IsOnOrange)
                    {
                        this.Perform(ShowWinScreen, 500);
                        levelCompleted = true;
                    }
                    else level.Update(gameTime);
                }

                backgroundScreen.Update(gameTime, otherScreenHasFocus, false);
                grayscaleScreen = null;

                // Update how to play hints for first level
                if (level.LevelNumber == 1) UpdateHowToPlayNotification(gameTime);
            }


            if (otherScreenHasFocus && !isPauseScreenDisplayed)
            {
                OnPauseMenuClicked();
            }

            isCoveredByOtherScreen = coveredByOtherScreen;
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        #region Perfrom method on main thread in a given delay

        private void Perform(Action myMethod, int delayInMilliseconds)
        {
            XNALauncher xnaLauncher = UnityEngine.MonoBehaviour.FindObjectOfType<XNALauncher>();
            float delay = (float)delayInMilliseconds / 1000;
            xnaLauncher.WaitAndExecute(delay, myMethod);
        }

        #endregion

        public override void HandleInput(  GameTime gameTime, InputState input)
        {
            if (!levelCompleted)
            {
                level.HandleInput(gameTime, input);

                menuButton.HandleInput(gameTime, input);
                restartButton.HandleInput(gameTime, input);

                // Call Pause Screen if we hit a back button or etc
//                if (pauseAction.Evaluate(input, null, out player))
//                {
//                    OnPauseMenuClicked();
//                }
            }
        }

        void DrawHowToPlayNotification()
        {
            // Draw How To Play Hint for first level
            if (level.LevelNumber == 1 && level.ElapsedTime.TotalSeconds > 0.7)
            {
                if (howToPlayText != null) howToPlayText.Draw(ScreenManager.SpriteBatch);
                if (howToPlayArrow != null) howToPlayArrow.Draw(ScreenManager.SpriteBatch);
            } 
        }

        private IEnumerator CaptureGrayscaledBackBufferData()
        {
            yield return new UnityEngine.WaitForEndOfFrame();

            var viewPort = VirtualViewport.Current;

            UnityEngine.Texture2D tex = new UnityEngine.Texture2D(viewPort.Width, viewPort.Height);
            tex.ReadPixels(new UnityEngine.Rect(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), 0, 0);
            tex.Apply();
            Texture2D grayscaleTexture = new Texture2D(tex);
            
            // Heavy operaton up to 400k iterations
            grayscaleScreen = Helper.GetGrayscaleTexture(grayscaleTexture);
        }

        void DrawGrayscaleScreen(GameTime gameTime)
        {
            if (grayscaleScreen != null)
            {
                ScreenManager.SpriteBatch.Draw(grayscaleScreen,
                        new Vector2(0,0),
                        Color.White, 
                        true);
            }
            else
            {
                // Draw screen without gui
                DrawScreen(gameTime, false);

                XNALauncher xnaLauncher = UnityEngine.MonoBehaviour.FindObjectOfType<XNALauncher>();
                xnaLauncher.StartCoroutine(CaptureGrayscaledBackBufferData());
            }
        }

        void DrawScreen(GameTime gameTime)
        {
            // Draw background
            backgroundScreen.Draw(gameTime);

            // Draw all blocks and hero face animation
            level.Draw(gameTime, ScreenManager.SpriteBatch);

            // GUI
            if (true)
            {
                menuButton.Draw(ScreenManager.SpriteBatch);
                restartButton.Draw(ScreenManager.SpriteBatch);
                levelTitle.Draw(ScreenManager.SpriteBatch);
            }

            // Draw How to play hints
            DrawHowToPlayNotification();
        }

        void DrawScreen(GameTime gameTime, bool showGUI)
        {
            // Draw background
            backgroundScreen.Draw(gameTime);

            // Draw all blocks and hero face animation
            level.Draw(gameTime, ScreenManager.SpriteBatch);

            // GUI
            if (showGUI)
            {
                menuButton.Draw(ScreenManager.SpriteBatch);
                restartButton.Draw(ScreenManager.SpriteBatch);

                levelTitle.Draw(ScreenManager.SpriteBatch);
            }

            // Draw How to play hints
            DrawHowToPlayNotification();
        }

        public override void Draw(  GameTime gameTime)
        {
            // Make screen grayscale if is covered by screen
            if (isCoveredByOtherScreen)
            {
                DrawGrayscaleScreen(gameTime);
            }
            else 
            {
                DrawScreen(gameTime);
                
                // If the game is transitioning on or off, fade it out to black.
                if (TransitionPosition > 0 || pauseAlpha > 0)
                {
                    float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                    ScreenManager.FadeBackBufferToBlack(alpha);
                }
            }
                        
        }


        #endregion
    }
}