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
    /// Adds a tile hit.
    /// </summary>
    public void AddTileHit(BoardTile tile)
    {
        if (hitTiles == null)
        {
            hitTiles = new List<BoardTile>();
        }

        if (!hitTiles.Contains(tile))
        {
            hitTiles.Add(tile);
        }
    }

    /// <summary>
    /// Adds a ship hit.
    /// </summary>
    public void AddShipHit(Ship ship)
    {
        if (hitShips == null)
        {
            hitShips = new List<Ship>();
        }

        if (!hitShips.Contains(ship))
        {
            hitShips.Add(ship);
        }
    }

    /// <summary>
    /// Adds a sunk ship.
    /// </summary>
    public void AddSunkShip(Ship ship)
    {
        if (sunkShips == null)
        {
            sunkShips = new List<Ship>();
        }

        if (!sunkShips.Contains(ship))
        {
            sunkShips.Add(ship);
        }
    }

    /// <summary>
    /// Adds a tile hit.
    /// </summary>
    public void AddTorpedoHit(BoardTile tile)
    {
        if (torpedoImpacts == null)
        {
            torpedoImpacts = new List<BoardTile>();
        }

        if (!torpedoImpacts.Contains(tile))
        {
            torpedoImpacts.Add(tile);
        }
    }
}
