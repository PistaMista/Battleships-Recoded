using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftCarrier : Ship
{

    public override void OnBattleBegin ()
    {
        base.OnBattleBegin();
        owner.aircraftCarrier = this;
    }
}
