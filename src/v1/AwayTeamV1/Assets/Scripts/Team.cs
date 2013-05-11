using System.Collections.Generic;

public class Team
{
    private List<ActorProperties> members = new List<ActorProperties>();

    public bool AIControlled
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public List<ActorProperties> Members
    {
        get
        {
            return members;
        }
    }

    public Team(string name)
    {
        this.Name = name;
    }
}

