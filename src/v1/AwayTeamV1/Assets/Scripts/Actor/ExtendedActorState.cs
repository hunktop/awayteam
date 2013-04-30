/// <summary>
/// Why is this here? I don't know, I think I had a reason at some point, 
/// but whatever, I'll get rid of this later.
/// </summary>
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