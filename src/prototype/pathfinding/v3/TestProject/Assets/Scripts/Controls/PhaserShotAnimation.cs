using UnityEngine;
class PhaserShotAnimation : AnimationCanvas
{
    private Vector2 start;
    private Vector2 end;
    private Vector2 translated;
    private float velocity = 6f;
    private float fadeRate = 0.05f;
    private FSprite laser;
    private bool done;

    public PhaserShotAnimation(Vector2 s, Vector2 e)
    {
        this.start = s;
        this.end = e;
        this.done = false;

        // Translate end into the coordinate system where start is the origin
        this.translated = this.end - this.start;

        // Compute the angle between end and the x axis 
        var rot = Mathf.Atan2(translated.y, translated.x);

        // Create the laser sprite, rotate it so it points in the right direction
        this.laser = new FSprite("phaser");
        this.laser.anchorX = 0;
        this.laser.rotation = -1 * rot * 180f / Mathf.PI;
        this.laser.x = start.x;
        this.laser.y = start.y;
        this.laser.height = 8;
        this.laser.width = 1;
    }

    public override void Start()
    {
        this.AddChild(this.laser);
        base.Start();
    }

    public override bool AnimationComplete()
    {
        return this.done;
    }

    public override void PrepareFrame()
    {
        if (this.laser.width < this.translated.magnitude)
        {
            this.laser.width += this.velocity;
        }
        else if (laser.alpha > 0)
        {
            this.laser.alpha -= this.fadeRate;
        }
        else
        {
            this.done = true;
        }
        base.PrepareFrame();
    }
}

