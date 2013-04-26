using System;

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
}


