using System.Collections.Generic;
using UnityEngine;
using System;

#region Event Arg Definition

public enum ActionCompleteReason 
{
    Canceled,
    Complete
}

public class ActionCompleteEventArgs : EventArgs 
{
    public bool TurnOver
    {
        get;
        set;
    }

    public ActionCompleteReason Reason
    {
        get;
        set;
    }

    public ActionCompleteEventArgs(bool turnOver, ActionCompleteReason reason)
    {
        this.TurnOver = turnOver;
        this.Reason = reason;
    }
}

#endregion

public abstract class AbilityController
{
    #region Events

    public event EventHandler<ActionCompleteEventArgs> ActionComplete;

    #endregion

    #region Properties

    public bool Active
    {
        get;
        protected set;
    }

    public MissionScene ParentController
    {
        get
        {
            return AwayTeam.MissionController;
        }
    }

    protected Map Map
    {
        get
        {
            return this.ParentController.Map;
        }
    }

    protected Actor SelectedActor
    {
        get
        {
            return this.ParentController.SelectedActor;
        }
    }

    #endregion

    #region Public Methods

    public virtual void Activate(Ability ability)
    {
        Futile.instance.SignalUpdate += Update;
        this.Active = true;
    }

    public virtual void Deactivate(bool turnOver, ActionCompleteReason reason)
    {
        this.OnAbilityComplete(new ActionCompleteEventArgs(turnOver, reason));
        this.Active = false;
    }

    public virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            this.Deactivate(false, ActionCompleteReason.Canceled);
        }
    }

    #endregion

    #region Private Methods

    private void OnAbilityComplete(ActionCompleteEventArgs args)
    {
        var handler = this.ActionComplete;
        if (handler != null)
        {
            handler(this, args);
        }
    }

    #endregion
}
