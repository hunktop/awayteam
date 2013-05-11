using System;
using UnityEngine;

public enum AbilityCategory
{
    Movement,
    Wait,
    Attack,
    None
}

public class AbilityCompleteEventArgs : EventArgs
{
    public bool Success
    {
        get;
        set;
    }
}

public abstract class Ability
{
    #region Events

    public event EventHandler<AbilityCompleteEventArgs> AbilityEnded;

    #endregion

    #region Private Fields

    int currentCooldown;

    #endregion 

    #region Properties

    public uint ID
    {
        get;
        private set;
    }

    public string Name
    {
        get;
        private set;
    }

    public AbilityCategory Category
    {
        get;
        private set;
    }

    public string IconName
    {
        get;
        private set;
    }
    
    public int Cooldown
    {
        get;
        private set;
    }

    public bool Available
    {
        get
        {
            return this.currentCooldown == 0;
        }
    }

    public bool EndsTurn
    {
        get;
        set;
    }

    #endregion

    #region Ctor

    public Ability(
        string name,
        uint id,
        AbilityCategory category,
        string icon,
        int cooldown,
        bool endsTurn)
    {
        this.Name = name;
        this.ID = id;
        this.Category = category;
        this.IconName = icon;
        this.Cooldown = cooldown;
    }

    #endregion 
    
    #region Execution

    public void StartExecute(object args)
    {
        if (!this.Available)
        {
            throw new InvalidOperationException("The ability " + this + " is on cooldown.");
        }
        this.ExecuteImpl(args);
    }

    protected K ConvertArgs<K>(object args) where K : class
    {
        if (args == null)
        {
            throw new ArgumentNullException("args");
        }
        var converted = args as K;
        if (converted == null)
        {
            throw new ArgumentException("Expected argument of type " + converted.GetType() + ", actual argument type " + args.GetType());
        }
        return converted;
    }

    protected abstract void ExecuteImpl(object args);

    protected virtual void AfterAbilityExecute(bool completed)
    {
        this.currentCooldown = this.Cooldown;

        var handler = this.AbilityEnded;
        if (handler != null)
        {
            var args = new AbilityCompleteEventArgs() { Success = completed };
            handler(this, args);
        }
    }

    #endregion

    #region Public Methods

    public void DecrementCooldown()
    {
        if (this.currentCooldown > 0)
        {
            this.currentCooldown--;
        }
    }

    public override string ToString()
    {
        return "[" + this.Name + ":" + this.ID + "]";
    }

    #endregion
}