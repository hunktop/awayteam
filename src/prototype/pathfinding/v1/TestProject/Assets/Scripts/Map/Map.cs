using System.Collections.Generic;
using UnityEngine;

class Map : IHandleMouseEvents
{
    #region Private Properties

    private Dictionary<Actor, Vector2i> actorToPoint;
    private Dictionary<Vector2i, Actor> pointToActor;
    private int tileSize;
    private float halfTileSize;
    private int[,] dist;
    private Vector2i[,] previous;
    private HashSet<Vector2i> visitable;
    private MouseManager mouseManager;
	private TileInterface tileInterface;
    
    #endregion

    #region Properties

    public Tile[,] Tiles
    {
        get;
        private set;
    }

    public int Height
    {
        get;
        private set;
    }

    public int Width
    {
        get;
        private set;
    }

    #endregion

    #region Ctor

    public Map(TileProperties[,] tiles, int size)
    {
        this.Initialize(tiles, size);
    }

    private void Initialize(TileProperties[,] tileProperties, int size)
    {
        this.tileSize = size;
        this.halfTileSize = this.tileSize / 2f;
        this.Width = tileProperties.GetUpperBound(0) + 1;
        this.Height = tileProperties.GetUpperBound(1) + 1;
        this.actorToPoint = new Dictionary<Actor, Vector2i>();
        this.pointToActor = new Dictionary<Vector2i, Actor>();

        this.Tiles = new Tile[this.Width, this.Height];
        for (int ii = 0; ii < this.Width; ii++)
        {
            for (int jj = 0; jj < this.Height; jj++)
            {
                var tile = new Tile(tileProperties[ii,jj]);
                tile.x = ii * this.tileSize + this.tileSize / 2;
                tile.y = jj * this.tileSize + this.tileSize / 2;
                tile.width = tile.height = this.tileSize;
                this.Tiles[ii, jj] = tile;
            }
        }

        this.mouseManager = new MouseManager(this);
		
		this.tileInterface = new TileInterface(this);
    }

    #endregion

    #region Operator Overloads

    public Tile this[int x, int y]
    {
        get
        {
            return this.Tiles[x, y];
        }
    }

    public Tile this[Vector2i vec]
    {
        get
        {
            return this.Tiles[vec.X, vec.Y];
        }
    }


    #endregion

    #region Public Methods

    public void InitializeStage()
    {
        for (int ii = 0; ii < this.Width; ii++)
        {
            for (int jj = 0; jj < this.Height; jj++)
            {
                Futile.stage.AddChild(this.Tiles[ii, jj]);
            }
        }

        foreach (var actor in this.actorToPoint.Keys)
        {
            Futile.stage.AddChild(actor);
        }
    }

    public void Update(float dt)
    {
        this.mouseManager.Update(dt);
		this.tileInterface.Update(dt);
    }

    public bool Contains(Vector2i c)
    {
        return (c.Y >= 0 && c.Y < this.Height) && (c.X >= 0 && c.X < this.Width);
    }
    
    public Vector2i? GetLocation(Actor actor)
    {
        Vector2i location;
        if (this.actorToPoint.TryGetValue(actor, out location))
        {
            return location;
        }
        return null;
    }

    #endregion

    #region Actor Management

    public Actor GetActor(Vector2i location)
    {
        Actor actor;
        if (this.pointToActor.TryGetValue(location, out actor))
        {
            return actor;
        }
        return null;
    }

    public void AddActor(ActorProperties actorProperties, Vector2i location)
    {
        var actor = new Actor(actorProperties);
        actor.x = this.tileSize * location.X + this.halfTileSize;
        actor.y = this.tileSize * location.Y + this.halfTileSize;
        actor.width = actor.height = this.tileSize;
        this.actorToPoint.Add(actor, location);
        this.pointToActor.Add(location, actor);
    }

    //public void RemoveActor(Actor actor)
    //{
    //    var location = this.actorToPoint[actor];
    //    this.actorToPoint.Remove(actor);
    //    this.pointToActor.Remove(location);
    //}

    //public void UpdateLocation(Actor actor, Vector2i location)
    //{
    //    var prevLocation = this.actorToPoint[actor];
    //    this.actorToPoint[actor] = location;
    //    this.pointToActor.Remove(prevLocation);
    //    this.pointToActor[location] = actor;
    //}

    #endregion

    #region Private Methods

    private IEnumerable<Vector2i> GetPointsAtDistance(Vector2i start, int dist)
    {
        int x = start.X;
        int y = start.Y;

        Vector2i p1 = new Vector2i(x - dist, y); yield return p1;
        Vector2i p2 = new Vector2i(x + dist, y); yield return p2;
        Vector2i p3 = new Vector2i(x, y - dist); yield return p3;
        Vector2i p4 = new Vector2i(x, y + dist); yield return p4;

        for (int ii = 0; ii < dist - 1; ii++)
        {
            p1.X++; p1.Y--; yield return p1;
            p2.X--; p2.Y++; yield return p2;
            p3.X++; p3.Y++; yield return p3;
            p4.X--; p4.Y--; yield return p4;
        }
    }

    #endregion

    public void MouseClicked(MouseEvent e)
    {
        Debug.Log("Mouse clicked: " + e.Location + " " + e.MouseButton.ToString());
    }

    public void MousePressed(MouseEvent e)
    {
        Debug.Log("Mouse Pressed: " + e.Location);
    }

    public void MouseReleased(MouseEvent e)
    {
        Debug.Log("Mouse Released: " + e.Location);
    }
}

