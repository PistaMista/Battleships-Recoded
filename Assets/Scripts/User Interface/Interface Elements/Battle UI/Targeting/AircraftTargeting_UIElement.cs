using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftTargeting_UIElement : UIElement
{
    static Player managedPlayer;
    static Player candidateAircraftTarget;
    static int candidateAircraftFlightTime = 0;
    static string candidateDirection = "NONE";

    public override void Enable ()
    {
        base.Enable();
        managedPlayer = UserInterface.managedBattle.activePlayer;
        flashTime = 0;
        UpdateSweepPreview();
    }

    public override void Disable ()
    {
        base.Disable();
        if (indicators != null)
        {
            foreach (GameObject indicator in indicators)
            {
                Destroy( indicator );
            }
        }

        indicators = null;
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
                int time = (int)Mathf.Clamp( distance * Master.vars.maximumAircraftFlightTime, 1, Master.vars.maximumAircraftFlightTime );
                if (candidateAircraftFlightTime != time)
                {
                    candidateAircraftFlightTime = time;

                    UpdateSweepPreview();
                }

                Debug.Log( "SELECTING FLIGHT TIME " + candidateAircraftFlightTime );
            }
        }
    }

    protected override void OnEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnEndPress( initialPosition, currentPosition );
        afterCancelLock = false;
        initialSelectionLock = false;
        UpdateSweepPreview();
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

    public float previewCycleTime;
    float flashTime;
    GameObject[] indicators;
    void UpdateSweepPreview ()
    {
        if (indicators != null)
        {
            foreach (GameObject indicator in indicators)
            {
                Destroy( indicator );
            }
            indicators = null;
        }

        if (candidateAircraftFlightTime > 0)
        {
            indicators = new GameObject[candidateAircraftFlightTime];

            Vector3 initialPosition = ( candidateDirection == "HORIZONTAL" ? Vector3.left : Vector3.back ) * candidateAircraftTarget.board.sideTileLength / 2;
            Vector3 movementStep = -initialPosition.normalized * ( candidateAircraftTarget.board.sideTileLength / (float)candidateAircraftFlightTime );

            for (int i = 0; i < indicators.Length; i++)
            {
                Vector3 position = initialPosition + movementStep * ( i + 1 ) + Vector3.up * 0.110f + candidateAircraftTarget.transform.position;
                Vector3 rotation = new Vector3( 90, 0, candidateDirection == "HORIZONTAL" ? 90 : 0 );
                Vector3 scale = new Vector3( candidateAircraftTarget.board.sideTileLength + 2, 0.15f, 1 );

                GameObject indicator = GameObject.CreatePrimitive( PrimitiveType.Quad );
                indicator.GetComponent<Renderer>().material = initialSelectionLock ? Master.vars.targetingUnconfirmedMaterial : Master.vars.targetingConfirmedMaterial;

                indicator.transform.SetParent( transform );
                indicator.transform.position = position;
                indicator.transform.rotation = Quaternion.Euler( rotation );
                indicator.transform.localScale = scale;

                indicators[i] = indicator;
            }
        }
    }

    protected override void Update ()
    {
        base.Update();
        if (indicators != null)
        {
            for (int i = 0; i < indicators.Length; i++)
            {
                float offset = i * ( previewCycleTime / indicators.Length );
                float opacity = Mathf.Sin( Mathf.PI * ( flashTime - offset ) / previewCycleTime );

                MaterialPropertyBlock block = new MaterialPropertyBlock();
                Color color = ( initialSelectionLock ? Master.vars.targetingUnconfirmedMaterial : Master.vars.targetingConfirmedMaterial ).GetColor( "_EmissionColor" ) * opacity;
                block.SetColor( "_EmissionColor", color );

                Renderer rnd = indicators[i].GetComponent<Renderer>();
                rnd.SetPropertyBlock( block );
            }

            flashTime += Time.deltaTime;
        }
    }
}
