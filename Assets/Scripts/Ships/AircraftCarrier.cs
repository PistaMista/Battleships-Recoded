using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftCarrier : Ship
{
    public override void OnShipPlace ( Ship ship )
    {
        base.OnShipPlace( ship );
        if (ship.type == ShipType.DESTROYER)
        {
            owner.destroyer.CalculateReloadRate();
        }
    }
}
