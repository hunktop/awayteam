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
                sceneToCreate = new MissionScene();
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









