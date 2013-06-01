using System;

public class CharacterData
{
	/*
	 * can't call functions to generate random names from a static object. makes sense...
	private enum names
	{
		Will,
		Ian,
		Edric,
		Arshed,
		Joe
	}
	
	static T RandomEnumValue<T> ()
	{
	    return Enum
	        .GetValues (typeof (T))
	        .Cast<T> ()
	        .OrderBy (x => _Random.Next())
	        .FirstOrDefault ();
	}
	
	private string getRandomName()
	{
		var randName = RandomEnumValue<names> ();
		return randName.ToString();
	}
	*/
	
	public static ActorProperties GenericBlueSoldier1 = new ActorProperties()
	{
		SpriteName = "goodsoldier",
		MovementPoints = 6,
		Name = "Will",
		MaxHealth = 10,
		CurrentHealth = 10
	};
	public static ActorProperties GenericBlueSoldier2 = new ActorProperties()
	{
		SpriteName = "goodsoldier",
		MovementPoints = 6,
		Name = "Ian",
		MaxHealth = 10,
		CurrentHealth = 10
	};
	
	public static ActorProperties GenericRedSoldier1 = new ActorProperties()
	{
		SpriteName = "evilsoldier",
		MovementPoints = 6,
		Name = "The",
		MaxHealth = 10,
		CurrentHealth = 10
	};
	public static ActorProperties GenericRedSoldier2 = new ActorProperties()
	{
		SpriteName = "evilsoldier",
		MovementPoints = 6,
		Name = "World",
		MaxHealth = 10,
		CurrentHealth = 10
	};
}