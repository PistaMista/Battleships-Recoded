using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtilleryTargeting_UIElement : UIElement
{
    public override void Enable ()
    {
        base.Enable();
        confirmed = false;
        candidate = null;
        delay = 0;
    }

    public override void Disable ()
    {
        base.Disable();
        if (indicator != null)
        {
            Destroy( indicator.gameObject );
        }
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
                UserInterface.managedBattle.ArtilleryAttack( candidate );
            }
        }
    }

    public BoardTile candidate;
    DynamicStripedRectangle_GraphicsElement indicator;
    bool confirmed;
    float delay;

    protected override void OnTap ( Vector2 position )
    {
        if (!confirmed)
        {
            if (UserInterface.managedBattle.selectedPlayer != null)
            {
                base.OnTap( position );
                BoardTile tile = UserInterface.managedBattle.selectedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 0.1f ) );
                if (tile != null)
                {
                    if (tile == candidate)
                    {
                        confirmed = true;
                        delay = 1;
                        CreateIndicator( Master.vars.targetingConfirmedMaterial );
                        indicator.transform.position = tile.transform.position + Vector3.up * 0.111f;
                    }
                    else if (!tile.hitBy.Contains( UserInterface.managedBattle.activePlayer ))
                    {
                        candidate = tile;
                        CreateIndicator( Master.vars.targetingUnconfirmedMaterial );
                        indicator.transform.position = tile.transform.position + Vector3.up * 0.111f;
                    }
                }
                else
                {
                    Destroy( indicator.gameObject );
                    candidate = null;
                }
            }
        }
    }

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
    }

    void CreateIndicator ( Material material )
    {
        if (indicator != null)
        {
            Destroy( indicator.gameObject );
        }
        indicator = new GameObject( "Candidate Target Indicator" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
        indicator.transform.SetParent( transform );
        indicator.material = material;
        indicator.Set( Vector2.one * 0.9f, 0.05f, true, 0.3f, 0.08f, 0.05f );
    }
}
