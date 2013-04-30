using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class implements the "Mission" scene, where the turned based strategy stuff happens.
/// Still under HEAVY development.
/// </summary>
public class MissionScene : GameScene, FSingleTouchableInterface
{
    #region Private Fields

    private int tileSize;
    private Map map;
    private Actor selectedActor; 
    private PathfindResult pathfindResults;

    // Some book-keeping lists/sets
    private List<Vector2i> highlightedPath;
    private HashSet<Vector2i> localAttackablePoints;
    private HashSet<Vector2i> highlightedIndicies;

    private FSprite blueHighlight;
    private FSprite redHighlight;
    private FSprite crosshair;
    private FSprite[,] highlights;
    private FButton moveButton;
    private FButton attackButton;
    private FButton cancelButton;
    private FButton waitButton;
    private FButton useItemButton;
    private ButtonStrip buttonStrip;
    private PhaserShotAnimation phaserShot;
   
    #endregion

    #region Start

    /// <summary>
    /// Initialize resources.  Right now, for testing purposes, a simple random map is 
    /// generated here with a few actors in it.
    /// </summary>
    public override void Start()
    {
        this.tileSize = 32;
        var width = 50;
        var height = 35;
        var tiles = new TileProperties[width, height];
        this.localAttackablePoints = new HashSet<Vector2i>();

        // Generate a very simple random map with two different kinds of tiles
        var rand = new System.Random();
        for (int ii = 0; ii < width; ii++)
        {
            for (int jj = 0; jj < height; jj++)
            {
                tiles[ii, jj] = (rand.NextDouble() < 0.7) ? StaticTiles.GrassTile : StaticTiles.ForestTiles;
            }
        }

        // Add a few actors to the map
        this.map = new Map(tiles, tileSize);
        this.map.AddActor(StaticActors.GoodSoldier, new Vector2i(5, 5));
        this.map.AddActor(StaticActors.GoodSoldier, new Vector2i(5, 6));
        this.map.AddActor(StaticActors.EvilSoldier, new Vector2i(5, 7), true);
        this.map.Start();
        this.AddChild(this.map);

        // Initilialize buttons
        this.crosshair = new FSprite("crosshair");
        this.crosshair.width = this.crosshair.height = this.tileSize;
        this.moveButton = new FButton("move");
        this.moveButton.sprite.width = this.moveButton.sprite.height = 32;
        this.attackButton = new FButton("attack");
        this.attackButton.sprite.width = this.attackButton.sprite.height = 32;
        this.waitButton = new FButton("wait");
        this.waitButton.sprite.width = this.waitButton.sprite.height = 32;
        this.useItemButton = new FButton("useitem");
        this.useItemButton.sprite.width = this.useItemButton.sprite.height = 32;
        this.moveButton.SignalRelease += new Action<FButton>(moveButton_SignalRelease);
        this.attackButton.SignalRelease += new Action<FButton>(attackButton_SignalRelease);
        this.waitButton.SignalRelease += new Action<FButton>(waitButton_SignalRelease);

        // Initialize highlights
        this.highlightedIndicies = new HashSet<Vector2i>();
        this.highlights = new FSprite[map.Columns, map.Rows];
        this.highlightedPath = new List<Vector2i>();
        for (int ii = 0; ii < map.Columns; ii++)
        {
            for (int jj = 0; jj < map.Rows; jj++)
            {
                var highlightSprite = new FSprite("bluehighlight");
                highlightSprite.x = ii * this.tileSize + this.tileSize / 2;
                highlightSprite.y = jj * this.tileSize + this.tileSize / 2;
                highlightSprite.isVisible = false;
                highlightSprite.width = highlightSprite.height = this.tileSize;
                this.highlights[ii, jj] = highlightSprite;
                this.AddChild(highlightSprite);
            }
        }
    }

    #endregion

    #region Update

    /// <summary>
    /// Update does a few things at the moment:
    /// - Handles mouse-over events (look at the position of the mouse and figure out if 
    ///   we have to change the display in some way).
    /// - Handles key presses
    /// </summary>
    public override void  Update()
    {
        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var globalCoords = MouseManager.ScreenToGlobal(mousePosition2d);
        var gridVector = this.map.GlobalToGrid(globalCoords);
        this.scrollMap(mousePosition2d);

        if (this.selectedActor != null)
        {
            var actorState = this.selectedActor.TurnState;
            if (actorState == ActorState.SelectingDestination)
            {
                this.highlightPath(gridVector);
            }
            else if (actorState == ActorState.SelectingEnemy)
            {
                if (this.localAttackablePoints.Contains(gridVector))
                {
                    this.drawCrossHair(gridVector);
                }
                else
                {
                    this.clearCrossHair();
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (actorState == ActorState.SelectingDestination || actorState == ActorState.SelectingEnemy)
                {
                    this.selectedActor.TurnState = ActorState.AwaitingCommand;
                    this.clearHighlights();
                    this.clearCrossHair();
                    this.showButtonStrip(this.selectedActor);
                }
                else if (actorState == ActorState.AwaitingCommand)
                {
                    if (!this.selectedActor.HasMovedThisTurn)
                    {
                        this.selectedActor.TurnState = ActorState.TurnStart;
                        this.clearSelection();
                        this.RemoveChild(this.buttonStrip);
                    }
                }
            }
        }

        base.Update();
    }

    #endregion 

    #region Overrides

    override public void HandleAddedToStage()
    {
        base.HandleAddedToStage();
        Futile.touchManager.AddSingleTouchTarget(this);
    }

    override public void HandleRemovedFromStage()
    {
        base.HandleRemovedFromStage();
        Futile.touchManager.RemoveSingleTouchTarget(this);
    }

    #endregion 

    #region Touch Handlers

    public bool HandleSingleTouchBegan(FTouch touch)
    {
        if (this.selectedActor != null && this.selectedActor.TurnState == ActorState.AwaitingCommand)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void HandleSingleTouchMoved(FTouch touch)
    {
    }

    public void HandleSingleTouchEnded(FTouch touch)
    {
        var gridVector = this.map.GlobalToGrid(touch.position);
        Debug.Log("Touch at: " + touch.position + ", map grid: " + gridVector);

        if (this.selectedActor != null)
        {
            var actorState = this.selectedActor.TurnState;
            if (actorState == ActorState.SelectingDestination)
            {
                if (this.pathfindResults != null && this.pathfindResults.VisitablePoints.Contains(gridVector))
                {
                    this.selectedActor.HasMovedThisTurn = true;
                    this.selectedActor.TurnState = ActorState.AwaitingCommand;
                    this.map.UpdateLocation(this.selectedActor, gridVector);

                    // Recompute the set of attackable points, since the actor has moved.
                    var tempEnum = MoveAndAttackHelper.GetAttackablePoints(this.map, gridVector, 1, 6);
                    this.localAttackablePoints = new HashSet<Vector2i>(tempEnum);
                    this.showButtonStrip(this.selectedActor);
                    this.clearHighlights();
                }
            }
            else if(actorState == ActorState.SelectingEnemy)
            {
                if (this.localAttackablePoints.Contains(gridVector))
                {
                    Debug.Log("Attacking point: " + gridVector);
                    Vector2 source = new Vector2(this.selectedActor.x, this.selectedActor.y);
                    Vector2 dest = this.map.GridToGlobal(gridVector);
                    this.ShootLaser(source, dest);
                }
            }
        }
        else if (this.map.TryGetActor(gridVector, out this.selectedActor))
        {
            var actorState = this.selectedActor.TurnState;
            if (actorState == ActorState.TurnStart)
            {
                this.selectedActor.TurnState = ActorState.AwaitingCommand;
                this.pathfindResults = MoveAndAttackHelper.Pathfind(this.map, this.selectedActor);
                var tempEnum = MoveAndAttackHelper.GetAttackablePoints(this.map, gridVector, 1, 6);
                this.localAttackablePoints = new HashSet<Vector2i>(tempEnum);
                this.showButtonStrip(this.selectedActor);
            }
        }
    }

    public void HandleSingleTouchCanceled(FTouch touch)
    {
    }

    #endregion 

    #region Button Presses

    private void moveButton_SignalRelease(FButton b)
    {
        Debug.Log("Move button clicked.");
        
        if (this.selectedActor != null 
            && this.selectedActor.TurnState == ActorState.AwaitingCommand 
            && !this.selectedActor.HasMovedThisTurn)
        {
            this.highlightDesinations();
            this.selectedActor.TurnState = ActorState.SelectingDestination;
            this.RemoveChild(this.buttonStrip);
        }
    }

    private void attackButton_SignalRelease(FButton b)
    {
        Debug.Log("Attack button clicked.");
        if (this.selectedActor != null)
        {
            this.highlightLocalAttackable();
            this.selectedActor.TurnState = ActorState.SelectingEnemy;
            this.RemoveChild(this.buttonStrip);
        }
    }

    private void waitButton_SignalRelease(FButton b)
    {
        Debug.Log("Wait button clicked.");
        if (this.selectedActor != null)
        {
            this.selectedActor.TurnState = ActorState.TurnOver;
            this.clearSelection();
            this.RemoveChild(this.buttonStrip);
        }
    }

    #endregion

    #region Private Methods

    private void ShootLaser(Vector2 start, Vector2 end)
    {
        var phaserShot = new PhaserShotAnimation(start, end);
        phaserShot.Start();
        phaserShot.Delay = 10;
        phaserShot.AnimationStopped += new EventHandler<AnimationEventArgs>(this.phaserShot_AnimationStopped);
        this.AddChild(phaserShot);
        phaserShot.Play();
    }

    private void phaserShot_AnimationStopped(object sender, AnimationEventArgs args)
    {
        if (args.StoppedReason == StoppedReason.Complete)
        {
            Debug.Log("Removing phaser " + sender);
            this.RemoveChild(sender as FNode);
        }
    }

    private void setElement(FSprite sprite, string name)
    {
        if (sprite.element.name != name)
        {
            sprite.element = Futile.atlasManager.GetElementWithName(name);
        }
    }

    private void clearCrossHair()
    {
        if (this._childNodes.Contains(this.crosshair))
        {
            this.RemoveChild(this.crosshair);
        }
    }

    private void clearHighlights()
    {
        foreach (var index in this.highlightedIndicies)
        {
            this.highlights[index.X, index.Y].isVisible = false;
        }
        this.highlightedIndicies.Clear();
    }

    private void clearHighlightedPath()
    {
        foreach (var item in this.highlightedPath)
        {
            this.highlights[item.X, item.Y].element = Futile.atlasManager.GetElementWithName("bluehighlight");
        }
        this.highlightedPath.Clear();
    }

    private void clearSelection()
    {
        this.clearHighlights();
        this.selectedActor = null;

    }

    private void showButtonStrip(Actor actor)
    {
        this.buttonStrip = new ButtonStrip();
        this.buttonStrip.Orientation = ButtonStripOrientation.Horizontal;
        this.buttonStrip.Spacing = 3.0f;

        if (actor != null && actor.TurnState == ActorState.AwaitingCommand)
        {
            if (!actor.HasMovedThisTurn)
            {
                this.buttonStrip.AddButton(this.moveButton);
            }

            this.buttonStrip.AddButton(this.attackButton);
            this.buttonStrip.AddButton(this.waitButton);
           
            this.buttonStrip.isVisible = true;
            this.buttonStrip.x = actor.x + actor.width;
            this.buttonStrip.y = actor.y - actor.height;
            this.AddChild(this.buttonStrip);
        }
    }

    /// <summary>
    /// Draw a crosshair at the given position in the grid.
    /// </summary>
    /// <param name="dest"></param>
    private void drawCrossHair(Vector2i dest)
    {
        this.crosshair.x = dest.X * this.tileSize + this.tileSize / 2;
        this.crosshair.y = dest.Y * this.tileSize + this.tileSize / 2;

        if (!this._childNodes.Contains(this.crosshair))
        {
            this.AddChild(this.crosshair);
        }
    }

    private void highlightDesinations()
    {
        this.clearHighlights();
        foreach (var item in this.pathfindResults.VisitablePoints)
        {
            this.setElement(this.highlights[item.X, item.Y], "bluehighlight");
            this.highlights[item.X, item.Y].isVisible = true;
            this.highlightedIndicies.Add(item);
        }
    }

    private void highlightLocalAttackable()
    {
        this.clearHighlights();
        foreach (var item in this.localAttackablePoints)
        {
            this.setElement(this.highlights[item.X, item.Y], "redhighlight");
            this.highlights[item.X, item.Y].isVisible = true;
            this.highlightedIndicies.Add(item);
        }
    }

    private void highlightPath(Vector2i dest)
    {
        if (this.selectedActor != null)
        {
            this.clearHighlightedPath();
            if (this.pathfindResults.VisitablePoints.Contains(dest))
            {
                Vector2i p = dest;
                Vector2i origin;
                if (map.TryGetLocation(this.selectedActor, out origin))
                {
                    while (p != origin)
                    {
                        this.highlightedPath.Add(p);
                        this.setElement(this.highlights[p.X, p.Y], "doublebluehighlight");
                        p = this.pathfindResults.Previous[p];
                    }
                }
            }
        }
    }

    private void scrollMap(Vector2 mousePosition)
    {
        var mX = mousePosition.x;
        var mY = mousePosition.y;
        var margin = GameConstants.MapScrollMargin;

        if ((mX >= 0 && mX < margin) || Input.GetKey(KeyCode.A))
        {
            if (this.x < 0)
            {
                this.x += GameConstants.MapScrollSpeed;
            }
        }

        if ((mX > (Futile.screen.width - margin) && mX <= Futile.screen.width) || Input.GetKey(KeyCode.D))
        {
            if (Math.Abs(this.x) + Futile.screen.width < this.map.Width)
            {
                this.x -= GameConstants.MapScrollSpeed;
            }
        }

        if ((mY > 0 && mY < margin) || Input.GetKey(KeyCode.S))
        {
            if (this.y < 0)
            {
                this.y += GameConstants.MapScrollSpeed;
            }
        }

        if ((mY > (Futile.screen.height - margin) && mY < Futile.screen.height) || Input.GetKey(KeyCode.W))
        {
            if (Math.Abs(this.y) + Futile.screen.height < this.map.Height)
            {
                this.y -= GameConstants.MapScrollSpeed;
            }
        }
    }

    #endregion
}