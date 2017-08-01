using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ShipType
{
    BATTLESHIP,
    AIRCRAFT_CARRIER,
    CRUISER,
    DESTROYER,
    LARGE_PATROL_BOAT,
    SMALL_PATROL_BOAT
}

public class Ship : MonoBehaviour
{
    /// <summary>
    /// The identifier.
    /// </summary>
    public int ID;
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
            revealedBy.Add( owner.battle.turnLog[0].activePlayer );
            destroyed = true;
            int lastIntactIndex = 0;
            int thisShipIndex = ID;
            for (int i = 0; i < owner.ships.Length; i++)
            {

                Ship ship = owner.ships[i];
                if (!ship.destroyed)
                {
                    lastIntactIndex = i;
                }
                else if (i != thisShipIndex)
                {
                    break;
                }
            }

            owner.ships[lastIntactIndex].ID = ID;
            ID = lastIntactIndex;


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

    /// <summary>
    /// Executed when the battle begins.
    /// </summary>
    public virtual void OnBattleBegin ()
    {

    }

    /// <summary>
    /// Executes when the turn of the owner begins.
    /// </summary>
    public virtual void OnTurnBegin ()
    {

    }

    /// <summary>
    /// Executed when the owner's turn ends.
    /// </summary>
    public virtual void OnTurnEnd ()
    {

    }

    /// <summary>
    /// Executed when any turn, except for the owner's, begins.
    /// </summary>
    public virtual void OnGeneralTurnBegin ()
    {

    }

    /// <summary>
    /// Executed when any turn, except for the owner's, ends.
    /// </summary>
    public virtual void OnGeneralTurnEnd ()
    {

    }

    /// <summary>
    /// Executed when a ship is placed on the board.
    /// </summary>
    public virtual void OnShipPlace ( Ship ship )
    {
        owner.destroyer.UpdateFiringArc();
    }

    /// <summary>
    /// Executed when a ship is removed from the board.
    /// </summary>
    /// <param name="ship">Ship.</param>
    public virtual void OnShipRemove ( Ship ship )
    {
        owner.destroyer.UpdateFiringArc();
    }
}
