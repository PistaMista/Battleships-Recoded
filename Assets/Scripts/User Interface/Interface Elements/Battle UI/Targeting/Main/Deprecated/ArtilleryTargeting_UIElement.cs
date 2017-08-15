using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtilleryTargeting_UIElement : UIElement
{
    void Start ()
    {
        candidates = new List<BoardTile>();
    }

    public override void Enable ()
    {
        base.Enable();
        confirmed = false;
        candidates = new List<BoardTile>();
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
                UserInterface.managedBattle.ArtilleryAttack( candidates.ToArray() );
            }
        }
    }

    public static List<BoardTile> candidates;
    GameObject indicator;
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
                    if (candidates.Contains( tile ))
                    {
                        confirmed = true;
                        delay = 1;
                        indicator.transform.position = tile.transform.position + Vector3.up * 0.111f;

                        UpdateIndicators();
                    }
                    else if (!( tile.hitBy.Contains( UserInterface.managedBattle.activePlayer ) || ( UserInterface.managedBattle.activePlayer == tile.board.owner && tile.hitBy.Count > 0 ) ) && TorpedoTargeting_UIElement.candidate == Vector3.zero)
                    {
                        if (candidates.Count < UserInterface.managedBattle.activePlayer.shotCapacity)
                        {
                            candidates.Add( tile );
                        }
                        UpdateIndicators();
                    }
                }
                else
                {
                    if (candidates.Count > 0)
                    {
                        candidates.RemoveAt( candidates.Count - 1 );
                        UpdateIndicators();
                    }
                }
            }
        }
    }

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
    }

    void UpdateIndicators ()
    {
        Destroy( indicator );

        if (candidates.Count > 0)
        {
            indicator = new GameObject( "Artillery Targeting Indicator" );
            foreach (BoardTile tile in candidates)
            {
                DynamicStripedRectangle_GraphicsElement tmp = new GameObject( "Candidate Target Indicator" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
                tmp.transform.SetParent( indicator.transform );
                tmp.transform.position = tile.transform.position + Vector3.up * 0.111f;
                tmp.material = confirmed ? Master.vars.targetingConfirmedMaterial : Master.vars.targetingUnconfirmedMaterial;
                tmp.Set( Vector2.one * 0.9f, 0.05f, true, 0, 0.3f, 0.05f, 0.05f, 1.0f );
            }
        }
    }
}
