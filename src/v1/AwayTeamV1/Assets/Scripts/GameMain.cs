using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enumeration of scenes in the game.
/// Based on Rix's BananaDemo.
/// </summary>
public enum GameSceneType
{
    None,
    MissionScene
}

/// <summary>
/// This class serves two functions: 
/// 1) Loading all of the resources used by the game (e.g. textures)
/// 2) Navigating between the different scenes in the game. 
/// </summary>
public class GameMain : MonoBehaviour
{
    public static GameMain Instance;
    private GameSceneType currentSceneType = GameSceneType.None;
    private GameScene currentScene;

    private void Start()
    {
        Instance = this;
        FutileParams fparams = new FutileParams(true, true, true, true);
        fparams.AddResolutionLevel(800.0f, 1.0f, 1.0f, ""); 
        fparams.origin = new Vector2(0.0f, 0.0f);
        Futile.instance.Init(fparams);

        // Clearly game assets must be put into an atlas as some point, 
        // but for testing purposes I just load each image I need.
		/*Futile.atlasManager.LoadFont("Franchise","FranchiseFont"+
		Futile.resourceSuffix+".png", "Atlases/FranchiseFont"+Futile.resourceSuffix);*/
		//Futile.atlasManager.LoadFont("Arial","arial","arial");
		//Futile.atlasManager.LoadFont("Arial", "arial"+Futile.resourceSuffix+".png", "arial"+Futile.resourceSuffix);
		//Futile.atlasManager.LoadFont("arial", "arial", "Atlases/arial");
		// how do you add fonnnttttssss?????????!!!!!

        //Futile.atlasManager.LoadImage("arial");
        //Futile.atlasManager.LoadFont("arial", "arial", "arialfnt");
        Futile.atlasManager.LoadImage("grasstile");
        Futile.atlasManager.LoadImage("foresttile");
        Futile.atlasManager.LoadImage("evilsoldier");
        Futile.atlasManager.LoadImage("goodsoldier");
        Futile.atlasManager.LoadImage("bluehighlight");
        Futile.atlasManager.LoadImage("redhighlight");
        Futile.atlasManager.LoadImage("phaser");
        Futile.atlasManager.LoadImage("doublebluehighlight");
        Futile.atlasManager.LoadImage("attack");
        Futile.atlasManager.LoadImage("defend");
        Futile.atlasManager.LoadImage("wait");
        Futile.atlasManager.LoadImage("cancel");
        Futile.atlasManager.LoadImage("useitem");
        Futile.atlasManager.LoadImage("move");
        Futile.atlasManager.LoadImage("useitem");
        Futile.atlasManager.LoadImage("crosshair");
        Futile.atlasManager.LoadImage("walltile");
        Futile.atlasManager.LoadImage("floortile");

        GoToScene(GameSceneType.MissionScene);
    }

    public void GoToScene(GameSceneType sceneType)
    {
        if (currentSceneType == sceneType)
        {
            return;
        }

        GameScene sceneToCreate = null;
        switch (sceneType)
        {
            case GameSceneType.MissionScene:
                var missionScene = new MissionScene();

                Team team1 = new Team("Rumbleshank");
                team1.AIControlled = false;
                var actor1 = new ActorProperties();
                actor1.SpriteName = "goodsoldier";
                actor1.MovementPoints = 6;
                actor1.Name = "Hunkenheim1";
                actor1.Abilities.Add(new BasicMoveAbility());
                actor1.Abilities.Add(new WaitAbility());
                team1.Members.Add(actor1);
                var actor2 = new ActorProperties();
                actor2.SpriteName = "goodsoldier";
                actor2.MovementPoints = 6;
                actor2.Name = "Hunkenheim2";
                actor2.Abilities.Add(new BasicMoveAbility());
                actor2.Abilities.Add(new WaitAbility());
                team1.Members.Add(actor2);
                missionScene.AddTeam(team1);

                Team team2 = new Team("Brown Eggz");
                team2.AIControlled = true;
                var actor3 = new ActorProperties();
                actor3.SpriteName = "evilsoldier";
                actor3.MovementPoints = 6;
                actor3.Name = "Scrambled Eggs";
                actor3.Abilities.Add(new BasicMoveAbility());
                actor3.Abilities.Add(new WaitAbility());
                team2.Members.Add(actor3);
                missionScene.AddTeam(team2);

                sceneToCreate = missionScene;
                AwayTeam.MissionController = missionScene;
                break;
            default:
                break;
        }

        if (sceneToCreate != null)
        {
            currentSceneType = sceneType;

            if (currentScene != null)
            {
                Futile.stage.RemoveChild(currentScene);
            }

            currentScene = sceneToCreate;
            Futile.stage.AddChild(currentScene);
            currentScene.Start();
        }
    }
}









