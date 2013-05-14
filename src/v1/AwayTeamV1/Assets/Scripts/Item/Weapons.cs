using System;

public class Weapons
{
	public static WeaponProperties AssaultRifle = new WeaponProperties()
	{
		WeaponName = "Assault Rifle",
		MinRange = 1,
		MaxRange = 10,
		CritChance = 0.1f,
		Damage = 3,
		AdditionalCritDamage = 1
	};
	
	public static WeaponProperties Pistol = new WeaponProperties()
	{
		WeaponName = "Phaser Pistol",
		MinRange = 1,
		MaxRange = 5,
		CritChance = 0.2f,
		Damage = 2,
		AdditionalCritDamage = 1
	};

    static Weapons()
    {
        AssaultRifle.Abilities.Add(new RangedAttackAbility());
        Pistol.Abilities.Add(new RangedAttackAbility());
    }
}

