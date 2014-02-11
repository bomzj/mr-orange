#define ALT_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Xna.Framework.Content
{
	public class ContentManager
	{
		private IServiceProvider serviceProvider;
		private string p;

		public ContentManager(IServiceProvider serviceProvider, string p)
		{
			// TODO: Complete member initialization
			this.serviceProvider = serviceProvider;
			this.p = p;
		}
		internal T1 Load<T1>(string asset) where T1 : IDisposable
		{
			Type type = typeof(T1);
			
#if ALT_MODE	
			asset = Path.Combine("Content", asset);
			asset = asset.Replace("\\","/");
#else
			asset = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Content", asset));
			string filename;
			string directoryName = Path.GetDirectoryName(asset);
			string smallFilename = Path.GetFileName(asset);
			string[] fileResults = Directory.GetFiles(directoryName, smallFilename + "*");
			asset = fileResults[0];
#endif		
			// TODO: Do cache check

			if(type == typeof(Texture2D))
			{
				Texture2D texture = LoadTexture2D(asset);
				return (T1)Convert.ChangeType(texture, type);
			}
			else if (type == typeof(SpriteFont))
			{
				SpriteFont spriteFont = LoadSpriteFont(asset);
				return (T1)Convert.ChangeType(spriteFont, type);
			}
			else if (type == typeof(SoundEffect))
			{
                SoundEffect soundEffect = LoadSoundEffect(asset);
                return (T1)Convert.ChangeType(soundEffect, type);
			}
			else if (type == typeof(Song))
			{
                Song song = LoadSong(asset);
                return (T1)Convert.ChangeType(song, type);
			}
            else if (type == typeof(ParticlesSettings.ParticleSystemSettings))
            {
                ParticlesSettings.ParticleSystemSettings particleSettings = LoadParticleSystemSettings(asset);
                return (T1)Convert.ChangeType(particleSettings, type);
            }
			
			// TODO: Improve
			return default(T1);            
		}

		private Texture2D LoadTexture2D(string asset)
		{
			UnityEngine.Texture2D unityTexture = new UnityEngine.Texture2D(2, 2);
			
#if ALT_MODE		
            unityTexture = (UnityEngine.Texture2D)UnityEngine.Resources.Load(asset, typeof(UnityEngine.Texture2D));
#else		
			byte[] bytes = File.ReadAllBytes(asset);
			unityTexture.LoadImage(bytes);
#endif
			
			return new Texture2D(unityTexture);
		}
		

        private SpriteFont LoadSpriteFont(string asset)
        {
            Texture2D fontTexture = LoadTexture2D(asset);
            UnityEngine.TextAsset spriteFontText = (UnityEngine.TextAsset)UnityEngine.Resources.Load(asset, typeof(UnityEngine.TextAsset));
            SpriteFont spriteFont = new SpriteFont(fontTexture, spriteFontText.text);
            return spriteFont;
        }

        private SoundEffect LoadSoundEffect(string asset)
        {
            SoundEffect soundEffect = new SoundEffect();
            soundEffect.Clip = (UnityEngine.AudioClip)UnityEngine.Resources.Load(asset);

            return soundEffect;
        }

        private Song LoadSong(string asset)
        {
            Song song = new Song();
            song.Clip = (UnityEngine.AudioClip)UnityEngine.Resources.Load(asset);
            
            return song;
        }

        private ParticlesSettings.ParticleSystemSettings LoadParticleSystemSettings(string asset)
        {
            ParticlesSettings.ParticleSystemSettings settings = new ParticlesSettings.ParticleSystemSettings();
            
            // Hardcoded settings 
            if (asset.Contains("BlockExplosion"))
            {
                settings.MinNumParticles = 25;
                settings.MaxNumParticles = 35;
                settings.TextureFilename = "Particles/WhiteCircle";
                settings.MinInitialSpeed = 50;
                settings.MaxInitialSpeed = 180;
                settings.AccelerationMode = ParticlesSettings.AccelerationMode.None;
                settings.MinRotationSpeed = -45;
                settings.MaxRotationSpeed = 45;
                settings.MinLifetime = 0.6f;
                settings.MaxLifetime = 0.8f;
                settings.MinSize = 0.5f;
                settings.MaxSize = 1;
                settings.Gravity = new Vector2(0, 1500);
            }
            else if (asset.Contains("StarDust"))
            {
                settings.MinNumParticles = 30;
                settings.MaxNumParticles = 40;
                settings.TextureFilename = "other/smallStar";
                settings.MinInitialSpeed = 60;
                settings.MaxInitialSpeed = 210;
                settings.AccelerationMode = ParticlesSettings.AccelerationMode.EndVelocity;
                settings.EndVelocity = 0;
                settings.MinRotationSpeed = 0;
                settings.MaxRotationSpeed = 360;
                settings.MinLifetime = 0.6f;
                settings.MaxLifetime = 1;
                settings.MinSize = 0.35f;
                settings.MaxSize = 0.35f;
                settings.Gravity = new Vector2(0, 0);
            }

            return settings;
        }

		internal void Unload()
		{
			// TODO
		}

		public string RootDirectory { get; set; }
	}
}
