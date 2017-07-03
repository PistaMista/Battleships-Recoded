using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    /// <summary>
    /// The board which this tile is part of.
    /// </summary>
    public Board board;
    /// <summary>
    /// The list of players that have hit this tile.
    /// </summary>
    public List<Player> hitBy;
    /// <summary>
    /// The list of players who can see whats in the tile.
    /// </summary>
    public List<Player> revealedBy;
    /// <summary>
    /// The ship that occupies this tile.
    /// </summary>
    public Ship containedShip;

    public void Awake()
    {
        hitBy = new List<Player>();
        revealedBy = new List<Player>();
    }
}
