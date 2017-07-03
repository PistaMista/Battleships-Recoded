using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BattleVisualModule : BattleVisualModule
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void Refresh()
    {
        base.Refresh();
        if (uptime > 3f)
        {
            Finish();
        }
    }

    public override void ProcessArtilleryAttack(PlayerTurnActionInformation turnInfo)
    {
        base.ProcessArtilleryAttack(turnInfo);
        Ship[] ships = turnInfo.activePlayer.ships;

        for (int i = 0; i < ships.Length; i++)
        {
            Ship ship = ships[i];
            if (!ship.eliminated)
            {
                RotatableWeaponMounting[] turrets = ship.gunTurrets;
                for (int x = 0; x < turrets.Length; x++)
                {
                    RotatableWeaponMounting turret = turrets[x];
                    turret.RotateTowards(turnInfo.hitTiles[0].transform.position);
                    turret.AimWeapons(turnInfo.hitTiles[0].transform.position);
                    turret.fireAfterRotationFinishes = true;
                }
            }
        }
    }
}
