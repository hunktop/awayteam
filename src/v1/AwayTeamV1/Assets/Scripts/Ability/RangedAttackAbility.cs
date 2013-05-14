using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RangedAttackAbility : AttackAbility
{
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

    protected override void ExecuteImpl(object args)
    {
        var attackArgs = this.ConvertArgs<AttackArgs>(args);
        var projectileEnd = AwayTeam.GridToGlobal(attackArgs.TargetedLocation);
        var projectileStart = AwayTeam.GridToGlobal(attackArgs.Actor.GridPosition);
        this.shootLaser(projectileStart, projectileEnd);
    }

    public override AbilityController GetController()
    {
        return AttackController.Instance;
    }

    private void shootLaser(Vector2 start, Vector2 end)
    {
        var phaserShot = new PhaserShotAnimation(start, end);
        phaserShot.Start();
        phaserShot.Delay = 10;
        phaserShot.AnimationStopped += new EventHandler<AnimationEventArgs>(this.phaserShot_AnimationStopped);
        AwayTeam.MissionController.AddChild(phaserShot);
        phaserShot.Play();
    }

    private void phaserShot_AnimationStopped(object sender, AnimationEventArgs args)
    {
        if (args.StoppedReason == StoppedReason.Complete)
        {
            AwayTeam.MissionController.RemoveChild(sender as FNode);
            this.AfterAbilityExecute(true);
        }
    }
}