using System;
using System.Collections.Generic;
using System.Linq;

public class PathfindResult
{
    private HashSet<Vector2i> visitablePoints;

    public Dictionary<Vector2i, int> Distance
    {
        get;
        private set;
    }

    public Dictionary<Vector2i, Vector2i> Previous
    {
        get;
        private set;
    }

    public Vector2i Origin
    {
        get;
        private set;
    }

    public HashSet<Vector2i> VisitablePoints
    {
        get
        {
            if (this.visitablePoints == null)
            {
                var items = this.Distance.Keys.Except(new List<Vector2i>() { this.Origin });
                this.visitablePoints = new HashSet<Vector2i>(items);
            }
            return this.visitablePoints;
        }
    }

    public PathfindResult(Vector2i origin)
    {
        this.Origin = origin;
        this.Distance = new Dictionary<Vector2i, int>();
        this.Previous = new Dictionary<Vector2i, Vector2i>();
    }
}

