using System;
using System.Collections.Generic;

class PathfindResult
{
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

    public PathfindResult()
    {
        this.Distance = new Dictionary<Vector2i, int>();
        this.Previous = new Dictionary<Vector2i, Vector2i>();
    }
}

