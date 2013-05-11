using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Map : FContainer
{
    #region Private Properties

    private float halfTileSize;
    private List<Actor> actors;

    #endregion

    #region Properties

    public IEnumerable<Actor> Actors
    {
        get
        {
            return this.actors;
        }
    }

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
            return this.Columns * AwayTeam.TileSize;
        }
    }

    // Actual height of the map in points
    public float Height
    {
        get
        {
            return this.Rows * AwayTeam.TileSize;
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

    public Map(TileProperties[,] tiles)
    {
        this.Initialize(tiles);
    }

    private void Initialize(TileProperties[,] tileProperties)
    {
        this.halfTileSize = AwayTeam.TileSize / 2f;
        this.Columns = tileProperties.GetUpperBound(0) + 1;
        this.Rows = tileProperties.GetUpperBound(1) + 1;

        this.Tiles = new Tile[this.Columns, this.Rows];
        for (int ii = 0; ii < this.Columns; ii++)
        {
            for (int jj = 0; jj < this.Rows; jj++)
            {
                var tile = new Tile(tileProperties[ii,jj]);
                tile.x = ii * AwayTeam.TileSize + this.halfTileSize;
                tile.y = jj * AwayTeam.TileSize + this.halfTileSize;
                tile.width = tile.height = AwayTeam.TileSize;
                this.Tiles[ii, jj] = tile;
            }
        }

        this.actors = new List<Actor>();
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
        var gridX = (int)((mapCoords.x / AwayTeam.TileSize) + 1) - 1;
        var gridY = (int)((mapCoords.y / AwayTeam.TileSize) + 1) - 1;
        return new Vector2i(gridX, gridY);
    }

    public Vector2 GridToGlobal(Vector2i grid)
    {
        var globalx = grid.X * AwayTeam.TileSize + this.halfTileSize;
        var globaly = grid.Y * AwayTeam.TileSize + this.halfTileSize;
        return new Vector2(globalx, globaly);
    }

    public void Start()
    {
        // Why do this?  Because Futile likes sprite with the same image to be loaded at the same time
        foreach (var group in this.YieldTiles().GroupBy(tile => tile.Properties.SpriteName))
        {
            foreach (var item in group)
            {
                this.AddChild(item);
            }
        }
        
        foreach (var actor in this.Actors)
        {
            AddChild(actor);
        }
    }

    public bool Contains(Vector2i c)
    {
        return (c.Y >= 0 && c.Y < this.Rows) && (c.X >= 0 && c.X < this.Columns);
    }

    #endregion

    #region Actor Management

    public bool TryGetActor(Vector2i location, out Actor actor)
    {
        actor = this.actors.FirstOrDefault(x => x.GridPosition == location);
        return actor != null;
    }

    public bool ContainsActorAtLocation(Vector2i location)
    {
        return this.actors.Any(x => x.GridPosition == location);
    }

    public void AddActor(Actor actor, Vector2i location)
    {
        actor.x = AwayTeam.TileSize * location.X + this.halfTileSize;
        actor.y = AwayTeam.TileSize * location.Y + this.halfTileSize;
        actor.width = actor.height = AwayTeam.TileSize;
        this.actors.Add(actor);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Generates a sequence containing all the tiles in the map.
    /// Makes it easier to iterate over all the tiles (can use one
    /// foreach loop instead of nested for loops).
    /// </summary>
    /// <returns></returns>
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

    #endregion
}

