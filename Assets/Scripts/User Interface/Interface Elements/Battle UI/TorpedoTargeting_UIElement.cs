using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoTargeting_UIElement : UIElement
{
    public static Vector3 candidate;
    public static int candidateTorpedoCount;
    GameObject indicator;
    bool confirmed;
    float delay;
    float tileHighlightRefreshAngle;

    public override void Enable ()
    {
        base.Enable();
        confirmed = false;
        candidate = Vector3.zero;
        canFire = true;
        delay = 0;

        Vector3 measurementPosition = UserInterface.managedBattle.selectedPlayer.transform.position + Vector3.forward * Mathf.Sqrt( UserInterface.managedBattle.selectedPlayer.board.tiles.Length );
        measurementPosition.x = 0.5f;

        tileHighlightRefreshAngle = Mathf.Atan2( measurementPosition.x, measurementPosition.z ) * Mathf.Rad2Deg;
    }

    public override void Disable ()
    {
        base.Disable();
        Destroy( indicator );
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

    protected override void Update ()
    {
        base.Update();
        if (delay > 0)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                UserInterface.managedBattle.TorpedoAttack( candidate, candidateTorpedoCount );
            }
        }
    }

    bool canFire = true;
    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
        if (!UserInterface.managedBattle.activePlayer.destroyer.destroyed && UserInterface.managedBattle.activePlayer.destroyer.torpedoes >= 1 && ArtilleryTargeting_UIElement.candidate == null)
        {
            candidate = ( InputController.ConvertToWorldPoint( currentPosition, Camera.main.transform.position.y ) - UserInterface.managedBattle.torpedoLaunchPosition ).normalized;
            candidate.y = 0;

            if (indicator == null)
            {
                CreateIndicator( canFire ? Master.vars.targetingUnconfirmedMaterial : Master.vars.targetingFailedMaterial );
            }

            bool lineOfFireFree = UserInterface.managedBattle.activePlayer.destroyer.CheckLineOfFire( Mathf.Atan2( candidate.x, candidate.z ) * Mathf.Rad2Deg );
            if (lineOfFireFree != canFire)
            {
                canFire = lineOfFireFree;
                CreateIndicator( canFire ? Master.vars.targetingUnconfirmedMaterial : Master.vars.targetingFailedMaterial );
            }

            indicator.transform.rotation = Quaternion.LookRotation( candidate );

            candidateTorpedoCount = Mathf.CeilToInt( ( currentPosition.y / Screen.height ) * Mathf.Floor( UserInterface.managedBattle.activePlayer.destroyer.torpedoes ) );
            Debug.Log( candidateTorpedoCount + " torpedoes will fire." );
        }
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        if (indicator != null && !confirmed)
        {
            Vector3 worldPos = InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y );

            Vector3 processed = indicator.transform.InverseTransformPoint( worldPos );

            if (Mathf.Abs( processed.x ) < 1 && canFire)
            {
                confirmed = true;
                delay = 1;
                InputController.onDrag -= OnDrag;
                Destroy( indicator );
                CreateIndicator( Master.vars.targetingConfirmedMaterial );
                indicator.transform.rotation = Quaternion.LookRotation( candidate );
            }
            else
            {
                candidate = Vector3.zero;
                Destroy( indicator );
            }
        }
    }

    void CreateIndicator ( Material material )
    {
        Destroy( indicator );

        indicator = new GameObject( "Torpedo Targeting Indicator" );
        indicator.transform.SetParent( gameObject.transform );
        indicator.transform.position = UserInterface.managedBattle.torpedoLaunchPosition + Vector3.up * 0.111f;

        DynamicStripedRectangle_GraphicsElement line = new GameObject( "Line" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
        line.transform.SetParent( indicator.transform );
        float lineLength = 50f;

        line.material = material;
        line.transform.localPosition = Vector3.forward * lineLength / 2f;
        line.Set( new Vector2( 0.8f, lineLength ), 0.25f, true, 0, 0.3f, 0.1f, 0.1f );
    }
}
