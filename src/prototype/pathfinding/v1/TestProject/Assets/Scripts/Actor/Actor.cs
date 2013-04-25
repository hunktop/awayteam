using System;

public class Actor : FSprite
{
    public ActorProperties Properties
    {
        get;
        private set;
    }

	public Actor(ActorProperties actorProperties) 
		: base(actorProperties == null ? "unknown" : actorProperties.SpriteName )
	{
        this.Properties = actorProperties;
	}
}


