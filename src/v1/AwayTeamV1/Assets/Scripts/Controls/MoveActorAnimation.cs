using System.Collections.Generic;
using UnityEngine;

class MoveActorAnimation : AnimationCanvas
{
    private Vector2i dest;
    private Actor actor;
    private List<Vector2> path;
    private float velocity = 4f;
    private bool done;
    private int index;

    public MoveActorAnimation(Actor mover, Vector2i destination, PathfindResult pathMetrics)
    {
        this.actor = mover;
        this.dest = destination;
        this.done = false;
        this.path = new List<Vector2>();
       
        Vector2i p = destination;
        Vector2i origin = actor.GridPosition;
        while (p != origin)
        {
            var globalx = p.X * AwayTeam.TileSize + AwayTeam.TileSize / 2;
            var globaly = p.Y * AwayTeam.TileSize + AwayTeam.TileSize / 2;
            this.path.Insert(0, new Vector2(globalx, globaly));
            p = pathMetrics.Previous[p];
        }
        this.index = 0;
    }

    public override bool AnimationComplete()
    {
        return this.done;
    }

    public override void PrepareFrame()
    {
        var currentDestination = this.path[this.index];
        var diffX = currentDestination.x - this.actor.x;
        var diffY = currentDestination.y - this.actor.y;

        if (diffX != 0)
        {
            actor.x += (Mathf.Abs(diffX) >= velocity) ? Mathf.Sign(diffX) * this.velocity : diffX;
        }
        else if (diffY != 0)
        {
            actor.y += (Mathf.Abs(diffY) >= velocity) ? Mathf.Sign(diffY) * this.velocity : diffY;
        }
        else
        {
            index++;
            this.done = this.index >= this.path.Count;
        }

        base.PrepareFrame();
    }
}