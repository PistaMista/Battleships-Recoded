using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battleship : Ship
{
    public int shotBonus;

    public override void OnShipDestroyed ( Ship ship )
    {
        base.OnShipDestroyed( ship );
        if (ship == this)
        {
            owner.shotCapacity -= shotBonus;
        }
    }

    public override void OnShipPlace ( Ship ship )
    {
        base.OnShipPlace( ship );
        if (ship == this)
        {
            owner.shotCapacity += shotBonus;
        }
    }

    public override void OnShipRemove ( Ship ship )
    {
        base.OnShipRemove( ship );
        if (ship == this)
        {
            owner.shotCapacity -= shotBonus;
        }
    }
}
