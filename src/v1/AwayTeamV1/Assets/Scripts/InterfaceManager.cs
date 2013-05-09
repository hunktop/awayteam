using System;
using UnityEngine;

public class InterfaceManager : ScriptableObject
{
	private FLabel whoseTurn;
	
	public InterfaceManager ()
	{
	}
	public void Start()
	{
		
	}
	void OnGui()
	{
		GUI.Box(new Rect(10,10,100,90), "Test");
		
	}
}

