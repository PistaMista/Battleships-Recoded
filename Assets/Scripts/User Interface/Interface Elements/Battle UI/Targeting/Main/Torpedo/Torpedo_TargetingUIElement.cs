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

    protected override void OnFocusableBeginPress ( Vector2 position )
    {
        base.OnFocusableBeginPress( position );
        OnFocusedBeginPress( position );

        if (selectedTargetMarker != null)
        {
            Focus();
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
        //if (selectedTargetMarker != null)
        //{
        //    if (selectedTargetMarker.ghost == null && selectedTargetMarker.potentialTarget.valid == false)
        //    {
        //        RemoveTargetMarker( selectedTargetMarker );
        //    }
        //}
        base.OnFocusedEndPress( initialPosition, currentPosition );
        Unfocus();
    }

    protected override Target CalculateTargetFromScreenPosition ( Vector2 position )
    {
        Vector3 directionThreeDimensional = ( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y ) - UserInterface.managedBattle.torpedoLaunchPosition ).normalized;

        Vector2 direction = new Vector2( directionThreeDimensional.x, directionThreeDimensional.z );
        float angle = Vector2.Angle( Vector2.up, direction ) * Mathf.Sign( direction.x );

        Vector2 launchPoint = Camera.main.WorldToScreenPoint( UserInterface.managedBattle.torpedoLaunchPosition );
        Vector2[] corners = { Vector2.zero, Vector2.up * Screen.height, new Vector2( Screen.width, Screen.height ), Vector2.right * Screen.width };
        Vector2[] cornerDirections = { ( corners[0] - launchPoint ).normalized, ( corners[1] - launchPoint ).normalized, ( corners[2] - launchPoint ).normalized, ( corners[3] - launchPoint ).normalized };
        Vector2[] points = new Vector2[2];

        for (int i = 0; i < 2; i++)
        {
            if (direction.x > cornerDirections[0].x && direction.x < cornerDirections[3].x)
            {
                Vector2 chosenCoordinate = Vector2.up;
                Vector2 absolutePartial = Vector2.up * corners[1 - i].y;
                if (i == 0)
                {
                    if (direction.x < cornerDirections[1].x)
                    {
                        absolutePartial = Vector2.right * corners[0].x;
                        chosenCoordinate = Vector2.right;
                    }
                    else if (direction.x > cornerDirections[2].x)
                    {
                        absolutePartial = Vector2.right * corners[3].x;
                        chosenCoordinate = Vector2.right;
                    }
                }

                Vector2 relativePartial = Vector2.Scale( ( absolutePartial - launchPoint ), chosenCoordinate );
                float directionNormalizationModifier = 1.0f / ( Vector2.Scale( direction, chosenCoordinate ).magnitude );

                Vector2 relativeComplete = relativePartial.magnitude * direction * directionNormalizationModifier;
                Vector2 absoluteComplete = relativeComplete + launchPoint;

                points[i] = absoluteComplete;

            }
        }

        float totalDistance = Vector2.Distance( points[0], points[1] );
        float extendedDistance = Vector2.Distance( position, points[1] );

        int torpedoCount = Mathf.CeilToInt( Mathf.Clamp01( extendedDistance / totalDistance ) * UserInterface.managedBattle.activePlayer.destroyer.torpedoes );

        Debug.Log( "Torpedo count: " + torpedoCount );
        TorpedoTarget target = new TorpedoTarget( directionThreeDimensional, torpedoCount );
        return new Target( target, UserInterface.managedBattle.activePlayer.destroyer.CheckLineOfFire( angle ) );
    }

    protected override TargetMarker AddTargetMarker ( Target target )
    {
        TorpedoMarkLine_TargetMarker marker = new GameObject( "Torpedo Marker" ).AddComponent<TorpedoMarkLine_TargetMarker>();
        return marker;
    }


    protected override void ConfirmTargeting ()
    {
        base.ConfirmTargeting();
        TorpedoTarget target = (TorpedoTarget)targetMarkers[0].target.target;
        UserInterface.managedBattle.TorpedoAttack( target.direction, target.torpedoCount );
    }
}
