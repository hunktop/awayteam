using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : FContainer
{
    #region Private Properties

    private Dictionary<Actor, Vector2i> actorToPoint;
    private Dictionary<Vector2i, Actor> pointToActor;
    private int tileSize;
    private float halfTileSize;

    #endregion

    #region Properties

    public Tile[,] Tiles
    {
        get;
        private set;
    }

    // Actual width of the map in points
    public float Width
    {
        get
        {
            return this.Columns * this.tileSize;
        }
    }

    // Actual height of the map in points
    public float Height
    {
        get
        {
            return this.Rows * this.tileSize;
        }
    }

    // Number of rows in the map grid
    public int Rows
    {
        get;
        private set;
    }

    // Number of columns in the map grid
    public int Columns
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
        this.Columns = tileProperties.GetUpperBound(0) + 1;
        this.Rows = tileProperties.GetUpperBound(1) + 1;
        this.actorToPoint = new Dictionary<Actor, Vector2i>();
        this.pointToActor = new Dictionary<Vector2i, Actor>();

        this.Tiles = new Tile[this.Columns, this.Rows];
        for (int ii = 0; ii < this.Columns; ii++)
        {
            for (int jj = 0; jj < this.Rows; jj++)
            {
                var tile = new Tile(tileProperties[ii,jj]);
                tile.x = ii * this.tileSize + this.tileSize / 2;
                tile.y = jj * this.tileSize + this.tileSize / 2;
                tile.width = tile.height = this.tileSize;
                this.Tiles[ii, jj] = tile;
            }
        }
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

    public Vector2i GlobalToGrid(Vector2 point)
    {
        var mapCoords = this.GlobalToLocal(point);
        var gridX = (int)((mapCoords.x / this.tileSize) + 1) - 1;
        var gridY = (int)((mapCoords.y / this.tileSize) + 1) - 1;
        return new Vector2i(gridX, gridY);
    }

    public void Start()
    {
        foreach (var group in this.YieldTiles().GroupBy(tile => tile.Properties.SpriteName))
        {
            foreach (var item in group)
            {
                AddChild(item);
            }
        }

        foreach (var actor in this.actorToPoint.Keys)
        {
            AddChild(actor);
        }
    }

    public bool Contains(Vector2i c)
    {
        return (c.Y >= 0 && c.Y < this.Rows) && (c.X >= 0 && c.X < this.Columns);
    }
    
    public bool TryGetLocation(Actor actor, out Vector2i location)
    {
        return this.actorToPoint.TryGetValue(actor, out location);
    }

    #endregion

    #region Actor Management

    public bool TryGetActor(Vector2i location, out Actor actor)
    {
        return this.pointToActor.TryGetValue(location, out actor);
    }

    public void AddActor(ActorProperties actorProperties, Vector2i location, bool isEnemy = false)
    {
        var actor = new Actor(actorProperties);
        actor.TurnState = ActorState.TurnStart;
        actor.IsEnemy = isEnemy;
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

    public void UpdateLocation(Actor actor, Vector2i location)
    {
        var prevLocation = this.actorToPoint[actor];
        this.actorToPoint[actor] = location;
        this.pointToActor.Remove(prevLocation);
        this.pointToActor[location] = actor;
        actor.x = this.tileSize * location.X + this.halfTileSize;
        actor.y = this.tileSize * location.Y + this.halfTileSize;
    }

    #endregion

    #region Private Methods

    private IEnumerable<Tile> YieldTiles()
    {
        for (int ii = 0; ii < this.Columns; ii++)
        {
            for (int jj = 0; jj < this.Rows; jj++)
            {
                yield return this.Tiles[ii, jj];
            }
        }
    }

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
}

