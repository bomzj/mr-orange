using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PushBlock.Sky
{
    class Cloud : Sprite
    {
        const float MaxSpeed = 0.3f;

        Vector2 velocity;

        public SkyManager SkyManager { get; set; }

        public Cloud(Texture2D texture, Vector2 position, Random rand) : base(texture, position)
        {
            double velocityX = 0.4f * 60;
            velocity = new Vector2((float)velocityX, 0);
        }

        public override void Update(GameTime gameTime)
        {
            Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
