using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AttackAbility : Ability
{
    public AttackAbility(string name, uint id, string iconname, int cooldown)
        : base(
            name,
            id,
            AbilityCategory.Attack,
            iconname,
            cooldown,
            false)
    {
    }

    public abstract IEnumerable<Vector2i> GetAttackableLocations(Map map, Actor actor);
}