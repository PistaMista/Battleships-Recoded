﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinematic_BattleVisualModule : BattleVisualModule
{
    int stage;
    RotatableWeaponMounting selectedTurret;

    public override void ProcessArtilleryAttack ( PlayerTurnActionInformation turnInfo )
    {
        base.ProcessArtilleryAttack( turnInfo );
        Player attacker = battle.turnLog[0].activePlayer;
        Player defender = battle.turnLog[0].attackedPlayer;
        Ship ship = attacker.ships[Random.Range( 0, attacker.ships.Length )];

        Vector3 relative = defender.transform.position - ship.transform.position;
        ship.transform.rotation.SetLookRotation( relative.normalized );
        ship.transform.Rotate( Vector3.up * ( Random.Range( -60, 60 ) + 90 ) );
        ship.gameObject.SetActive( true );

        foreach (RotatableWeaponMounting turret in ship.gunTurrets)
        {
            Vector3 position = Vector3.zero;
            if (battle.turnLog[0].hitShips.Count > 0)
            {
                Ship hitShip = battle.turnLog[0].hitShips[0];
                position = battle.turnLog[0].hitShips[0].transform.position;
                hitShip.gameObject.SetActive( hitShip.revealedBy.Contains( attacker ) );
            }
            else
            {
                position = battle.turnLog[0].hitTiles[0].transform.position;
            }

            turret.RotateTowards( position );
            turret.AimWeapons( position );

            if (turret.IsWithinFiringAngles())
            {
                selectedTurret = turret;
            }
        }

        stage = 1;
        Cineman.StartCinematic( "WeaponTurretPan", OnTurretPanFinish, new object[] { selectedTurret, 5f, 2f } );
    }

    public override void Refresh ()
    {
        base.Refresh();
        switch (stage)
        {
            case 1:

                break;
        }
    }

    protected override void Finish ()
    {
        base.Finish();
    }

    void OnTurretPanFinish ()
    {

    }
}
