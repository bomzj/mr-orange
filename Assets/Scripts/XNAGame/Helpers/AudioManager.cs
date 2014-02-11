#region File Description
//-----------------------------------------------------------------------------
// AudioManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using PushBlock.Helpers;


#endregion

namespace PushBlock
{
    /// <summary>
    /// Component that manages audio playback for all sounds.
    /// </summary>
    public class AudioManager : GameComponent
    {
        const string soundAssetLocation = "Sounds/";

        // Audio Data        
        static Dictionary<string, SoundEffect> soundBank = new Dictionary<string, SoundEffect>();
        static Dictionary<string, Song> musicBank = new Dictionary<string, Song>();

        // Audio Volume
        //static float musicVolume = 1f;
        public static float MusicVolume
        {
            get 
            { 
                return MediaPlayer.Volume; 
            }
            set 
            {
                MediaPlayer.Volume = value;
            }
        }

        static float soundVolume = 1f;
        public static float SoundVolume
        {
            get { return soundVolume; }
            set 
            {
                soundVolume = MathHelper.Clamp(value, 0, 1);
            }
        }

        #region Initialization

        public AudioManager(  Game game)
            : base(game) 
        {
        }

       
        #endregion

        #region Loading Methodes


        /// <summary>
        /// Loads a single sound into the sound manager, giving it a specified alias.
        /// </summary>
        /// <param name="contentName">The content name of the sound file. Assumes all sounds are located under
        /// the "Sounds" folder in the content project.</param>
        /// <param name="alias">Alias to give the sound. This will be used to identify the sound uniquely.</param>
        /// <remarks>Loading a sound with an alias that is already used will have no effect.</remarks>
        public void LoadSound(string contentName, string alias)
        {
            SoundEffect soundEffect = Game.Content.Load<SoundEffect>(soundAssetLocation + contentName); 

            if (!soundBank.ContainsKey(alias))
            {
                soundBank.Add(alias, soundEffect);
            }
        }

        /// <summary>
        /// Loads a single song into the sound manager, giving it a specified alias.
        /// </summary>
        /// <param name="contentName">The content name of the sound file containing the song. Assumes all sounds are 
        /// located under the "Sounds" folder in the content project.</param>
        /// <param name="alias">Alias to give the song. This will be used to identify the song uniquely.</param>
        /// /// <remarks>Loading a song with an alias that is already used will have no effect.</remarks>
        public void LoadSong(string contentName, string alias)
        {
            Song song = Game.Content.Load<Song>(soundAssetLocation + contentName);

            if (!musicBank.ContainsKey(alias))
            {
                musicBank.Add(alias, song);
            }
        }

        /// <summary>
        /// Loads and organizes the sounds used by the game.
        /// </summary>
        public void LoadSounds()
        {
            LoadSound("ButtonClick", "ButtonClick");
            LoadSound("flop", "flop");
            LoadSound("yeah", "yeah");
            LoadSound("ooh", "ooh");
            LoadSound("drop", "drop");
            LoadSound("StarCollected", "StarCollected");
        }

        /// <summary>
        /// Loads and organizes the music used by the game.
        /// </summary>
        public void LoadMusic()
        {
            LoadSong("beach party", "beach party");
        }

        #endregion

        #region Sound Methods


        /// <summary>
        /// Indexer. Return a sound instance by name.
        /// </summary>
        public SoundEffect this[string soundName]
        {
            get
            {
                if (soundBank.ContainsKey(soundName))
                {
                    return soundBank[soundName];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        public static void PlaySound(string soundName, float volume)
        {
            if (soundBank.ContainsKey(soundName))
            {
                SoundEffect sound = soundBank[soundName];
                sound.Play(SoundVolume);
            }
        }

        public static void PlaySound(string soundName)
        {
            PlaySound(soundName, SoundVolume);
        }
      
        /// <summary>
        /// Play music by name. This stops the currently playing music first. Music will loop until stopped.
        /// </summary>
        /// <param name="musicSoundName">The name of the music sound.</param>
        /// <remarks>If the desired music is not in the music bank, nothing will happen.</remarks>
        public static void PlayMusic(string musicSoundName)
        {
            // If the music sound exists
            if (musicBank.ContainsKey(musicSoundName))
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(musicBank[musicSoundName]);
            }
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }

        #endregion
    }
}
