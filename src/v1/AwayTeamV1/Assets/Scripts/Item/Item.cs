using System.Collections.Generic;

public class Item
{
    List<Ability> abilities = new List<Ability>();

    public string Name
    {
        get;
        set;
    }

    public List<Ability> Abilities
    {
        get
        {
            return this.abilities;
        }
    }

    public override string ToString()
    {
        return "[Item: " + this.Name + "]";
    }
}