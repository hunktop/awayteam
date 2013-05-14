using UnityEngine;
public class AwayTeam
{
    public static int MapScrollSpeed = 5;

    // When I move my mouse to the edge of the screen, how close do 
    // I need to get(in pixels) before the map scrolls?
    public static int MapScrollMargin = 50;

    // Pixel width of tiles 
    public static int TileSize = 40;

    public static MissionScene MissionController;

    public static Vector2 GridToGlobal(Vector2i grid)
    {
        var globalx = grid.X * AwayTeam.TileSize + AwayTeam.TileSize / 2;
        var globaly = grid.Y * AwayTeam.TileSize + AwayTeam.TileSize / 2;
        return new Vector2(globalx, globaly);
    }
}
