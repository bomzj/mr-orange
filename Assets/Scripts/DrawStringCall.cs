using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class DrawStringCall
{
    private SpriteFont font;
    private string value;
    private Vector2 position;
    private Vector4 color;
    private float scale;
    private bool isCentered;

	public SpriteFont Font {
		get {
			return this.font;
		}
	}

	public Vector2 Position {
		get {
			return this.position;
		}
        set { this.position = value; }
	}

	public string Value {
		get {
			return this.value;
		}
	}

	public Vector4 Color {
		get {
			return this.color;
		}
	}

    public float Scale
    {
        get
        {
            return this.scale;
        }
        set
        {
            this.scale = value;
        }
    }

    public bool IsCentered
    {
        get { return isCentered; }
    }

    public DrawStringCall(SpriteFont font, string value, Vector2 position, Vector4 color)
        : this(font, value, position, color, 1, true)
    {
    }

    public DrawStringCall(SpriteFont font, string value, Vector2 position, Vector4 color, float scale)
        : this(font, value, position, color, scale, true)
    {
    }

    public DrawStringCall(SpriteFont font, string value, Vector2 position, Vector4 color, float scale, bool isCentered)
    {
        this.font = font;
        this.value = value;
        this.position = position;
        this.color = color;
        this.scale = scale;
        this.isCentered = isCentered;
    }
}