using System;
using System.Collections.Generic;
using System.Linq;

public class ActorProperties
{
    #region Private Fields

    private List<Ability> myAbilities = new List<Ability>();

    #endregion

    #region Properties

    public string Name
    {
        get;
        set;
    }

    public bool IsAlive
    {
        get
        {
            return this.CurrentHealth > 0;
        }
    }

    public int MaxHealth
    {
        get;
        set;
    }

    public int CurrentHealth
    {
        get;
        set;
    }

    public IEnumerable<Ability> AvailableAbilities
    {
        get
        {
            if (this.Inventory.EquippedItem != null)
            {
                return this.myAbilities
                    .Union(this.Inventory.EquippedItem.Abilities)
                    .Where(a => a.Available);
            }
            else
            {
                return myAbilities.Where(a => a.Available);
            }
        }
    }

    public IEnumerable<Ability> AllAbilities
    {
        get
        {
            if (this.Inventory.EquippedItem != null)
            {
                return this.myAbilities.Union(this.Inventory.EquippedItem.Abilities);
            }
            else
            {
                return myAbilities;
            }
        }
    }

    public int MovementPoints
    {
        get;
        set;
    }

    public string SpriteName
    {
        get;
        set;
    }

    public List<Ability> Abilities
    {
        get
        {
            return this.myAbilities;
        }
    }

    public Inventory Inventory
    {
        get;
        private set;
    }

    #endregion

    public ActorProperties()
    {
        this.Inventory = new Inventory();
    }

    public override string ToString()
    {
        return this.Name;
    }
}


