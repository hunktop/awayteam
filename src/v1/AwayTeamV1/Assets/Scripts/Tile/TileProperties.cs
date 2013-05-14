/// <summary>
/// This class is a temporary way to represent tile metadata -- 
/// probably should replaced by something better in the future.
/// </summary>
public class TileProperties
{
    public string SpriteName
    {
        get;
        set;
    }

    public int MovementPenalty
    {
        get;
        set;
    }

    public bool BlocksVision
    {
        get;
        set;
    }
}

