using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagementSample;
using GameStateManagement;
using PushBlock.UI;
using System.Diagnostics;
using System.Threading;
using PushBlock.Helpers;

namespace PushBlock
{
    class Button
    {
        public Vector2 Position { get; set; }
        public Texture2D Image { get; set; }
        public string ClickSoundContent { get; set; }

        // Text label
        TextBlock textBlock;
        public SpriteFont Font { get { return textBlock.Font; } set { textBlock.Font = value; } }
        public float FontScale { get { return textBlock.FontScale; } set { textBlock.FontScale = value; } }
        public string Text { get { return textBlock.Text; } set { textBlock.Text = value; } }

        public Color Color = Color.White;

        public float scale = 1f;
        #if WINDOWS_PHONE
        public TimeSpan TimeToShowPressedImage = new TimeSpan(0,0,0,0,000);
        #else
        public TimeSpan TimeToShowPressedImage = new TimeSpan(0, 0, 0, 0, 000);
        #endif
        
        public TimeSpan TimeTillFireClickedEvent = TimeSpan.Zero;
         
        public int Width
        {
            get 
            {
                int width = 0;
                if (Image != null) width = (int)(Image.Width * scale);
                else if (textBlock.Font != null && !string.IsNullOrEmpty(Text)) width = (int)(textBlock.Font.MeasureString(Text).X * textBlock.FontScale);
                return width;
            }
        }

        public int Height
        {
            get
            {
                int height = 0;
                if (Image != null) height = (int)(Image.Height* scale);
                else if (textBlock.Font != null && !string.IsNullOrEmpty(Text)) height = (int)(textBlock.Font.MeasureString(Text).Y * textBlock.FontScale);
                return height;
            }
        }

        protected enum ButtonState { Normal, Mouseover, Pressed, Clicked }
        protected ButtonState buttonState;

        public event Action Clicked;

        public Button() 
        {
            textBlock = new TextBlock();
        }

        public void Scale(float value)
        {
            scale = value;
            textBlock.FontScale = value;
        }

        bool Intersects(int x, int y)
        {
            Rectangle buttonRectangle = new Rectangle((int)(Position.X - Width / 2),
                                (int)(Position.Y - Height / 2), Width, Height);
            return buttonRectangle.Intersects(new Rectangle(x,y,1,1));
        }

        public virtual void HandleInput(GameTime gameTime, InputState input)
        {
            if (Intersects((int)input.CurrentMouseState.X, (int)input.CurrentMouseState.Y))
            {
                if (buttonState == ButtonState.Pressed &&
                    input.CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
                    input.LastMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    buttonState = ButtonState.Clicked;
                    TimeTillFireClickedEvent = TimeToShowPressedImage;
                    AudioManager.PlaySound("ButtonClick");
                }
                else if (buttonState != ButtonState.Pressed && buttonState != ButtonState.Clicked)
                {
                    if (input.IsNewMouseLeftButtonDown())
                    {
                        buttonState = ButtonState.Pressed;
                    }
                    else
                    {
                        buttonState = ButtonState.Mouseover;
                    }
                }
            }
            else if (buttonState != ButtonState.Clicked)
            {
                buttonState = ButtonState.Normal;
            }

            // Handle clicked timeout state
            if (buttonState == ButtonState.Clicked)
            {
                TimeTillFireClickedEvent -= gameTime.ElapsedGameTime;
                if (TimeTillFireClickedEvent.TotalMilliseconds < 0)
                {
                    if (Clicked != null) Clicked();
                    buttonState = Intersects((int)input.CurrentMouseState.X, (int)input.CurrentMouseState.Y) ?
                        ButtonState.Mouseover : ButtonState.Normal;
                }
            }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color;
            if (buttonState != ButtonState.Normal && buttonState != ButtonState.Mouseover)
            {
                color = new Color(0.85f, 0.85f, 0.85f);
            }

            if (Image != null)
            {
                spriteBatch.Draw(Image, Position, null, color, 0f,
                            new Vector2(Image.Width / 2f, Image.Height / 2f), scale, SpriteEffects.None, 0f);
            }

            if (textBlock.Font != null && !string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(textBlock.Font, Text, Position, color);
            }
        }
    }
}
