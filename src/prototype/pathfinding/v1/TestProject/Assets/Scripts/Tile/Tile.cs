using System;
using UnityEngine;

public class Tile : FSprite
{
    public TileProperties Properties
    {
        get;
        private set;
    }

	public Tile(TileProperties tileProperties)
        : base(tileProperties == null ? "unknown" : tileProperties.SpriteName)
	{
        this.Properties = tileProperties;
	}
	
	public bool Contains(Vector2 c)
    {
        return (c.y >= this.y && c.y < this.y + this.height) && (c.x >= this.x && c.x < this.x + this.width);
    }
}


