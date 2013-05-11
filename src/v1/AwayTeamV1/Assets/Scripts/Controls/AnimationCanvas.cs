using UnityEngine;
using System;

public enum StoppedReason 
{
    None, 
    Complete,
    Paused,
    Error
}

public class AnimationEventArgs : EventArgs 
{
    public StoppedReason StoppedReason
    {
        get;
        set;
    }
}

/// <summary>
/// A very rudimentary class for animating stuff in an FContainer.
/// Basically calls a method every X milliseconds to update the frame. 
/// Animation complete when "AnimationComplete" method returns true.
/// Doesn't do much by itself, subclasses are expected to implement the 
/// animation behavior.
/// </summary>
public class AnimationCanvas : FContainer
{
    private bool paused;
    private bool complete;
    private float time;

    public event EventHandler<AnimationEventArgs> AnimationStarted;
    public event EventHandler<AnimationEventArgs> AnimationStopped;

    public int Delay
    {
        get;
        set;
    }

    public bool Running
    {
        get
        {
            return !this.paused && !this.complete;
        }
    }

    public AnimationCanvas()
        : this(false)
    {

    }

    public AnimationCanvas(bool playWhenAdded)
    {
        this.paused = playWhenAdded;
    }

    public void Pause()
    {
        if (!this.paused)
        {
            this.paused = true;
            this.OnAnimationStopped(StoppedReason.Paused);
        }
    }

    public void Play()
    {
        if (this.complete)
        {
            return;
        } else if (this.paused)
        {
            this.paused = false;
            this.OnAnimationStarted();
        }
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
        if (!this.paused)
        {
            time += Time.deltaTime;
            while (time > (float)this.Delay / 1000.0f)
            {
                if (this.AnimationComplete())
                {
                    this.complete = true;
                    this.paused = true;
                    this.OnAnimationStopped(StoppedReason.Complete);
                    break;
                }

                this.PrepareFrame();
                time -= (float)this.Delay / 1000.0f;
            }
        }
    }

    public virtual void Resize(bool wasOrientationChange)
    {
    }

    public override void HandleAddedToStage()
    {
        this.Start();
        Futile.instance.SignalUpdate += Update;
        Futile.screen.SignalResize += Resize;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        Futile.screen.SignalResize -= Resize;
        base.HandleRemovedFromStage();
    }

    protected virtual void OnAnimationStarted()
    {
        var handler = this.AnimationStarted;
        if (handler != null)
        {
            var args = new AnimationEventArgs();
            args.StoppedReason = StoppedReason.None;
            handler(this, args);
        }
    }

    protected virtual void OnAnimationStopped(StoppedReason reason)
    {
        var handler = this.AnimationStopped;
        if (handler != null)
        {
            var args = new AnimationEventArgs();
            args.StoppedReason = reason;
            handler(this, args);
        }
    }
}
