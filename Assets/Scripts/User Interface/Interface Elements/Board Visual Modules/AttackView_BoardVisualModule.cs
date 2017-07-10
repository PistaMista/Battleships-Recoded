using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackView_BoardVisualModule : BoardVisualModule
{
    public override void Initialize ()
    {
        base.Initialize();
    }

    public override void Enable ()
    {
        base.Enable();
        board.visualModules[1].Enable();
        board.ReinitializeGrid();

        foreach (Ship ship in board.owner.ships)
        {
            ship.gameObject.SetActive( !ship.eliminated && ship.revealedBy.Contains( UserInterface.managedBattle.activePlayer ) );
        }

        float boardSide = Mathf.Sqrt( board.tiles.Length );
        float elevation = 0.5f + ( boardSide / 2f ) / Mathf.Atan( Camera.main.fieldOfView / 2f * Mathf.Deg2Rad );
        Cameraman.AddWaypoint( board.owner.transform.position + Vector3.up * elevation, Vector3.down, 0.3f, Mathf.Infinity, 90f, false );
    }

    public override void Disable ()
    {
        base.Disable();
        board.DisableGrid();
    }

    public override void Refresh ()
    {
        base.Refresh();
    }
}
