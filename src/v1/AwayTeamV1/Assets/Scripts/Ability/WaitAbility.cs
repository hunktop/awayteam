using System;
using System.Collections.Generic;

public class WaitArgs
{
    public Actor Actor
    {
        get;
        set;
    }
}

public class WaitAbility : Ability
{
    public WaitAbility()
        : base(
            "Wait",
            AbilityId.Wait,
            AbilityCategory.Wait,
            "wait",
            1,
            true)
    {
    }

    protected override void ExecuteImpl(object args)
    {
        var waitArgs = this.ConvertArgs<WaitArgs>(args);
        waitArgs.Actor.TurnState = ActorState.TurnOver;
    }

    public override AbilityController GetController()
    {
        return WaitController.Instance;
    }
}