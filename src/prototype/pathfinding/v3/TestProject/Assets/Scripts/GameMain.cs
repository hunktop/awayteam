using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GamePageType
{
    None,
    MissionPage
}

public class GameMain : MonoBehaviour
{
    public static GameMain Instance;
    private GamePageType currentPageType = GamePageType.None;
    private GamePage currentPage;

    private void Start()
    {
        Instance = this;
        FutileParams fparams = new FutileParams(true, true, true, true);
        fparams.AddResolutionLevel(800.0f, 1.0f, 1.0f, ""); 
        fparams.origin = new Vector2(0.0f, 0.0f);
        Futile.instance.Init(fparams);
        Futile.atlasManager.LoadImage("grasstile");
        Futile.atlasManager.LoadImage("foresttile");
        Futile.atlasManager.LoadImage("evilsoldier");
        Futile.atlasManager.LoadImage("goodsoldier");
        Futile.atlasManager.LoadImage("bluehighlight");
        Futile.atlasManager.LoadImage("redhighlight");
        Futile.atlasManager.LoadImage("doublebluehighlight");
        Futile.atlasManager.LoadImage("attack");
        Futile.atlasManager.LoadImage("defend");
        Futile.atlasManager.LoadImage("cancel");
        Futile.atlasManager.LoadImage("move");
        Futile.atlasManager.LoadImage("useitem");
        GoToPage(GamePageType.MissionPage);
    }

    public void GoToPage(GamePageType pageType)
    {
        if (currentPageType == pageType)
        {
            return;
        }

        GamePage pageToCreate = null;
        switch (pageType)
        {
            case GamePageType.MissionPage:
                pageToCreate = new MissionPage();
                break;
            default:
                break;
        }

        if (pageToCreate != null)
        {
            currentPageType = pageType;

            if (currentPage != null)
            {
                Futile.stage.RemoveChild(currentPage);
            }

            currentPage = pageToCreate;
            Futile.stage.AddChild(currentPage);
            currentPage.Start();
        }
    }
}









