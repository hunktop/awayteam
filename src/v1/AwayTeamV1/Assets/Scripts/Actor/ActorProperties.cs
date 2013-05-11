using System;
using System.Collections.Generic;
using System.Linq;

public class ActorProperties
{
    #region Private Fields

    private HashSet<Ability> allAbilities = new HashSet<Ability>();

    #endregion

    #region Attributes

    public string Name
    {
        get;
        set;
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

    public HashSet<Ability> Abilities
    {
        get
        {
            return this.allAbilities;
        }
    }

    #endregion

    #region Turn State

    public IEnumerable<Ability> AvailableAbilities
    {
        get
        {
            return this.allAbilities.Where(a => a.Available);
        }
    }

    #endregion

    public override string ToString()
    {
        return this.Name;
    }
}


