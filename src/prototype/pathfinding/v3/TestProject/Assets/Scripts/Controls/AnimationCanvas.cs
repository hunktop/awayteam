using UnityEngine;

/// <summary>
/// A very rudimentary class for animating stuff in an FContainer.
/// Basically calls a method every X milliseconds to update the frame. 
/// Animation complete when "AnimationComplete" method returns true.
/// Doesn't do much by itself, subclasses are expected to implement the 
/// animation behavior.
/// </summary>
class AnimationCanvas : FContainer
{
    private bool pause;
    private float time;

    public int Delay
    {
        get;
        set;
    }

    public void Pause()
    {
        this.pause = true;
    }

    public void Play()
    {
        this.pause = false;
    }

    /// <summary>
    /// Sets up the FContainer for the animation.
    /// Should be implemented by inheritors.
    /// </summary>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Renders the next frame of the animation.  Should be implemented 
    /// by inheritors.
    /// </summary>
    public virtual void PrepareFrame()
    {
    }

    /// <summary>
    /// Returns true if the animation if complete. Should be implemented 
    /// by inheritors.
    /// </summary>
    public virtual bool AnimationComplete()
    {
        return true;
    }

    /// <summary>
    /// Took this timer code with slight adaptations from:
    /// https://github.com/mattfox12/FutileAdditionalClasses/blob/master/Plugins/FAnimatedSprite.cs
    /// In short this methods calls PrepareFrame on a timer to move the 
    /// animation forwards.
    /// </summary>
    public virtual void Update()
    {
        if (!this.pause)
        {
            time += Time.deltaTime;
            while (time > (float) this.Delay / 1000.0f)
            {
                if (this.AnimationComplete())
                {
                    break;
                }
                this.PrepareFrame();
                time -= (float) this.Delay / 1000.0f;
            }
        }
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
