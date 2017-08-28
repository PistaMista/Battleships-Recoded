using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft_FocusableUIElement : AttackViewUIElement
{
    static Player managedPlayer;
    static Player candidateAircraftTarget;
    static int candidateAircraftFlightTime = 0;
    static Vector3 candidateDirection;

    public override void Enable ()
    {
        base.Enable();
        managedPlayer = UserInterface.managedBattle.activePlayer;
    }

    public override void Disable ()
    {
        base.Disable();
    }

    protected override void OnFocusableDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusableDrag( initialPosition, currentPosition );
        if (InputController.screenInputPoints == 2 && !managedPlayer.aircraftCarrier.destroyed && managedPlayer.aircraftCarrier.flightTime == 0)
        {
            Focus();
        }
    }

    bool directionSelected;
    bool directionCanceled;
    protected override void OnFocusedDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedDrag( initialPosition, currentPosition );

    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
        directionCanceled = false;
        Unfocus();
    }

    public static void FinalizeTargeting ()
    {
        if (managedPlayer != null)
        {
            if (!managedPlayer.aircraftCarrier.destroyed && candidateAircraftTarget != null)
            {
                managedPlayer.aircraftCarrier.SendAircraftToPlayer( candidateAircraftTarget, candidateAircraftFlightTime, candidateDirection );
            }

            candidateAircraftTarget = null;
            candidateAircraftFlightTime = 0;
            candidateDirection = Vector3.zero;
        }
    }
}
