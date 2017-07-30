using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoTargeting_UIElement : UIElement
{
    public static Vector3 candidate;
    GameObject indicator;
    bool confirmed;
    float delay;
    float tileHighlightRefreshAngle;
    float lastAngle;

    public override void Enable ()
    {
        base.Enable();
        confirmed = false;
        candidate = Vector3.zero;
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
                UserInterface.managedBattle.TorpedoAttack( candidate );
            }
        }
    }

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
        if (!UserInterface.managedBattle.activePlayer.destroyer.destroyed && UserInterface.managedBattle.activePlayer.destroyer.torpedoes >= 1)
        {
            candidate = ( InputController.ConvertToWorldPoint( currentPosition, Camera.main.transform.position.y ) - UserInterface.managedBattle.torpedoLaunchPosition ).normalized;
            candidate.y = 0;

            if (indicator == null)
            {
                CreateIndicator( Master.vars.targetingUnconfirmedMaterial );
            }

            indicator.transform.rotation = Quaternion.LookRotation( candidate );
        }
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        if (indicator != null && !confirmed)
        {
            Vector3 worldPos = InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y );

            Vector3 processed = indicator.transform.InverseTransformPoint( worldPos );

            if (Mathf.Abs( processed.x ) < 1)
            {
                confirmed = true;
                delay = 1;
                InputController.onDrag -= OnDrag;
                Destroy( indicator );
                CreateIndicator( Master.vars.targetingConfirmedMaterial );
                indicator.transform.rotation = Quaternion.LookRotation( candidate );
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
        float lineLength = 25f;

        line.material = material;
        line.transform.localPosition = Vector3.forward * lineLength / 2f;
        line.Set( new Vector2( 0.8f, lineLength ), 0.25f, true, 0, 0.3f, 0.1f, 0.1f );
    }
}
