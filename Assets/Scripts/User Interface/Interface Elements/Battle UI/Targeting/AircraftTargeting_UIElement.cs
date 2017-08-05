using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftTargeting_UIElement : UIElement
{
    static Player managedPlayer;
    static Player candidateAircraftTarget;
    static int candidateAircraftFlightTime = 10;
    static string candidateDirection = "NONE";

    public override void Enable ()
    {
        base.Enable();
        managedPlayer = UserInterface.managedBattle.activePlayer;
    }

    public override void Disable ()
    {
        base.Disable();
    }

    public static bool afterCancelLock;
    public static bool initialSelectionLock;

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
        //Vector3 worldPosition = InputController.ConvertToWorldPoint( currentPosition, Camera.main.transform.position.y - 1 );
        //foreach (Player player in UserInterface.managedBattle.combatants)
        //{
        //    Bounds tagBounds = ( (TacticalView_BoardVisualModule)player.board.visualModules[1] ).textBounds;
        //    Vector3 lowerCorner = player.board.transform.position - tagBounds.extents;
        //    Vector3 upperCorner = player.board.transform.position + tagBounds.extents;

        //    if (worldPosition.x > lowerCorner.x && worldPosition.x < upperCorner.x && worldPosition.z > lowerCorner.z && worldPosition.z < upperCorner.z)
        //    {
        //        candidateAircraftTarget = player;
        //        break;
        //    }
        //}


        if (InputController.screenInputPoints == 2 && !afterCancelLock && TorpedoTargeting_UIElement.candidate == Vector3.zero)
        {
            Vector2 relativePosition = currentPosition - initialPosition;
            string direction = Mathf.Abs( relativePosition.x ) > Mathf.Abs( relativePosition.y ) ? "HORIZONTAL" : "VERTICAL";
            if (!initialSelectionLock)
            {
                if (direction == candidateDirection)
                {
                    Debug.Log( "CANCELED " + candidateDirection );

                    afterCancelLock = true;
                    candidateAircraftTarget = null;
                    candidateAircraftFlightTime = 0;
                    candidateDirection = "NONE";
                    UpdateSweepPreview();
                }
                else
                {
                    Debug.Log( "SELECTED " + direction );

                    initialSelectionLock = true;
                    candidateAircraftTarget = UserInterface.managedBattle.selectedPlayer;
                    candidateDirection = direction;
                }
            }
            else
            {
                float distance = Vector2.Distance( initialPosition, currentPosition ) / ( direction == "HORIZONTAL" ? Screen.width : Screen.height ) * 1.25f;
                int time = (int)( distance * Master.vars.maximumAircraftFlightTime );
                if (candidateAircraftFlightTime != time)
                {
                    UpdateSweepPreview();
                }
                candidateAircraftFlightTime = time;

                Debug.Log( "SELECTING FLIGHT TIME " + candidateAircraftFlightTime );
            }
        }
    }

    protected override void OnEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnEndPress( initialPosition, currentPosition );
        afterCancelLock = false;
        initialSelectionLock = false;
    }

    public override void OnBattleChange ()
    {
        base.OnBattleChange();
        if (UserInterface.managedBattle.selectedPlayer != null && !gameObject.activeInHierarchy && !UserInterface.managedBattle.activePlayer.AI)
        {
            Enable();
        }
        else if (gameObject.activeInHierarchy)
        {
            Disable();
        }
    }

    public static void FinalizeTargeting ()
    {
        if (!managedPlayer.aircraftCarrier.destroyed && candidateAircraftTarget != null)
        {
            managedPlayer.aircraftCarrier.SendAircraftToPlayer( candidateAircraftTarget, candidateAircraftFlightTime, candidateDirection );
        }

        candidateAircraftTarget = null;
        candidateAircraftFlightTime = 0;
        candidateDirection = "NONE";
    }

    void UpdateSweepPreview ()
    {

    }
}
