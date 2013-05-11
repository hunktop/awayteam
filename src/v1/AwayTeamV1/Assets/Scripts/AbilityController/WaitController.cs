using System.Collections.Generic;
using UnityEngine;
using System;

public class WaitController : AbilityController
{
    private static WaitController instance;

    private WaitController() 
    { 
    }

    public static WaitController Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new WaitController();
            }
            return instance;
        }
    }

    public override void Activate(Ability ability)
    {
        if ((ability as WaitAbility) == null)
        {
            throw new ArgumentException("Ability " + ability + " is not implemented by this controller.");
        }

        base.Activate(ability);
        this.Deactivate(true, ActionCompleteReason.Complete);
    }

    public override void Deactivate(bool turnOver, ActionCompleteReason reason)
    {
        base.Deactivate(turnOver, reason);
    }
}
