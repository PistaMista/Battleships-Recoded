using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleVisualModule : ScriptableObject
{

    /// <summary>
    /// Whether the visual module is running and doing something important.
    /// </summary>
    public bool running = false;
    /// <summary>
    /// The battle this module is attached to.
    /// </summary>
    public Battle battle;
    /// <summary>
    /// How long has the module been running.
    /// </summary>
    public float uptime;
    /// <summary>
    /// The type of this visual module.
    /// </summary>
    public string type;

    /// <summary>
    /// Processes an artillery attack.
    /// </summary>
    /// <param name="turnInfo"></param>
    public virtual void ProcessArtilleryAttack ( PlayerTurnActionInformation turnInfo )
    {
        running = true;
    }

    /// <summary>
    /// Finishes the job of the module.
    /// </summary>
    protected virtual void Finish ()
    {
        running = false;
        battle.OnVisualFinish();
        uptime = 0;
    }

    /// <summary>
    /// Updates the module.
    /// </summary>
    public virtual void Refresh ()
    {
        uptime += Time.deltaTime;
    }
}
