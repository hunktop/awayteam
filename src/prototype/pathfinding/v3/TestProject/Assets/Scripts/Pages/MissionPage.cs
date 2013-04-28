using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionPage : GamePage, FSingleTouchableInterface
{
    #region Private Fields

    private Map map;
    private Actor selectedActor;
    private int tileSize;
    private PathfindResult pathfindResults;
    private List<Vector2i> highlightedIndicies;
    private FSprite blueHighlight;
    private FSprite redHighlight;
    private FSprite[,] highlights;
    private FButton moveButton;
    private FButton attackButton;
    private FButton cancelButton;
    private ButtonStrip buttonStrip;
    private List<Vector2i> highlightedPath;
    private Stack<ExtendedActorState> actorActionStack;

    #endregion

    #region Start

    public override void Start()
    {
        this.tileSize = 30;
        var width = 50;
        var height = 35;
        var tiles = new TileProperties[width, height];

        var rand = new System.Random();
        for (int ii = 0; ii < width; ii++)
        {
            for (int jj = 0; jj < height; jj++)
            {
                tiles[ii, jj] = (rand.NextDouble() < 0.7) ? StaticTiles.GrassTile : StaticTiles.ForestTiles;
            }
        }

        this.map = new Map(tiles, tileSize);
        this.map.AddActor(StaticActors.GoodSoldier, new Vector2i(5, 5));
        this.map.AddActor(StaticActors.EvilSoldier, new Vector2i(5, 7), true);
        this.map.Start();
        this.AddChild(this.map);

        this.moveButton = new FButton("move");
        this.moveButton.sprite.width = this.moveButton.sprite.height = 32;
        this.attackButton = new FButton("attack");
        this.attackButton.sprite.width = this.attackButton.sprite.height = 32;
        this.moveButton.SignalRelease += new Action<FButton>(moveButton_SignalRelease);
        this.attackButton.SignalRelease += new Action<FButton>(attackButton_SignalRelease);

        this.highlightedIndicies = new List<Vector2i>();
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

        this.actorActionStack = new Stack<ExtendedActorState>();
    }

    #endregion

    #region Update

    public override void  Update()
    {
        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        this.ScrollMap(mousePosition2d);

        if (this.selectedActor != null)
        {
            var actorState = this.selectedActor.TurnState;
            if (actorState == ActorState.SelectingDestination)
            {
                var globalCoords = MouseManager.ScreenToGlobal(mousePosition2d);
                var gridVector = this.map.GlobalToGrid(globalCoords);
                this.highlightPath(gridVector);
            }
            else if(actorState == ActorState.SelectingEnemy)
            {

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
                    this.ClearHighlights();

                    if (MoveAndAttackHelper.CanAttack(this.selectedActor, this.map))
                    {
                        var targets = MoveAndAttackHelper.GetAttackablePoints(this.map, gridVector, 1, 2);
                        foreach (var item in targets)
                        {
                            this.setElement(this.highlights[item.X, item.Y], "redhighlight");
                            this.highlights[item.X, item.Y].isVisible = true;
                            this.highlightedIndicies.Add(item);
                        }
                        this.showButtonStrip(this.selectedActor);
                    }
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
                foreach (var item in this.pathfindResults.VisitablePoints)
                {
                    this.setElement(this.highlights[item.X, item.Y], "bluehighlight");
                    this.highlights[item.X, item.Y].isVisible = true;
                    this.highlightedIndicies.Add(item);
                }
                foreach (var item in this.pathfindResults.AttackablePoints)
                {
                    this.setElement(this.highlights[item.X, item.Y], "redhighlight");
                    this.highlights[item.X, item.Y].isVisible = true;
                    this.highlightedIndicies.Add(item);
                }
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
        if (this.selectedActor != null 
            && this.selectedActor.TurnState == ActorState.AwaitingCommand 
            && !this.selectedActor.HasMovedThisTurn)
        {
            this.selectedActor.TurnState = ActorState.SelectingDestination;
            this.RemoveChild(this.buttonStrip);
        }
    }

    private void attackButton_SignalRelease(FButton b)
    {
        if (this.selectedActor != null)
        {
            Debug.Log("Attack button clicked.");
        }
    }

    #endregion

    #region Private Methods

    private void setElement(FSprite sprite, string name)
    {
        if (sprite.element.name != name)
        {
            sprite.element = Futile.atlasManager.GetElementWithName(name);
        }
    }

    private void ClearHighlights()
    {
        foreach (var index in this.highlightedIndicies)
        {
            this.highlights[index.X, index.Y].isVisible = false;
        }
        this.highlightedIndicies.Clear();
    }

    private void ClearSelection()
    {
        this.ClearHighlights();
        this.selectedActor = null;
    }

    private void showButtonStrip(Actor actor)
    {
        this.buttonStrip = new ButtonStrip();
        this.buttonStrip.Orientation = ButtonStripOrientation.Horizontal;
        this.buttonStrip.Spacing = 3.0f;

        if (actor != null && actor.TurnState == ActorState.AwaitingCommand)
        {
            Debug.Log(actor.HasMovedThisTurn);
            if (!actor.HasMovedThisTurn)
            {
                this.buttonStrip.AddButton(this.moveButton);
            }

            if(MoveAndAttackHelper.CanAttack(actor, this.map))
            {
                this.buttonStrip.AddButton(this.attackButton);
            }
            
            this.buttonStrip.isVisible = true;
            this.buttonStrip.x = actor.x + actor.width;
            this.buttonStrip.y = actor.y - actor.height;
            this.AddChild(this.buttonStrip);
        }
    }

    private void highlightPath(Vector2i dest)
    {
        if (this.selectedActor != null)
        {
            foreach(var item in this.highlightedPath)
            {
                this.highlights[item.X, item.Y].element = Futile.atlasManager.GetElementWithName("bluehighlight");
            }
            this.highlightedPath.Clear();

            if (this.pathfindResults.VisitablePoints.Contains(dest))
            {
                Vector2i p = dest;
                Vector2i origin;
                if(map.TryGetLocation(this.selectedActor, out origin))
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

    private void ScrollMap(Vector2 mousePosition)
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

    private void PushState(Actor actor, Map map)
    {
    }

    private void PopState(Actor actor, Map map)
    {
    }

    #endregion
}