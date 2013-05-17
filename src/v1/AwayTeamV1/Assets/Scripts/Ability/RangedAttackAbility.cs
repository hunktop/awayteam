using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RangedAttackAbility : AttackAbility
{
    private Actor hitActor;
    private int damage;
    private AttackArgs attackArgs;

    public RangedAttackAbility()
        : base(
            "Ranged Attack",
            AbilityId.BasicAttack,
            "attack",
            1)
    {
    }

    public override IEnumerable<Vector2i> GetAttackableLocations(Map map, Actor actor, WeaponProperties weapon)
    {
        return AttackHelper.GetTargetablePoints(map, actor, weapon.MinRange, weapon.MaxRange, true, false);
    }

    public override AbilityController GetController()
    {
        return AttackController.Instance;
    }

    #region Combat Calculation

    protected override void ExecuteImpl(object args)
    {
        this.attackArgs = this.ConvertArgs<AttackArgs>(args);
        var projectileEnd = AwayTeam.GridToGlobal(attackArgs.TargetedLocation);
        var projectileStart = AwayTeam.GridToGlobal(attackArgs.Actor.GridPosition);
        var actor = attackArgs.Actor;
        var weapon = attackArgs.Actor.EquippedItem as WeaponProperties;
        bool hits = false;
        
        var squareHit = this.GetSquareHit(attackArgs.Actor, attackArgs.TargetedLocation);
        if (attackArgs.Map.TryGetActor(attackArgs.TargetedLocation, out hitActor))
        {
            var hitChance = GetChanceToHitActor(attackArgs.Actor, hitActor, weapon);
            var hitRoll = UnityEngine.Random.Range(0.0f, 1.0f);
            if (hitRoll <= hitChance)
            {
                hits = true;
                this.damage = this.GetDamageOnHit(attackArgs.Actor, hitActor, weapon);
            }
        }
        this.shootLaser(projectileStart, projectileEnd, hits, this.damage);
    }

    private void ApplyDamage(Map map, Actor actor, int damage)
    {
        actor.Properties.CurrentHealth = Math.Max(0, actor.Properties.CurrentHealth - damage);
        if (actor.Properties.CurrentHealth == 0)
        {
            AwayTeam.MissionController.RemoveActor(actor);
        }
    }

    private Vector2i GetSquareHit(Actor attacker, Vector2i destination)
    {
        return destination;
    }

    private float GetChanceToHitActor(Actor attacker, Actor target, WeaponProperties properties)
    {
        return 0.80f;
    }

    private int GetDamageOnHit(Actor attacker, Actor target, WeaponProperties properties)
    {
        var damage = properties.Damage;
        var critRoll = UnityEngine.Random.Range(0.0f, 1.0f);
        if (critRoll <= properties.CritChance)
        {
            damage += properties.AdditionalCritDamage;
        }
        return damage;
    }

    #endregion

    #region Animation

    private void shootLaser(Vector2 start, Vector2 end, bool hits, int damage)
    {
        var phaserShot = new PhaserShotAnimation(start, end, hits, damage);
        phaserShot.Start();
        phaserShot.Delay = 10;
        phaserShot.AnimationStopped += new EventHandler<AnimationEventArgs>(this.phaserShot_AnimationStopped);
        AwayTeam.MissionController.MapContainer.AddChild(phaserShot);
        phaserShot.Play();
    }

    private void phaserShot_AnimationStopped(object sender, AnimationEventArgs args)
    {
        if (args.StoppedReason == StoppedReason.Complete)
        {
            if (this.hitActor != null)
            {
                this.ApplyDamage(this.attackArgs.Map, this.hitActor, this.damage);
            }
            AwayTeam.MissionController.MapContainer.RemoveChild(sender as FNode);
            this.AfterAbilityExecute(true);
        }
    }

    #endregion
}