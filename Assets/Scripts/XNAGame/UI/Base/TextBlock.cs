using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using PushBlock.Screens;
using Microsoft.Xna.Framework.Input;

namespace PushBlock.UI
{
    class TextBlock 
    {
        public string Text { get; set; }

        public List<string> MultilineText { get; private set; }

        public SpriteFont Font { get; set; }

        public Color FontColor { get; set; }

        public float FontScale { get; set; }
        
        public float LineSpacing { get; set; }

        public int Width
        {
            get { return (int)Font.MeasureString(Text).X; }
        }

        public int Height
        {
            get { return (int)Font.MeasureString(Text).Y; }
        }

        // Origin of text
        public Vector2 Position { get; set; }
                
        public bool IsCentered { get; set; }

        public float Rotation { get; set; }

        public TextBlock()
        {
            FontColor = Color.White;
            FontScale = 1;
            IsCentered = true;

            MultilineText = new List<string>();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                spriteBatch.DrawString(Font, Text, Position, FontColor, FontScale, IsCentered); 
            }   
            else if (MultilineText.Count > 0)
            { 
                float height = Font.FontHeight * FontScale;
                float yOffset = (height / 2f);
                float yPosition = Position.Y - (MultilineText.Count - 1) * (yOffset + LineSpacing /2) ;
                
                foreach (var textLine in MultilineText)
                {
                    spriteBatch.DrawString(Font, textLine, new Vector2(Position.X, yPosition), FontColor, FontScale);
                    yPosition += yOffset + LineSpacing;
                }
            }
        }

    }
}
