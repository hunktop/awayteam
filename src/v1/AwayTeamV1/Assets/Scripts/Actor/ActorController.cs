using System;
using System.Collections.Generic;

public class ActorController
{
	public List<Team> teams
	{
		get;
		private set;
	}
	
	public ActorController ()
	{
		this.teams = new List<Team>();
		Team blueTeam = new Team("Blue Team");
		blueTeam.AIControlled = false;
		this.teams.Add(blueTeam);
		
		Team redTeam = new Team("Red Team");
		redTeam.AIControlled = false;
		this.teams.Add(redTeam);
		
		blueTeam.Members.Add(MakeSoldier(CharacterData.GenericBlueSoldier1));
		blueTeam.Members.Add(MakeSoldier(CharacterData.GenericBlueSoldier2));
		
		redTeam.Members.Add(MakeSoldier(CharacterData.GenericRedSoldier1));
		redTeam.Members.Add(MakeSoldier(CharacterData.GenericRedSoldier2));
	}
	
	private ActorProperties MakeSoldier(ActorProperties actorProps)
	{
		// so we can't call functions from within the CharacterData static classes... hmm.
		actorProps.Abilities.Add(new BasicMoveAbility());
		actorProps.Abilities.Add(new WaitAbility());
		actorProps.Inventory.AddItem(Weapons.AssaultRifle);
		actorProps.Inventory.EquipItem(Weapons.AssaultRifle);
		return actorProps;
	}
	/*
	 * this.Map = new Map(tiles);
        int i = 5;
        int j = 5;
        foreach (var team in this.teams)
        {
            var actorList = new List<Actor>();
            foreach (var member in team.Members)
            {
                var actor = new Actor(member, team);
                actorList.Add(actor);
                this.Map.AddActor(actor, new Vector2i(i++, j++));
            }
        }*/
	
	
	/*
	 * Team team1 = new Team("Rumbleshank");
                team1.AIControlled = false;
                var actor1 = new ActorProperties();
                actor1.SpriteName = "goodsoldier";
                actor1.MovementPoints = 6;
                actor1.Name = "Hunkenheim1";
                actor1.MaxHealth = 10;
                actor1.CurrentHealth = 10;
                actor1.Abilities.Add(new BasicMoveAbility());
                actor1.Abilities.Add(new WaitAbility());
                actor1.Inventory.AddItem(Weapons.AssaultRifle);
                actor1.Inventory.EquipItem(Weapons.AssaultRifle);
                team1.Members.Add(actor1);
                var actor2 = new ActorProperties();
                actor2.SpriteName = "goodsoldier";
                actor2.MovementPoints = 6;
                actor2.MaxHealth = 10;
                actor2.CurrentHealth = 10;
                actor2.Name = "Hunkenheim2";
                actor2.Abilities.Add(new BasicMoveAbility());
                actor2.Abilities.Add(new WaitAbility());
                team1.Members.Add(actor2);
                missionScene.AddTeam(team1);

                Team team2 = new Team("Brown Eggz");
                team2.AIControlled = true;
                var actor3 = new ActorProperties();
                actor3.SpriteName = "evilsoldier";
                actor3.MaxHealth = 10;
                actor3.CurrentHealth = 10;
                actor3.MovementPoints = 6;
                actor3.Name = "Scrambled Eggs";
                actor3.Abilities.Add(new BasicMoveAbility());
                actor3.Abilities.Add(new WaitAbility());
                team2.Members.Add(actor3);
                missionScene.AddTeam(team2);
                */
}
