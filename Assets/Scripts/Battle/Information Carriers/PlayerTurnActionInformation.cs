using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnActionType
{
    SKIP,
    ARTILLERY_ATTACK,
    TORPEDO_ATTACK
}

public class PlayerTurnActionInformation : ScriptableObject
{
    /// <summary>
    /// The type of the turn action.
    /// </summary>
    public TurnActionType type;
    /// <summary>
    /// The player that was active.
    /// </summary>
    public Player activePlayer;
    /// <summary>
    /// The player that was attacked.
    /// </summary>
    public Player attackedPlayer;
    /// <summary>
    /// The ships which were sunk.
    /// </summary>
    public List<Ship> sunkShips;
    /// <summary>
    /// The ships which were hit.
    /// </summary>
    public List<Ship> hitShips;
    /// <summary>
    /// The tiles which were hit.
    /// </summary>
    public List<BoardTile> hitTiles;
    /// <summary>
    /// The tiles which were hit directly by torpedo.
    /// </summary>
    public List<BoardTile> torpedoImpacts;

    /// <summary>
    /// Initialize this instance.
    /// </summary>
    public void Initialize ()
    {
        hitTiles = new List<BoardTile>();
        torpedoImpacts = new List<BoardTile>();
        hitShips = new List<Ship>();
        sunkShips = new List<Ship>();
    }
    /// <summary>
    /// Adds a tile hit.
    /// </summary>
    public void AddTileHit ( BoardTile tile )
    {
        hitTiles.Add( tile );
    }

    /// <summary>
    /// Adds a ship hit.
    /// </summary>
    public void AddShipHit ( Ship ship )
    {
        hitShips.Add( ship );
    }

    /// <summary>
    /// Adds a sunk ship.
    /// </summary>
    public void AddSunkShip ( Ship ship )
    {
        sunkShips.Add( ship );
    }

    /// <summary>
    /// Adds a tile hit.
    /// </summary>
    public void AddTorpedoHit ( BoardTile tile )
    {
        torpedoImpacts.Add( tile );
    }
}
