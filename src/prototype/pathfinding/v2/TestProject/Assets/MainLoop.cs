using UnityEngine;
using System;

public class MainLoop : MonoBehaviour, IHandleMouseEvents
{
    private Map map;
    MouseManager manager;
    FStage tileStage;

    // Use this for initialization
    void Start()
    {
        FutileParams futileParams = new FutileParams(true, true, true, true);
        manager = new MouseManager(this);
        futileParams.AddResolutionLevel(1024, 1, 1, "");

        futileParams.origin = new Vector2(
            0.0f, 0.0f);
        Futile.instance.Init(futileParams);
        Futile.atlasManager.LoadImage("grasstile");
        Futile.atlasManager.LoadImage("foresttile");
        Futile.atlasManager.LoadImage("soldier");
        Futile.atlasManager.LoadImage("bluehighlight");
        var tileSize = 30;
        var width = 35;
        var height = 35;
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
        this.map.Start();

        tileStage = new FStage("test");
        Futile.AddStage(tileStage);
        this.map.x += 100;
        this.map.y += 100;

        tileStage.stage.AddChild(this.map);

        //var result = Pathfinder.Pathfind(map, map.GetActor(new Vector2i(5, 5)));
        //foreach (var item in result.Distance.Keys)
        //{
        //    FSprite highlight = new FSprite("bluehighlight");
        //    highlight.x = item.X * tileSize + tileSize / 2;
        //    highlight.y = item.Y * tileSize + tileSize / 2;
        //    highlight.width = highlight.height = tileSize;
        //    Futile.stage.AddChild(highlight);
        //}
    }
 
    // Update is called once per frame
    void Update()
	{
        if (Input.GetKey("w")) { tileStage.y-=3; }
        if (Input.GetKey("s")) { tileStage.y+=3; }
        if (Input.GetKey("a")) { tileStage.x+=3; }
        if (Input.GetKey("d")) { tileStage.x-=3; }
        Futile.stage.x++;
        Futile.stage.y++;
        this.manager.Update();
        this.map.Update();
    }

    public void MouseClicked(MouseEvent e)
    { 
        var globalToLocal = tileStage.GlobalToLocal(e.WorldCoordinates);
        var localToStage = tileStage.LocalToStage(globalToLocal);
        Debug.Log("Screen Coords " + e.ScreenCoordinates + " " 
            + "Global Coords " + e.WorldCoordinates + " "
            + "Global To Local" + globalToLocal + " "
            + "Local To Stage" + localToStage);
    }

    public void MousePressed(MouseEvent e)
    {
    }

    public void MouseReleased(MouseEvent e)
    {
    }
}