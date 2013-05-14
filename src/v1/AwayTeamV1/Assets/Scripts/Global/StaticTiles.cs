using System;
public class StaticTiles
{
    public static TileProperties GrassTile = new TileProperties()
    {
        SpriteName = "grasstile",
        MovementPenalty = 1,
        BlocksVision = false
    };

    public static TileProperties ForestTile = new TileProperties()
    {
        SpriteName = "foresttile",
        MovementPenalty = 2,
        BlocksVision = false
    };
	
	public static TileProperties WallTile = new TileProperties()
	{
		SpriteName = "walltile",
		MovementPenalty = Int32.MaxValue, // how do we handle infinity?
        BlocksVision = true
	};
	
	public static TileProperties FloorTile = new TileProperties()
	{
		SpriteName = "floortile",
		MovementPenalty = 1,
        BlocksVision = false
	};
}
