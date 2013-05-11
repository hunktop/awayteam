public class RangedWeapon
{
    public int MinRange
    {
        get;
        private set;
    }

    public int MaxRange
    {
        get;
        private set;
    }

    public RangedWeapon(int minRange, int maxRange)
    {
        this.MinRange = minRange;
        this.MinRange = maxRange;
    }
}

