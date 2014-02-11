using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using PushBlock.Screens;
using System.Threading;
using PushBlock.Helpers;
using GameStateManagement;
using GameStateManagementSample;
using System.Diagnostics;
using UnityEngine;
using PushBlock;
using Assets.Scripts.XNAGame.Helpers;

namespace XNAGame
{
    public class Game1 : Game
    {
        ScreenManager screenManager;
        
        public Game1()
        {
            InitGraphics();

            this.GamePaused += Game1_GamePaused;
            this.GameResumed += Game1_GameResumed;

            UnityEngine.Debug.Log("SystemInfo.deviceUniqueIdentifier " + SystemInfo.deviceUniqueIdentifier);
            UnityEngine.Debug.Log("Application.dataPath = " + Application.dataPath);
            UnityEngine.Debug.Log("Application.persistentDataPath = " + Application.persistentDataPath);
        }

        void InitGraphics()
        {
            // Set up graphics
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
        }

        protected override void Update(GameTime gameTime)
        {
            var tempComponents = Components.ToArray();
            foreach (var item in tempComponents)
            {
                IUpdateable component = item as IUpdateable;
                if (component != null)
                {
                    component.Update(gameTime);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            var tempComponents = Components.ToArray();
            foreach (var item in Components)
            {
                IDrawable component = item as IDrawable;
                if (component != null)
                {
                    component.Draw(gameTime);
                }
            }
        }

        void InitializeScreenManager()
        {
            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);
            screenManager.Initialize();

            AddInitialScreens(screenManager);
        }

        void AddInitialScreens(ScreenManager screenManager)
        {
            // Activate the first screens.
            screenManager.AddScreen(new SplashScreen(), null);
        }

        void InitializeAudioManager()
        {
            AudioManager audio = new AudioManager(this);

            audio.LoadSounds();
            audio.LoadMusic();

            // load audio settings
            if (!string.IsNullOrEmpty(Settings.GetValue("MusicVolume")))
                AudioManager.MusicVolume = float.Parse(Settings.GetValue("MusicVolume"));

            if (!string.IsNullOrEmpty(Settings.GetValue("SoundVolume")))
                AudioManager.SoundVolume = float.Parse(Settings.GetValue("SoundVolume"));

        }

        void InitializeHelper()
        {
            Helper.Game = this;
            Helper.Content = Content;
        }
        
        protected override void LoadContent()
        {
           Content.RootDirectory = "Content";

           InitializeScreenManager();
           InitializeAudioManager();
           InitializeHelper();
        }

        void Game1_GameResumed()
        {
        }

        void Game1_GamePaused()
        {
        }

    }
}
