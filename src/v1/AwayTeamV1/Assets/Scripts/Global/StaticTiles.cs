public class StaticTiles
{
    public static TileProperties GrassTile = new TileProperties()
    {
        SpriteName = "grasstile",
        MovementPenalty = 1
    };

    public static TileProperties ForestTile = new TileProperties()
    {
        SpriteName = "foresttile",
        MovementPenalty = 2
    };
	
	public static TileProperties WallTile = new TileProperties()
	{
		SpriteName = "walltile",
		MovementPenalty = 10000000 // how do we handle infinity?
	};
	
	public static TileProperties FloorTile = new TileProperties()
	{
		SpriteName = "floortile",
		MovementPenalty = 1
	};
}
