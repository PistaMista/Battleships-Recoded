using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// The ID of this player in the battle.
    /// </summary>
    public int ID;
    /// <summary>
    /// The name of this player.
    /// </summary>
    public string label;

    /// <summary>
    /// Whether this player is alive.
    /// </summary>
    public bool alive = true;
    /// <summary>
    /// Whether this player is controlled by artificial intelligence.
    /// </summary>
    public bool AI;

    /// <summary>
    /// Determines the amount of tiles this player can target in one turn with his guns.
    /// </summary>
    public int gunTargetCap = 1;
    /// <summary>
    /// This player's board.
    /// </summary>
    public Board board;
    /// <summary>
    /// All ships owned by this player.
    /// </summary>
    public Ship[] ships;
    /// <summary>
    /// This player's aircraft carrier.
    /// </summary>
    public AircraftCarrier aircraftCarrier;
    /// <summary>
    /// This player's destroyer.
    /// </summary>
    public Destroyer destroyer;
    /// <summary>
    /// The battle this player is taking part in.
    /// </summary>
    public Battle battle;

    /// <summary>
    /// All the hits the player has scored on other players and the information about them.
    /// </summary>
    public Dictionary<Player, Dictionary<BoardTile, TileHitInformation>> hits;

    void Awake ()
    {
        board = new GameObject( "Board" ).AddComponent<Board>();
        board.owner = this;
        board.transform.parent = transform;
        hits = new Dictionary<Player, Dictionary<BoardTile, TileHitInformation>>();
    }

    /// <summary>
    /// Executed when a turn begins.
    /// </summary>
    public virtual void OnBeginTurn ()
    {

    }

    /// <summary>
    /// Executed when a turn ends.
    /// </summary>
    public virtual void OnEndTurn ()
    {

    }
}
