using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoveAbility : MoveAbility
{
    public BasicMoveAbility() 
        : base(
            "Move",
            AbilityId.BasicMove,
            "move",
            1)
    {
    }

    public override PathfindResult GetDestinations(Map map, Actor actor)
    {
        return MoveHelper.Djikstra(
            p => map.ContainsActorAtLocation(p) ? int.MaxValue : map[p].Properties.MovementPenalty,
            p => GetAdjacentCoordinates(map, p),
            actor.GridPosition,
            actor.Properties.MovementPoints);
    }
}