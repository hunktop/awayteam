using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : AbilityController
{
    private List<Vector2i> attackableLocations;
    private FSprite crossHair = new FSprite("crosshair");

    #region Properties

    protected AttackAbility AttackAbility
    {
        get;
        set;
    }

    #endregion

    #region Ctor

    private static AttackController instance;

    private AttackController()
    {
    }

    public static AttackController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AttackController();
            }
            return instance;
        }
    }

    #endregion

    #region Public Methods

    public override void Activate(Ability ability)
    {
        base.Activate(ability);
        this.AttackAbility = ability as AttackAbility;
        if (this.AttackAbility == null)
        {
            throw new ArgumentException("Ability " + ability + " is not implemented by this controller.");
        }

        var weapon = this.SelectedActor.EquippedItem as WeaponProperties;
        this.attackableLocations = 
            this.AttackAbility.GetAttackableLocations(this.Map, this.SelectedActor, weapon)
            .ToList();
        
        this.highlightAttackableLocations();
        this.crossHair.isVisible = false;
        this.crossHair.width = AwayTeam.TileSize;
        this.crossHair.height = AwayTeam.TileSize;
        this.ParentController.MapContainer.AddChild(this.crossHair);
        this.ParentController.TouchEnded += onTouchEnded;
    }

    public override void Deactivate(bool turnOver, ActionCompleteReason reason)
    {
        this.ParentController.TouchEnded -= onTouchEnded;
        this.ParentController.ClearOverlay();
        Futile.instance.SignalUpdate -= Update;
        this.ParentController.MapContainer.RemoveChild(this.crossHair);
        base.Deactivate(turnOver, reason);
    }

    public override void Update()
    {

        var mousePosition2d = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var globalCoords = MouseManager.ScreenToGlobal(mousePosition2d);
        var gridCoords = this.Map.GlobalToGrid(globalCoords);
        if (this.attackableLocations.Contains(gridCoords))
        {
            this.crossHair.x = gridCoords.X * AwayTeam.TileSize + AwayTeam.TileSize / 2;
            this.crossHair.y = gridCoords.Y * AwayTeam.TileSize + AwayTeam.TileSize / 2;
            this.crossHair.isVisible = true;
        }
        else
        {
            this.crossHair.isVisible = false;
        }
        base.Update();
    }

    #endregion

    #region Touch Handler

    private void onTouchEnded(object sender, TouchEventArgs args)
    {
        var touch = args.Touch;
        var gridVector = this.Map.GlobalToGrid(touch.position);
        if (this.attackableLocations.Contains(gridVector))
        {
            this.ParentController.ClearOverlay();
            Futile.instance.SignalUpdate -= Update;

            var margs = new AttackArgs()
            {
                Actor = this.SelectedActor,
                TargetedLocation = gridVector,
                Map = this.Map
            };

            this.AttackAbility.AbilityEnded += abilityEnded;
            this.AttackAbility.StartExecute(margs);
        }
    }

    private void abilityEnded(object sender, AbilityCompleteEventArgs args)
    {
        if (args.Success)
        {
            this.Deactivate(this.AttackAbility.EndsTurn, ActionCompleteReason.Complete);
        }

        // TODO What if there's an error!?  Success == false?
    }

    #endregion

    #region Private Methods

    private void highlightAttackableLocations()
    {
        this.ParentController.ClearOverlay();
        foreach (var dest in this.attackableLocations)
        {
            this.ParentController.SetOverlay(dest.X, dest.Y, "redhighlight");
            this.ParentController.SetOverlayVisible(dest.X, dest.Y, true);
        }
    }

    #endregion
}
