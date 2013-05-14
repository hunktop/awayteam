using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class StaticWeapons
{
    public static RangedWeapon Rifle = new RangedWeapon()
    {
        Name = "Rifle",
        Accuracy = 70,
        Damage = 4,
        MinRange = 1,
        MaxRange = 6
    };

    static StaticWeapons()
    {
        Rifle.Abilities.Add(new RangedAttackAbility());
    }
}
