using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ShipType
{
    CRUISER,
    BATTLESHIP,
    DESTROYER,
    AIRCRAFT_CARRIER
}

public class Ship : MonoBehaviour
{
    /// <summary>
    /// The owner of this ship.
    /// </summary>
    public Player owner;
    /// <summary>
    /// The length of this ship.
    /// </summary>
    public int length = 3;
    /// <summary>
    /// The positions on the board that this ship occupies.
    /// </summary>
    public BoardTile[] tiles;
    /// <summary>
    /// Whether this ship has been destroyed.
    /// </summary>
    public bool destroyed = false;
    /// <summary>
    /// The number of ship segments still intact.
    /// </summary>
    public int lengthRemaining;
    /// <summary>
    /// All the players who can see exactly where this ship is.
    /// </summary>
    public List<Player> revealedBy;

    /// <summary>
    /// The type of this ship.
    /// </summary>
    public ShipType type;

    /// <summary>
    /// The position of this ship on the board.
    /// </summary>
    public Vector3 boardPosition;
    /// <summary>
    /// The rotation of this ship on the board.
    /// </summary>
    public Vector3 boardRotation;
    /// <summary>
    /// Registers a hit on this ship.
    /// </summary>
    public void RegisterHit ()
    {
        lengthRemaining--;

        if (lengthRemaining == 0)
        {
            destroyed = true;
            int lastIntactIndex = 0;
            int thisShipIndex = 0;
            for (int i = 0; i < owner.ships.Length; i++)
            {

                Ship ship = owner.ships[i];
                if (ship == this)
                {
                    thisShipIndex = i;
                }
                else if (!ship.destroyed)
                {
                    lastIntactIndex = i;
                }
                else
                {
                    break;
                }
            }

            owner.ships[thisShipIndex] = owner.ships[lastIntactIndex];
            owner.ships[lastIntactIndex] = this;
        }
    }

    /// <summary>
    /// Positions this ship correctly on the board.
    /// </summary>
    public void PositionOnBoard ()
    {
        transform.position = boardPosition;
        transform.rotation = Quaternion.Euler( boardRotation );
    }


    void Awake ()
    {
        tiles = new BoardTile[length];
        revealedBy = new List<Player>();
        lengthRemaining = length;
    }


    //VISUALS
    /// <summary>
    /// All of the gun turrets that this ship is equipped with.
    /// </summary>
    public RotatableWeaponMounting[] gunTurrets;

    void Update ()
    {

    }
}
