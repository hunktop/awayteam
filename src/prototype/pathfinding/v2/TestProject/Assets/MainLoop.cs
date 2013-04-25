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
        futileParams.AddResolutionLevel(800, 1, 1, "");

        futileParams.origin = new Vector2(
            0.0f, 0.0f);
        Futile.instance.Init(futileParams);
        Futile.atlasManager.LoadImage("grasstile");
        Futile.atlasManager.LoadImage("foresttile");
        Futile.atlasManager.LoadImage("soldier");
        Futile.atlasManager.LoadImage("bluehighlight");
        var tileSize = 30;
        var width = 50;
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
        tileStage.stage.AddChild(this.map);
    }
 
    // Update is called once per frame
    void Update()
	{
        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var mX = mousePosition2d.x;
        var mY = mousePosition2d.y;
        var margin = GameConstants.MapScrollMargin;

        if ((mX >= 0 && mX < margin)|| Input.GetKey("a"))
        {
            if (tileStage.x < 0)
            {
                tileStage.x += GameConstants.MapScrollSpeed;
            }
        }

        if ((mX > (Futile.screen.width - margin) && mX <= Futile.screen.width)|| Input.GetKey("d"))
        {
            if (Math.Abs(tileStage.x) + Futile.screen.width < this.map.Width)
            {
                tileStage.x -= GameConstants.MapScrollSpeed;
            }
        }

        if ((mY > 0 && mY < margin) || Input.GetKey("s"))
        {
            if (tileStage.y < 0)
            {
                tileStage.y += GameConstants.MapScrollSpeed;
            }
        }

        if ((mY > (Futile.screen.height - margin) && mY < Futile.screen.height) || Input.GetKey("w"))
        {
            if (Math.Abs(tileStage.y) + Futile.screen.height < this.map.Height)
            {
                tileStage.y -= GameConstants.MapScrollSpeed;
            }
        }

        this.manager.Update();
        this.map.Update();
    }

    public void MouseClicked(MouseEvent e)
    { 
        var globalToLocal = tileStage.GlobalToLocal(e.GlobalCoordinates);
        var localToStage = tileStage.LocalToStage(globalToLocal);
        Debug.Log("Screen Coords " + e.ScreenCoordinates + " " 
            + "Global Coords " + e.GlobalCoordinates + " "
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