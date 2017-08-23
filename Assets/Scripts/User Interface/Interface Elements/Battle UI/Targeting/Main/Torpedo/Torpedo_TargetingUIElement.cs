using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;

public class Torpedo_TargetingUIElement : TargetingUIElement
{
    public struct TorpedoTarget
    {
        public Vector3 direction;
        public int torpedoCount;
        public TorpedoTarget ( Vector3 direction, int torpedoCount )
        {
            this.direction = direction.normalized;
            this.torpedoCount = torpedoCount;
        }
    }

    protected override void OnFocusableDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusableDrag( initialPosition, currentPosition );
        if (targetMarkers.Count == 0 && !UserInterface.managedBattle.activePlayer.destroyer.destroyed && UserInterface.managedBattle.activePlayer.destroyer.torpedoes >= 1)
        {
            Focus();
        }
    }

    protected override void OnFocusedDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedDrag( initialPosition, currentPosition );
        if (selectedTargetMarker == null)
        {
            AddTarget( CalculateTargetFromScreenPosition( currentPosition ) );
            selectedTargetMarker = targetMarkers[targetMarkers.Count - 1];
        }
    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
        Unfocus();
    }

    protected override object CalculateTargetFromScreenPosition ( Vector2 position )
    {
        TorpedoTarget target = new TorpedoTarget( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y ) - UserInterface.managedBattle.torpedoLaunchPosition, 1 );
        return target;
    }


    protected override TargetMarker AddTargetMarker ( object target )
    {
        TorpedoMarkLine_TargetMarker marker = new GameObject( "Torpedo Marker" ).AddComponent<TorpedoMarkLine_TargetMarker>();
        return marker;
    }


    protected override void ConfirmTargeting ()
    {
        base.ConfirmTargeting();
        TorpedoTarget target = (TorpedoTarget)targetMarkers[0].target;
        UserInterface.managedBattle.TorpedoAttack( target.direction, target.torpedoCount );
    }
}
