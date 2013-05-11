using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MovementArgs
{
    public PathfindResult PrecomputedPathfinding
    {
        get;
        set;
    }

    public Vector2i Destination
    {
        get;
        set;
    }

    public Actor Actor
    {
        get;
        set;
    }

    public Map Map
    {
        get;
        set;
    }
}

public abstract class MoveAbility : Ability
{
    public MoveAbility(string name, uint id, string iconname, int cooldown) 
        : base(
            name, 
            id,
            AbilityCategory.Movement,
            iconname,
            cooldown, 
            false)
    {
    }

    public abstract PathfindResult GetDestinations(Map map, Actor actor);

    protected override void ExecuteImpl(object args)
    {
        var movementArgs = this.ConvertArgs<MovementArgs>(args);
        var actor = movementArgs.Actor;
        var map = movementArgs.Map;
        var destination = movementArgs.Destination;
        var pathfinding = movementArgs.PrecomputedPathfinding ?? this.GetDestinations(map, actor);
        this.moveActor(actor, destination, pathfinding);
    }

    protected IEnumerable<Vector2i> GetAdjacentCoordinates(Map map, Vector2i coord)
    {
        var x = coord.X;
        var y = coord.Y;

        if (x - 1 >= 0) yield return new Vector2i(x - 1, y);
        if (x + 1 < map.Columns) yield return new Vector2i(x + 1, y);
        if (y - 1 >= 0) yield return new Vector2i(x, y - 1);
        if (y + 1 < map.Rows) yield return new Vector2i(x, y + 1);
    }


    private void moveActor(Actor actor, Vector2i destination, PathfindResult pathfinding)
    {
        MoveActorAnimation move = new MoveActorAnimation(actor, destination, pathfinding);
        move.Delay = 10;
        move.AnimationStopped += new EventHandler<AnimationEventArgs>(anim_AnimationStopped);
        move.Start();
        AwayTeam.MissionController.AddChild(move);
        move.Play();
    }

    private void anim_AnimationStopped(object sender, AnimationEventArgs args)
    {
        if (args.StoppedReason == StoppedReason.Complete)
        {
            AwayTeam.MissionController.RemoveChild(sender as FNode);
            this.AfterAbilityExecute(true);
        }
    }
}
