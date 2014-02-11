using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PushBlock.Screens;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using PushBlock.Helpers;

namespace PushBlock.UI
{
    class LevelButton : Button
    {
        public Texture2D GoldStarTexture;
        public Texture2D GrayStarTexture;

        public int GoldStarsCount;

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();

            // Draw gold stars
            Vector2 starPosition = Position + new Vector2(-13, +35);

            for (int i = 0; i < 3; i++)
            {
                Texture2D starTexture = GoldStarsCount > i ? GoldStarTexture : GrayStarTexture;

                spriteBatch.Draw(starTexture, starPosition, null, Color, 0f,
                       new Vector2(starTexture.Width / 2f, starTexture.Height / 2f), 1f, SpriteEffects.None, 0f);
                
                starPosition += new Vector2(13, 0);
            }
            
            spriteBatch.End();
        }


    }
}
