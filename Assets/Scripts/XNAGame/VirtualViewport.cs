using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.XNAGame
{
    class VirtualViewport
    {
        public int DesignWidth { get; private set; }
        
        public int DesignHeight { get; private set; }

        public float DesignAspectRatio 
        { 
            get
            {
                return (float)DesignWidth / DesignHeight;
            }
        }

        public int ScreenWidth { get { return Screen.width; } }

        public int ScreenHeight { get { return Screen.height; } }

        public float ScreenAspectRatio
        {
            get 
            {
                return (float)ScreenWidth / ScreenHeight;
            }
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public float Scale { get; private set; }

        private static VirtualViewport current;

        public static VirtualViewport Current { get { return current;  } }

        public VirtualViewport(int designWidth, int designHeight)
	    {
            DesignWidth = designWidth;
            DesignHeight = designHeight;
            current = this;
	    }

        public void Resize()
        {
            float targetaspect = DesignAspectRatio;

            // determine the game window's current aspect ratio
            float windowaspect = ScreenAspectRatio;

            // current viewport height should be scaled by this amount
            float scaleheight = windowaspect / targetaspect;

            // if scaled height is less than current height, add letterbox
            Rect rect = new Rect();
            if (scaleheight < 1.0f)
            {
                rect.width = ScreenWidth;
                rect.height = scaleheight * ScreenHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) * ScreenHeight / 2.0f;

                Scale = (float)ScreenWidth / DesignWidth;
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleheight;

                rect.width = scalewidth * ScreenWidth;
                rect.height = 1.0f * ScreenHeight;
                rect.x = (1.0f - scalewidth) * ScreenWidth / 2.0f;
                rect.y = 0;

                Scale = (float)ScreenHeight / DesignHeight;
            }

            X = (int)rect.x;
            Y = (int)rect.y;
            Width = (int)rect.width;
            Height = (int)rect.height;
        }

        public void AdjustDrawCallsForViewport(DrawQueue drawQueue)
        {
            foreach (var item in drawQueue.LastSpriteQueue)
            {
                var position = item.Position;

                if (!item.SkipViewportScaleTransformation)
                {
                    position *= Scale;
                    item.Scale *= Scale;
                }

                position.X += X;
                position.Y += Y;

                item.Position = position;
            }

            foreach (var item in drawQueue.LastStringQueue)
            {
                var position = item.Position;

                //if (!item.SkipViewportScaleTransformation)
                //{
                    position *= Scale;
                    item.Scale *= Scale;
                //}

                position.X += X;
                position.Y += Y;

                item.Position = position;
            } 
        }

        

    }
}
