using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mime;

public class Destroyer : Ship
{
    /// <summary>
    /// The torpedo cap.
    /// </summary>
    public int torpedoCap;
    /// <summary>
    /// The amount of torpedoes this destroyer has.
    /// </summary>
    public float torpedoes;
    /// <summary>
    /// The amount of torpedoes resupplied each turn.
    /// </summary>
    public float reloadRate;

    public override void OnBattleBegin ()
    {
        base.OnBattleBegin();
        owner.destroyer = this;
        float boardDimensions = Mathf.Sqrt( owner.board.tiles.Length );
        float distance = Vector3.Distance( transform.position, owner.aircraftCarrier.transform.position );
        reloadRate = 1.2f / Mathf.Pow( distance, 0.8f );
    }
    public override void OnTurnBegin ()
    {
        base.OnTurnBegin();
        torpedoes = Mathf.Clamp( torpedoes + reloadRate, 0, torpedoCap );
    }
}
