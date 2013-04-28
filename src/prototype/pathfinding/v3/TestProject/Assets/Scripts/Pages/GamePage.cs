using UnityEngine;
using System.Collections;

public class GamePage : FContainer
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
