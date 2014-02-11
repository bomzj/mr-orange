using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PushBlock.Screens;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;

namespace PushBlock.Sky
{
    class SkyManager
    {
        const int maxCloudsOnScreen = 4;
        
        ContentManager content;
        List<Cloud> clouds = new List<Cloud>();

        Random random;

        public SkyManager(ContentManager content)
        {
            this.content = content;
            random = new Random(DateTime.Now.Millisecond);
        }

        public bool GenerateCloud(Rectangle area)
        {
            // Generate random cloud texture
            Texture2D texture = content.Load<Texture2D>(string.Format("Sky/cloud{0}", random.Next(4) + 1));

            int x = random.Next((int)(area.X + texture.Width / 2f), (int)(area.X + area.Width - texture.Width/2f));
            int y = random.Next((int)(area.Y + texture.Height / 2f), (int)(area.Y + area.Height - texture.Height / 2f));

            int cloudPaddingX = 100;
            int cloudPaddingY = 80;
            Rectangle cloudRect = new Rectangle((int)(x - (texture.Width + cloudPaddingX) / 2f),
                                                (int)(y - (texture.Height + cloudPaddingY) / 2f), 
                                                texture.Width + cloudPaddingX,
                                                texture.Height + cloudPaddingY);

            
                
            // Check whether new cloud intersects with other clouds
            bool isPositionEmpty = true;

            foreach (Cloud existedCloud in clouds)
            {
                if (cloudRect.Intersects(new Rectangle((int)(existedCloud.Position.X - existedCloud.Width / 2f),
                                                        (int)(existedCloud.Position.Y - existedCloud.Height / 2f),
                                                        existedCloud.Width, existedCloud.Height)))
                {
                    isPositionEmpty = false;
                    break;
                }

            }

            // If new cloud does not intersect others then add it to sky
            if (isPositionEmpty)
            {
                Cloud cloud = new Cloud(texture, new Vector2(x, y), random);
                cloud.SkyManager = this;

                clouds.Add(cloud);
            }

            return isPositionEmpty;
        }

        public void Update(  GameTime gameTime)
        {
            // Generate new clouds
            if (clouds.Count < maxCloudsOnScreen) 
                GenerateCloud(new Rectangle(-200, 0, 200, 180));

            // Update clouds position
            clouds.ForEach(c => c.Update(gameTime));

            int screenWidth = 800;
            List<Cloud> cloudsToRemove = clouds.ToList();
            
            // Remove clouds outside the right side of screen
            foreach (Cloud item in cloudsToRemove)
            {
                if (item.Position.X - item.Width / 2f > screenWidth) clouds.Remove(item);
            }

        }

        public void Draw(  GameTime gameTime, SpriteBatch spriteBatch)
        {
            clouds.ForEach(c => c.Draw(spriteBatch));
        }

    }
}
