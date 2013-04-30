using System;

public class Actor : FSprite
{
    public ActorProperties Properties
    {
        get;
        private set;
    }

    public ActorState TurnState
    {
        get;
        set;
    }

    public bool HasMovedThisTurn
    {
        get;
        set;
    }
        
    public bool IsEnemy
    {
        get;
        set;
    }

	public Actor(ActorProperties actorProperties) 
		: base(actorProperties == null ? "unknown" : actorProperties.SpriteName )
	{
        this.Properties = actorProperties;
	}
}


