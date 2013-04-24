using UnityEngine;
using System;

public class MainLoop : MonoBehaviour
{
    private Map map;

    // Use this for initialization
    void Start()
    {
        FutileParams futileParams =
            new FutileParams(true, true, true, true);

        futileParams.AddResolutionLevel(
            1024, 1, 1, "");

        futileParams.origin = new Vector2(
            0f, 0f);
        Futile.instance.Init(futileParams);
        Futile.atlasManager.LoadImage("grasstile");
        Futile.atlasManager.LoadImage("foresttile");
        Futile.atlasManager.LoadImage("soldier");
        Futile.atlasManager.LoadImage("bluehighlight");
        var tileSize = 30;
        var width = 12;
        var height = 12;
        var tiles = new TileProperties[width, height];

        var grassTile = new TileProperties()
        {
            SpriteName = "grasstile",
            MovementPenalty = 1
        };

        var forestTile = new TileProperties()
        {
            SpriteName = "foresttile",
            MovementPenalty = 2
        };

        var actorProps = new ActorProperties()
        {
            SpriteName = "soldier",
            Name = "Hunkfort",
            MovementPoints = 5
        };

        var rand = new System.Random();
        for (int ii = 0; ii < width; ii++)
        {
            for (int jj = 0; jj < height; jj++)
            {
                tiles[ii, jj] = (rand.NextDouble() < 0.7) ? grassTile : forestTile;
            }
        }

        this.map = new Map(tiles, tileSize);
        this.map.AddActor(actorProps, new Vector2i(5, 5));
        this.map.InitializeStage();
        var result = Pathfinder.Pathfind(map, map.GetActor(new Vector2i(5, 5)));
        foreach (var item in result.Distance.Keys)
        {
            FSprite highlight = new FSprite("bluehighlight");
            highlight.x = item.X * tileSize + tileSize / 2;
            highlight.y = item.Y * tileSize + tileSize / 2;
            highlight.width = highlight.height = tileSize;
            Futile.stage.AddChild(highlight);
        }
    }
 
    // Update is called once per frame
    void Update()
	{
        if(map != null)
        this.map.Update();
    }
}