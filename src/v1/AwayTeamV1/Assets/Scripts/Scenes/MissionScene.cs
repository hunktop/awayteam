using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Event Args

public class TouchEventArgs : EventArgs 
{
    public FTouch Touch
    {
        get;
        set;
    }
}

#endregion

#region Turn State

public enum TurnState
{
    TurnBegin,
    TurnMiddle,
    TurnEnd
}

#endregion

/// <summary>
/// This class implements the "Mission" scene, where the turned based strategy stuff happens.
/// Still under HEAVY development.
/// </summary>
public class MissionScene : GameScene, FSingleTouchableInterface
{
    #region Events
    
    public event EventHandler<TouchEventArgs> TouchEnded;

    #endregion

    #region Private Fields

    private MapGenerator mapGenerator;

    // Some book-keeping lists/sets
    private HashSet<Vector2i> visibleOverlayIndices;

    private FContainer mapContainer;
    private FContainer hud;
    private FSprite[,] overlay;
    private FLabel turnLabel;
    private ButtonStrip buttonStrip;
	private ScriptableObject interfaceManager;
    private TurnState turnState;
    private bool started;

    private List<Team> teams = new List<Team>();
    private int teamIndex;
    private int actorsProcessedThisTurn;
    private int teamActorCount;
    
    #endregion

    #region Public Properties

    public FContainer MapContainer
    {
        get
        {
            return this.mapContainer;
        }
    }

    public FContainer HUD
    {
        get
        {
            return this.hud;
        }
    }

    public Actor SelectedActor
    {
        get;
        private set;
    }

    public Map Map
    {
        get;
        private set;
    }

    public Team CurrentTeam
    {
        get
        {
            return this.teams[this.teamIndex];
        }
    }
    
    #endregion

    #region Start

   /// <summary>
    /// Initialize resources.  Right now, for testing purposes, a simple random map is 
    /// generated here with a few actors in it.
    /// </summary>
    public override void Start()
    {
        var width = 50;
        var height = 35;
		var mapSeed = "blah blah";
        var tiles = new TileProperties[width, height];

        this.mapContainer = new FContainer();
        this.hud = new FContainer();

		// This handles GUI and stuff
		//this.interfaceManager = new InterfaceManager();
		//this.interfaceManager = ScriptableObject.CreateInstance("InterfaceManager");;
		
        // Generate a very simple random map tiles
		this.mapGenerator = new MapGenerator();
		tiles = mapGenerator.GenerateMap(width, height, 10);
        
        /*
        var rand = new System.Random();
        for (int ii = 0; ii < width; ii++)
        {
            for (int jj = 0; jj < height; jj++)
            {
                tiles[ii, jj] = (rand.NextDouble() < 0.7) ? StaticTiles.GrassTile : StaticTiles.ForestTile;
            }
        }
        */
        this.Map = new Map(tiles);
        int i = 5;
        int j = 5;
        foreach (var team in this.teams)
        {
            var actorList = new List<Actor>();
            foreach (var member in team.Members)
            {
                var actor = new Actor(member, team);
                actorList.Add(actor);
                this.Map.AddActor(actor, new Vector2i(i++, j++));
            }
        }

        
        this.Map.Start();
        this.mapContainer.AddChild(this.Map);

        // Initialize overlay
        this.visibleOverlayIndices = new HashSet<Vector2i>();
        this.overlay = new FSprite[Map.Columns, Map.Rows];
        for (int ii = 0; ii < Map.Columns; ii++)
        {
            for (int jj = 0; jj < Map.Rows; jj++)
            {
                var overlaySprite = new FSprite("bluehighlight");
                overlaySprite.x = ii * AwayTeam.TileSize + AwayTeam.TileSize / 2;
                overlaySprite.y = jj * AwayTeam.TileSize + AwayTeam.TileSize / 2;
                overlaySprite.isVisible = false;
                overlaySprite.width = overlaySprite.height = AwayTeam.TileSize;
                this.overlay[ii, jj] = overlaySprite;
                this.mapContainer.AddChild(overlaySprite);
            }
        }

        this.AddChild(this.mapContainer);
        this.AddChild(this.hud);
        this.teamIndex = 0;
        this.turnState = TurnState.TurnBegin;
        this.started = true;
    }

    #endregion

    #region Update

    /// <summary>
    /// Figure out which part of the turn we're in and act accordinly.
    /// </summary>
    public override void  Update()
    {
        switch (this.turnState)
        {
            case TurnState.TurnBegin:
                this.TurnBegin();
                break;
            case TurnState.TurnMiddle:
                this.TurnMiddle();
                break;
            case TurnState.TurnEnd:
                this.TurnEnd();
                break;
        }

        base.Update();
    }

    
    private void TurnBegin()
    {
        var curTeam = this.CurrentTeam;
        Debug.Log("[[Team Turn Begin]] Team: " + curTeam.Name);

        if (this.turnLabel == null)
        {
            this.turnLabel = new FLabel("courier", "Current Team: " + curTeam.Name);
            this.turnLabel.x = 250;
            this.turnLabel.y = 50;
            this.turnLabel.color = Color.black;
            this.hud.AddChild(this.turnLabel);
        }
        else
        {
            this.turnLabel.text = "Current Team: " + curTeam.Name;
        }

        this.teamActorCount = curTeam.Members.Count;
        this.actorsProcessedThisTurn = 0;
        foreach (var actor in this.Map.Actors.Where(a => a.Team == curTeam))
        {
            actor.TurnState = ActorState.CommandsAvailable;
            foreach (var ability in actor.Properties.AllAbilities)
            {
                ability.DecrementCooldown();
            }
        }

        this.turnState = TurnState.TurnMiddle;
    }

    private void TurnMiddle()
    {
        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var globalCoords = MouseManager.ScreenToGlobal(mousePosition2d);
        var gridVector = this.Map.GlobalToGrid(globalCoords);
        this.scrollMap(mousePosition2d);

        if (this.SelectedActor != null)
        {
            var actorState = this.SelectedActor.TurnState;

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (actorState == ActorState.AwaitingCommand)
                {
                    this.SelectedActor.TurnState = ActorState.CommandsAvailable;
                    this.clearSelection();
                }
            }
        }

        if (this.TurnOver())
        {
            this.turnState = TurnState.TurnEnd;
        }
    }

    private void TurnEnd()
    {
        Debug.Log("[[Team Turn End]] Team: " +  this.CurrentTeam.Name);
        this.teamIndex = (this.teamIndex + 1) % this.teams.Count;
        this.turnState = TurnState.TurnBegin;
    }

    private bool TurnOver()
    {
        return this.actorsProcessedThisTurn >= this.CurrentTeam.Members.Count(x => x.IsAlive);
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

    #region Public Methods

    public void AddTeam(Team team)
    {
        // TODO Do something if the map is alreadt running, probably throw exception
        this.teams.Add(team);
    }

    public void SetOverlay(int x, int y, string sprite)
    {
        this.overlay[x, y].SetElementByName(sprite);
    }

    public void SetOverlayVisible(int x, int y, bool visible)
    {
        this.overlay[x, y].isVisible = visible;

        var vector = new Vector2i(x,y);
        if (visible)
        {
            this.visibleOverlayIndices.Add(vector);
        }
        else
        {
            if (this.visibleOverlayIndices.Contains(vector))
            {
                this.visibleOverlayIndices.Remove(vector);
            }
        }
    }

    public void ClearOverlay()
    {
        foreach (var index in this.visibleOverlayIndices)
        {
            this.overlay[index.X, index.Y].isVisible = false;
        }
        this.visibleOverlayIndices.Clear();
    }

    public void RemoveActor(Actor a)
    {
        this.Map.RemoveActor(a);
    }

    #endregion

    #region Touch Handlers

    public bool HandleSingleTouchBegan(FTouch touch)
    {
        if (this.SelectedActor != null && this.SelectedActor.TurnState == ActorState.AwaitingCommand)
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
        var handler = this.TouchEnded;
        if (handler != null)
        {
            handler(this, new TouchEventArgs() { Touch = touch });
        }

        var gridVector = this.Map.GlobalToGrid(touch.position);
        var curTeam = this.CurrentTeam;
        Actor tempActor;

        Debug.Log("[[Touch Detected]]: World Coordinates: " + touch.position + ", Map Coordinates: " + gridVector);
        if (this.SelectedActor == null)
        {
            if (this.Map.TryGetActor(gridVector, out tempActor))
            {
                if (tempActor.Team == curTeam && tempActor.TurnState == ActorState.CommandsAvailable)
                {
                    this.SelectedActor = tempActor;
                    this.SelectedActor.TurnState = ActorState.AwaitingCommand;
                    Debug.Log("[[Actor Turn Start]]: Actor " + this.SelectedActor); 
                    this.showButtonStrip(this.SelectedActor);
                }
            }
        }
    }

    public void HandleSingleTouchCanceled(FTouch touch)
    {
    }

    #endregion 

    #region Button Presses

    private void abilityButtonPressed(FButton b)
    {
        Debug.Log("[[Ability Button Pressed]]: Actor " + this.SelectedActor + ", Ability " + b.data);
        var ability = b.data as Ability;
        var controller = ability.GetController();
        controller.ActionComplete += actionComplete;
        this.mapContainer.RemoveChild(this.buttonStrip);
        this.buttonStrip = null;
        this.SelectedActor.TurnState = ActorState.ExecutingCommand;
        controller.Activate(ability);
    }

    private void actionComplete(object sender, ActionCompleteEventArgs args)
    {
        if (args.TurnOver)
        {
            Debug.Log("[[Actor Turn Over]]: Actor " + this.SelectedActor);
            this.SelectedActor.TurnState = ActorState.TurnOver;
            this.clearSelection();
            this.actorsProcessedThisTurn++;
        }
        else
        {
            this.SelectedActor.TurnState = ActorState.AwaitingCommand;
            this.showButtonStrip(this.SelectedActor);
        }

        var controller = sender as AbilityController;
        controller.ActionComplete -= actionComplete;
    }

    #endregion

    #region Private Methods

    private void clearSelection()
    {
        this.ClearOverlay();
        this.SelectedActor = null;
        if (this.buttonStrip != null && this._childNodes.Contains(this.buttonStrip))
        {
            this.mapContainer.RemoveChild(this.buttonStrip);
        }
    }

    private void showButtonStrip(Actor actor)
    {
        this.buttonStrip = new ButtonStrip();
        this.buttonStrip.Orientation = ButtonStripOrientation.Horizontal;
        this.buttonStrip.Spacing = 3.0f;

        if (actor != null && actor.TurnState == ActorState.AwaitingCommand)
        {
            foreach (var ability in actor.Properties.AvailableAbilities)
            {
                var button = new FButton(ability.IconName);
                button.sprite.width = button.sprite.height = 32;
                button.data = ability;
                button.SignalRelease += new Action<FButton>(abilityButtonPressed);
                this.buttonStrip.AddButton(button);
            }
        }

        this.buttonStrip.x = actor.x + actor.width;
        this.buttonStrip.y = actor.y - actor.height;
        this.mapContainer.AddChild(this.buttonStrip);
    }
        
    private void scrollMap(Vector2 mousePosition)
    {
        var mX = mousePosition.x;
        var mY = mousePosition.y;
        var margin = AwayTeam.MapScrollMargin;

        if ((mX >= 0 && mX < margin) || Input.GetKey(KeyCode.A))
        {
            if (this.mapContainer.x < 0)
            {
                this.mapContainer.x += AwayTeam.MapScrollSpeed;
            }
        }

        if ((mX > (Futile.screen.width - margin) && mX <= Futile.screen.width) || Input.GetKey(KeyCode.D))
        {
            if (Math.Abs(this.mapContainer.x) + Futile.screen.width < this.Map.Width)
            {
                this.mapContainer.x -= AwayTeam.MapScrollSpeed;
            }
        }

        if ((mY > 0 && mY < margin) || Input.GetKey(KeyCode.S))
        {
            if (this.mapContainer.y < 0)
            {
                this.mapContainer.y += AwayTeam.MapScrollSpeed;
            }
        }

        if ((mY > (Futile.screen.height - margin) && mY < Futile.screen.height) || Input.GetKey(KeyCode.W))
        {
            if (Math.Abs(this.mapContainer.y) + Futile.screen.height < this.Map.Height)
            {
                this.mapContainer.y -= AwayTeam.MapScrollSpeed;
            }
        }
    }

    #endregion
}