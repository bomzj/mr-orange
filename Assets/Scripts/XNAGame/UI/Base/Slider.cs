using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using PushBlock.Helpers;
using GameStateManagement;
using Assets.Scripts.XNAEmulator.Input;


namespace PushBlock.UI
{
    class Slider
    {
        private Texture2D sliderTexture;
        private Vector2 sliderPosition; 
        
        private Texture2D sliderBarTexture;
        private Vector2 sliderBarPosition;

        Color color = Color.White;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public int Width { get { return sliderBarTexture.Width; } }
        public int Height { get { return sliderTexture.Height; } }
        
        public Vector2 Position
        {
            get { return sliderBarPosition; }
            set 
            {
                if (sliderBarPosition != value)
                {
                    int dx = (int)(sliderBarPosition.X - sliderPosition.X);
                    sliderBarPosition = value;
                    sliderPosition = sliderBarPosition - new Vector2(dx, 0);
                }
            }
        }

        // Get slider value between 0-1
        public float Value 
        {
            get 
            {
                return (sliderPosition.X - (sliderBarPosition.X - sliderBarTexture.Width / 2)) / sliderBarTexture.Width;
            }

            set
            {
                sliderPosition.X = MathHelper.Clamp(
                    sliderBarPosition.X - sliderBarTexture.Width / 2 + value * sliderBarTexture.Width,
                    sliderBarPosition.X - sliderBarTexture.Width / 2,
                    sliderBarPosition.X + sliderBarTexture.Width / 2);
            }
        }

        public event Action ValueChanged;

        public Slider(string barAsset, string sliderAsset, Vector2 position, ContentManager content)
        {
            sliderTexture = content.Load<Texture2D>(sliderAsset);
            sliderBarTexture = content.Load<Texture2D>(barAsset);
            sliderBarPosition = sliderPosition = position;
        }

        void OnValueChanged()
        {
            if (ValueChanged != null) ValueChanged();
        }

        public void HandleInput(InputState input)
        {
            MouseState currentMouseState = input.CurrentMouseState;
            MouseState lastMouseState = input.LastMouseState;

            Rectangle sliderBarRect = new Rectangle(
                (int)sliderBarPosition.X - sliderBarTexture.Width / 2,
                (int)sliderPosition.Y - sliderTexture.Height / 2,
                sliderBarTexture.Width, sliderTexture.Height);

            if (sliderBarRect.Contains((int)currentMouseState.X, (int)currentMouseState.Y) && input.IsMouseLeftButtonDown())
            {
                sliderPosition = new Vector2(currentMouseState.X, sliderBarPosition.Y);
                if (Value < 0.03f) Value = 0;
                OnValueChanged();
            }
        }

        public void Update(GameTime gameTime)
        {
          
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            
            spriteBatch.Draw(sliderBarTexture, sliderBarPosition, null, color, 0, 
                new Vector2(sliderBarTexture.Width / 2f, sliderBarTexture.Height / 2f), 1f, SpriteEffects.None, 0);
            
            spriteBatch.Draw(sliderTexture, sliderPosition, null, color, 0, 
                new Vector2(sliderTexture.Width / 2f, sliderTexture.Height / 2f), 1f, SpriteEffects.None, 0);
            
            spriteBatch.End();
        }

    }
}