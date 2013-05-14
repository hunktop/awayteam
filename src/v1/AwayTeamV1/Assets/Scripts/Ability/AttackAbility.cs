using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class AttackArgs
{
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

    public Vector2i TargetedLocation
    {
        get;
        set;
    }
}

public abstract class AttackAbility : Ability
{
    public AttackAbility(string name, uint id, string iconname, int cooldown)
        : base(
            name,
            id,
            AbilityCategory.Attack,
            iconname,
            cooldown,
            true)
    {
    }

    public abstract IEnumerable<Vector2i> GetAttackableLocations(Map map, Actor actor, WeaponProperties weapon);
}