using System;
using System.Collections.Generic;
using System.Linq;

public class Actor : FSprite
{
    #region Private Fields

    private HashSet<Ability> allAbilities = new HashSet<Ability>();
    private static uint IdCounter = 0;

    #endregion 

    #region Attributes

    public Team Team
    {
        get;
        private set;
    }

    public uint ID
    {
        get;
        private set;
    }

    public ActorProperties Properties
    {
        get;
        private set;
    }

    public Vector2i GridPosition
    {
        get
        {
            var gridX = (int)((x / AwayTeam.TileSize) + 1) - 1;
            var gridY = (int)((y / AwayTeam.TileSize) + 1) - 1;
            return new Vector2i(gridX, gridY);
        }
        set
        {
            this.x = value.X * AwayTeam.TileSize + AwayTeam.TileSize / 2;
            this.y = value.Y * AwayTeam.TileSize + AwayTeam.TileSize / 2;
        }
    }
	
	public WeaponProperties Weapon
	{
		get;
		set;
	}

    #endregion

    #region Turn State

    public ActorState TurnState
    {
        get;
        set;
    }

    #endregion

    #region Ctor

    public Actor(ActorProperties properties, Team team)
        : base(properties.SpriteName)
    {
        this.ID = IdCounter++;
        this.Team = team;
        this.Properties = properties;
    }

    #endregion

    #region Public Methods

    public override string ToString()
    {
        return string.Format("[Name:{0}, Position:{1}, State:{2}]", this.Properties.Name, this.GridPosition, this.TurnState);
    }

    #endregion
}


