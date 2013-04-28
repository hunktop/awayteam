class ExtendedActorState
{
    public Vector2i Location
    {
        get;
        set;
    }

    public ActorState State
    {
        get;
        set;
    }

    public bool CanMove
    {
        get;
        set;
    }

    public bool CanAttack
    {
        get;
        set;
    }

    public bool CanCancel
    {
        get;
        set;
    }

    public bool CanWait
    {
        get;
        set;
    }
}