using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PushBlock.Screens;

namespace PushBlock
{
    class Sprite
    {
        public Texture2D Texture { get; set; }
                
        public Vector2 Position { get; set; }
        
        public int Width { get { return Texture.Width; } }

        public int Height { get { return Texture.Height; } }

        public Color Color { get; set; }
        
        public float Scale { get; set; }

        public float Rotation { get; set; }

        #region Construction

        public Sprite(Texture2D texture, Vector2 position, float scale, float rotation)
        {
            this.Texture = texture;
            this.Position = position;
            this.Scale = scale;
            this.Rotation = rotation;
            this.Color = Color.White;
        }

        public Sprite(Texture2D texture, Vector2 position, float scale) : this(texture, position, scale, 0) { }

        public Sprite(Texture2D texture, Vector2 position) : this(texture, position, 1) { }

        public Sprite() : this(null, Vector2.Zero, 1, 0) { }

        #endregion

        public bool Intersects(Rectangle rectangle)
        {
            return new Rectangle((int)(Position.X - Width/2f), (int)(Position.Y - Height/2f), Width, Height).Intersects(rectangle);
        }

        public bool Contains(Vector2 point)
        {
            return Intersects(new Rectangle((int)point.X, (int)point.Y, 1, 1));
        }

        #region Update

        public virtual void Update(  GameTime gameTime)
        {
           
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color, 
                            Rotation, new Vector2(Width / 2f , Height / 2f ), 
                            Scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
