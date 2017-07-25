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
        InputController.onTap += Skip;
    }

    /// <summary>
    /// Finishes the job of the module.
    /// </summary>
    protected virtual void Finish ()
    {
        running = false;
        foreach (Player player in battle.combatants)
        {
            foreach (Ship ship in player.ships)
            {
                ship.PositionOnBoard();
                ship.gameObject.SetActive( false );
            }
        }
        battle.OnVisualFinish();
        uptime = 0;
        InputController.onTap -= Skip;
    }

    /// <summary>
    /// Updates the module.
    /// </summary>
    public virtual void Refresh ()
    {
        uptime += Time.deltaTime;
    }


    void Skip ( Vector2 position )
    {
        if (Cineman.currentlyPlaying != null)
        {
            Cineman.currentlyPlaying.End();
        }
        Cameraman.ResetWaypoints();
        Finish();
    }
}
