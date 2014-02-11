using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class DrawSpriteCall
{
    private Texture2D texture2D;
    private Vector2 position;
	private Nullable<Rectangle> source;
    private Vector4 color;
	private Vector2 origin;
	private SpriteEffects spriteEffects;
    private float rotation;
    private float scale;

	public Texture2D Texture2D {
		get {
			return this.texture2D;
		}
	}

	public Vector2 Position {
		get {
			return this.position;
		}
        set
        {
            this.position = value;
        }
	}

	public Nullable<Rectangle> Source {
		get {
			return this.source;
		}
	}

	public Vector4 Color {
		get {
			return this.color;
		}
	}
	
	public Vector2 Origin {
		get {
			return this.origin;
		}
	}
	
	public SpriteEffects SpriteEffects {
		get {
			return this.spriteEffects;
		}
	}

    public float Rotation {
        get {
            return rotation;
        }
    }

    public float Scale
    {
        get
        {
            return scale;
        }
        set
        {
            scale = value;
        }
    }

    public bool SkipViewportScaleTransformation { get; set; }

    public DrawSpriteCall(Texture2D texture2D, Vector2 position, Nullable<Rectangle> source, Vector4 color, Vector2 origin, SpriteEffects spriteEffects, float rotation, float scale)
    {
        // TODO: Complete member initialization
        this.texture2D = texture2D;
        this.position = position;
        this.source = source;
        this.color = color;
        this.origin = origin;
        this.spriteEffects = spriteEffects;
        this.rotation = rotation;
        this.scale = scale;
    }

    public DrawSpriteCall(Texture2D texture2D, Vector2 position, Nullable<Rectangle> source, Vector4 color, Vector2 origin, SpriteEffects spriteEffects, float rotation)
        : this(texture2D, position, source, color, origin, spriteEffects, rotation, 1)
    {
    }

    public DrawSpriteCall(Texture2D texture2D, Vector2 position, Nullable<Rectangle> source, Vector4 color, Vector2 origin, SpriteEffects spriteEffects) 
        :this(texture2D, position, source, color, origin, spriteEffects, 0)
    {
    }

    public DrawSpriteCall(Texture2D texture2D, Vector2 position, Nullable<Rectangle> source, Vector4 color, Vector2 origin, SpriteEffects spriteEffects, bool skipViewportScaleTransformation)
        : this(texture2D, position, source, color, origin, spriteEffects, 0)
    {
        SkipViewportScaleTransformation = skipViewportScaleTransformation;
    }
}
