using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BasicRangedAttackAbility : AttackAbility
{
    public BasicRangedAttackAbility(string name, uint id, string iconname, int cooldown)
        : base(
            "Basic Ranged Attack",
            AbilityId.BasicAttack,
            "attack",
            1)
    {
    }

    public override IEnumerable<Vector2i> GetAttackableLocations(Map map, Actor actor)
    {
        throw new NotImplementedException();
    }

    protected override void ExecuteImpl(object args)
    {
        throw new NotImplementedException();
    }

    //private void shootLaser(Vector2 start, Vector2 end)
    //{
    //    var phaserShot = new PhaserShotAnimation(start, end);
    //    phaserShot.Start();
    //    phaserShot.Delay = 10;
    //    phaserShot.AnimationStopped += new EventHandler<AnimationEventArgs>(this.phaserShot_AnimationStopped);
    //    this.AddChild(phaserShot);
    //    phaserShot.Play();
    //}

    //private void phaserShot_AnimationStopped(object sender, AnimationEventArgs args)
    //{
    //    if (args.StoppedReason == StoppedReason.Complete)
    //    {
    //        Debug.Log("Removing phaser " + sender);
    //        this.RemoveChild(sender as FNode);
    //    }
    //}
}