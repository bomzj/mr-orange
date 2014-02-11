using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Xna.Framework.Graphics
{
    public class SpriteFont : IDisposable
    {
        public class SpriteFontChar
        {
            public int characterKey;
            public int x;
            public int y;
            public int width;
            public int height;

            public SpriteFontChar(int characterKey, int x, int y, int width, int height)
	        {
                this.characterKey = characterKey;
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
	        }
        }

        private SpriteFontChar[] chars;
        private Texture2D fontTexture;

        public Texture2D Texture { get { return fontTexture; } }

        public int FontHeight
        {
            get
            {
                return chars.First().height;
            }

        }

        public SpriteFont(Texture2D fontTexture, string fontDescriptorXmlContent)
        {
            InitializeFontDescriptor(fontDescriptorXmlContent);
            this.fontTexture = fontTexture;
        }

        private void InitializeFontDescriptor(string fontDescriptorXmlContent)
        {
            XDocument xdoc = XDocument.Parse(fontDescriptorXmlContent);

            List<SpriteFont.SpriteFontChar> chars = new List<SpriteFont.SpriteFontChar>();

            foreach (var item in xdoc.Element("fontMetrics").Elements("character"))
            {
                int key = int.Parse(item.Attribute("key").Value);
                int x = int.Parse(item.Element("x").Value);
                int y = int.Parse(item.Element("y").Value);
                int width = int.Parse(item.Element("width").Value);
                int height = int.Parse(item.Element("height").Value);
                SpriteFont.SpriteFontChar character = new SpriteFont.SpriteFontChar(key, x + 1, y + 1, width - 2, height - 2);
                chars.Add(character);
            }

            this.chars = chars.ToArray();
        }

        internal Vector2 MeasureString(string text)
        {			
            Vector2 size = Vector2.Zero;
            size.Y = chars.First().height;

            foreach (char c in text)
            {
                var spriteChar = chars.First(sfc => sfc.characterKey == (int)c);
                size.X += spriteChar.width;
            }

            return size;
        }

        public UnityEngine.Rect GetCharTextureCoords(char c)
        {
            var fontChar = GetSpriteFontChar(c);
            
            UnityEngine.Rect texCoords = new UnityEngine.Rect();
            texCoords.x = (float)fontChar.x / fontTexture.Width;
            texCoords.y = (float)(fontTexture.Height - (fontChar.y + fontChar.height)) / fontTexture.Height;
            texCoords.width = (float)fontChar.width / fontTexture.Width;
            texCoords.height = (float) fontChar.height / fontTexture.Height;

            return texCoords;
        }

        public SpriteFontChar GetSpriteFontChar(char c)
        {
            return chars.First(sfc => sfc.characterKey == (int)c);
        }

        public int GetCharWidth(char c)
        {
            return chars.First(sfc => sfc.characterKey == (int)c).width;
        }

        public void Dispose()
        { }
    }
}
