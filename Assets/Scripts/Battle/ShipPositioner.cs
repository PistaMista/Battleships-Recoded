using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositioner : MonoBehaviour
{

    /// <summary>
    /// The player currently having his ships positioned.
    /// </summary>
    public static Player currentPlayer;
    /// <summary>
    /// The battle currently being processed.
    /// </summary>
    static Battle battle;
    /// <summary>
    /// The ships still waiting to be placed.
    /// </summary>
    public static List<Ship> shipsToPlace;

    /// <summary>
    /// Adds ships to a battle.
    /// </summary>
    /// <param name="battle"></param>
    public static void AddShipsToBattle ( Battle battle )
    {
        currentPlayer = null;
        ShipPositioner.battle = battle;
        selectedTiles = new List<BoardTile>();
        NextPlayer();
    }

    /// <summary>
    /// Switches to the next player.
    /// </summary>
    /// <returns>Whether there are players to select.</returns>
    public static bool NextPlayer ()
    {
        Player nextPlayer = battle.GetNextPlayer( currentPlayer );
        if (currentPlayer != null && nextPlayer == battle.combatants[0])
        {
            currentPlayer = null;
            Debug.Log( "No more players." );
            return false;
        }
        else
        {
            currentPlayer = nextPlayer;
            AssignShipsToCurrentPlayer();
            ValidateTiles();
            return true;
        }
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update ()
    {
        if (battle != null)
        {
            if (currentPlayer != null)
            {
                if (currentPlayer.AI)
                {
                    RandomizeShips();
                    if (!NextPlayer())
                    {
                        battle.StartBattle();
                        battle = null;
                    }
                }
            }
            else
            {
                battle = null;
            }
        }
    }


    public static List<BoardTile> validTiles;
    static List<BoardTile> processedTiles;
    public static List<BoardTile> selectedTiles;

    /// <summary>
    /// Recalculates the tiles valid for placement.
    /// </summary>
    public static void ValidateTiles ()
    {
        if (shipsToPlace.Count > 0)
        {
            Vector3[] cardinals = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
            if (selectedTiles.Count == 0)
            {
                validTiles = new List<BoardTile>();
                processedTiles = new List<BoardTile>();
                foreach (BoardTile tile in currentPlayer.board.tiles)
                {
                    if (!processedTiles.Contains( tile ))
                    {
                        validTiles.AddRange( ValidateTileRegion( tile, Vector3.right ) );
                        validTiles.AddRange( ValidateTileRegion( tile, Vector3.forward ) );
                    }
                }
            }
            else
            {
                validTiles = new List<BoardTile>();
                processedTiles = new List<BoardTile>();

                if (selectedTiles.Count == 1)
                {
                    List<BoardTile> candidates = new List<BoardTile>();
                    foreach (Vector3 cardinal in cardinals)
                    {
                        BoardTile candidate = currentPlayer.board.GetTileAtWorldPosition( selectedTiles[0].transform.position + cardinal );
                        if (candidate != null)
                        {
                            if (candidate.containedShip == null)
                            {
                                candidates.Add( candidate );
                            }
                        }
                    }

                    List<BoardTile> localTiles = new List<BoardTile>();
                    localTiles.AddRange( ValidateTileRegion( selectedTiles[0], Vector3.forward ) );
                    localTiles.AddRange( ValidateTileRegion( selectedTiles[0], Vector3.right ) );

                    foreach (BoardTile candidate in candidates)
                    {
                        if (localTiles.Contains( candidate ))
                        {
                            validTiles.Add( candidate );
                        }
                    }
                }
                else
                {
                    Vector3 direction = selectedTiles[0].transform.position - selectedTiles[1].transform.position;
                    BoardTile candidate1 = currentPlayer.board.GetTileAtWorldPosition( selectedTiles[0].transform.position + direction );
                    BoardTile candidate2 = currentPlayer.board.GetTileAtWorldPosition( selectedTiles[selectedTiles.Count - 1].transform.position - direction );
                    if (candidate1 != null)
                    {
                        if (candidate1.containedShip == null)
                        {
                            validTiles.Add( candidate1 );
                        }
                    }

                    if (candidate2 != null)
                    {
                        if (candidate2.containedShip == null)
                        {
                            validTiles.Add( candidate2 );
                        }
                    }
                }
            }
        }
        else
        {
            validTiles = new List<BoardTile>();
        }
    }

    /// <summary>
    /// Validates a line of tiles.
    /// </summary>
    /// <param name="tile">Root tile.</param>
    /// <param name="direction">Direction.</param>
    static List<BoardTile> ValidateTileRegion ( BoardTile tile, Vector3 direction )
    {
        List<BoardTile> candidateTiles = new List<BoardTile>();
        int space = 0;

        for (int i = -1; i <= 1; i += 2)
        {
            for (int depth = 0; depth < shipsToPlace[0].length; depth++)
            {
                Vector3 examinedPosition = tile.transform.position + direction * depth * i;
                BoardTile candidate = currentPlayer.board.GetTileAtWorldPosition( examinedPosition );
                if (candidate != null)
                {
                    if (!processedTiles.Contains( candidate ))
                    {
                        processedTiles.Add( candidate );
                    }
                    if (candidate.containedShip == null)
                    {
                        if (!candidateTiles.Contains( candidate ))
                        {
                            if (!validTiles.Contains( candidate ))
                            {
                                candidateTiles.Add( candidate );
                            }
                            space++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        if (space >= shipsToPlace[0].length)
        {
            return candidateTiles;
        }
        return new List<BoardTile>();
    }

    /// <summary>
    /// Selects a tile to contain the current ship.
    /// </summary>
    /// <param name="tile"></param>
    public static void SelectTile ( BoardTile tile )
    {
        if (validTiles.Contains( tile ))
        {
            if (selectedTiles.Count > 0)
            {
                if (Vector3.Distance( selectedTiles[0].transform.position, tile.transform.position ) > 1.1f)
                {
                    selectedTiles.Add( tile );
                }
                else
                {
                    selectedTiles.Insert( 0, tile );
                }
            }
            else
            {
                selectedTiles.Add( tile );
            }

            if (selectedTiles.Count == shipsToPlace[0].length)
            {
                PlaceShip();
                selectedTiles = new List<BoardTile>();
            }

            ValidateTiles();
        }
    }

    /// <summary>
    /// Finishes placing the current ship.
    /// </summary>
    static void PlaceShip ()
    {
        Ship ship = shipsToPlace[0];

        for (int i = 0; i < selectedTiles.Count; i++)
        {
            BoardTile tile = selectedTiles[i];
            tile.containedShip = ship;
            ship.tiles[i] = tile;
        }

        Vector3 position1 = selectedTiles[0].transform.position;
        Vector3 position2 = selectedTiles[selectedTiles.Count - 1].transform.position;

        ship.boardPosition = position1 + ( position2 - position1 ) / 2f;
        if (( position1 - position2 ).x != 0) //Rotates the ship correctly
        {
            ship.boardRotation = Vector3.up * ( 90f - 180f * Random.Range( 0, 2 ) );
        }
        else
        {
            ship.boardRotation = Vector3.up * ( 180f * Random.Range( 0, 2 ) );
        }

        //currentPlayer.ships[Master.vars.startingShipLoadout.Length - shipsToPlace.Count] = ship;
        ship.transform.parent = currentPlayer.transform;

        ship.gameObject.SetActive( true );
        ship.PositionOnBoard();
        shipsToPlace.RemoveAt( 0 );
    }

    /// <summary>
    /// Assigns new ships for the current player.
    /// </summary>
    static void AssignShipsToCurrentPlayer ()
    {
        currentPlayer.ships = new Ship[Master.vars.startingShipLoadout.Length];

        shipsToPlace = new List<Ship>();
        for (int i = 0; i < Master.vars.startingShipLoadout.Length; i++)
        {
            Ship ship = Instantiate( Master.vars.startingShipLoadout[i] ).GetComponent<Ship>();
            ship.owner = currentPlayer;
            ship.ID = i;
            ship.revealedBy.Add( currentPlayer );
            currentPlayer.ships[i] = ship;
            shipsToPlace.Add( ship );
        }
    }

    /// <summary>
    /// Randomizes the ships.
    /// </summary>
    static void RandomizeShips ()
    {
        ValidateTiles();

        while (shipsToPlace.Count > 0)
        {
            if (validTiles.Count == 0)
            {
                selectedTiles = new List<BoardTile>();
                shipsToPlace = new List<Ship>();
                foreach (Ship ship in currentPlayer.ships)
                {
                    RemoveShip( ship );
                }
                ValidateTiles();
            }

            BoardTile randomTile = validTiles[Random.Range( 0, validTiles.Count )];
            SelectTile( randomTile );
        }
    }

    /// <summary>
    /// Removes the ship.
    /// </summary>
    /// <param name="ship">Ship.</param>
    public static void RemoveShip ( Ship ship )
    {
        for (int i = 0; i < ship.tiles.Length; i++)
        {
            BoardTile tile = ship.tiles[i];

            if (tile != null)
            {
                tile.containedShip = null;
            }

            ship.tiles[i] = null;
        }

        ship.gameObject.SetActive( false );
        shipsToPlace.Insert( 0, ship );
    }
}
