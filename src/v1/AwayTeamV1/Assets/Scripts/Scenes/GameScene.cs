using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for game scenes, implements functionality common to all game scenes,
/// basically hooking up events necessary for Update, Resize.
/// </summary>
public class GameScene : FContainer
{
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Resize(bool wasOrientationChange)
    {
    }

    override public void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        Futile.screen.SignalResize += Resize;
        base.HandleAddedToStage();
    }

    override public void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        Futile.screen.SignalResize -= Resize;
        base.HandleRemovedFromStage();
    }
}
