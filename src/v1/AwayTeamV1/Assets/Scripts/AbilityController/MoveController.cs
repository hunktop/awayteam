using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveController : AbilityController
{
    #region Private Fields

    private List<Vector2i> highlightedPath = new List<Vector2i>();
    private Vector2i previousMouseOverPoint;
    private PathfindResult pathfinding;

    #endregion 

    #region Properties

    private MoveAbility MoveAbility
    {
        get;
        set;
    }

    #endregion 

    #region Ctor

    private static MoveController instance;

    private MoveController() 
    { 
    }

    public static MoveController Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new MoveController();
            }
            return instance;
        }
    }

    #endregion

    #region Public Methods

    public override void Activate(Ability ability)
    {
        base.Activate(ability);
        this.MoveAbility = ability as BasicMoveAbility;
        if (this.MoveAbility == null)
        {
            throw new ArgumentException("Ability " + ability + " is not implemented by this controller.");
        }

        this.pathfinding = this.MoveAbility.GetDestinations(ParentController.Map, ParentController.SelectedActor);
        this.highlightDesinations();
        this.ParentController.TouchEnded += onTouchEnded;
    }

    public override void Deactivate(bool turnOver, ActionCompleteReason reason)
    {
        this.ParentController.TouchEnded -= onTouchEnded;
        this.ParentController.ClearOverlay();
        Futile.instance.SignalUpdate -= Update;
        base.Deactivate(turnOver, reason);
    }

    public override void Update()
    {
        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var globalCoords = MouseManager.ScreenToGlobal(mousePosition2d);
        var gridVector = this.Map.GlobalToGrid(globalCoords);
        if (gridVector != this.previousMouseOverPoint)
        {
            this.highlightPath(gridVector);
        }
        this.previousMouseOverPoint = gridVector;
        base.Update();
    }

    #endregion

    #region Touch Handler

    private void onTouchEnded(object sender, TouchEventArgs args)
    {
        var touch = args.Touch;
        var gridVector = this.Map.GlobalToGrid(touch.position);
        if (this.pathfinding.VisitablePoints.Contains(gridVector))
        {
            this.ParentController.ClearOverlay();
            Futile.instance.SignalUpdate -= Update;

            var margs = new MovementArgs()
            {
                Actor = this.SelectedActor,
                Destination = gridVector,
                Map = this.Map,
                PrecomputedPathfinding = this.pathfinding
            };

            this.MoveAbility.AbilityEnded += abilityEnded;
            this.MoveAbility.StartExecute(margs);
        }
    }

    private void abilityEnded(object sender, AbilityCompleteEventArgs args)
    {
        if (args.Success)
        {
            this.Deactivate(this.MoveAbility.EndsTurn, ActionCompleteReason.Complete);
        }

        // TODO What if there's an error!?  Success == false
    }  

    #endregion

    #region Private Methods

    private void highlightDesinations()
    {
        this.ParentController.ClearOverlay();
        foreach (var dest in this.pathfinding.VisitablePoints)
        {
            this.ParentController.SetOverlay(dest.X, dest.Y, "bluehighlight");
            this.ParentController.SetOverlayVisible(dest.X, dest.Y, true);
        }
    }

    private void clearHighlightedPath()
    {
        foreach (var item in this.highlightedPath)
        {
            this.ParentController.SetOverlay(item.X, item.Y, "bluehighlight");
        }
        this.highlightedPath.Clear();
    }

    private void highlightPath(Vector2i dest)
    {
        this.clearHighlightedPath();
        if (this.pathfinding.VisitablePoints.Contains(dest))
        {
            Vector2i p = dest;
            Vector2i origin = this.SelectedActor.GridPosition;

            while (p != origin)
            {
                this.highlightedPath.Add(p);
                this.ParentController.SetOverlay(p.X, p.Y, "doublebluehighlight");
                p = this.pathfinding.Previous[p];
            }
        }
    }

    #endregion
}
