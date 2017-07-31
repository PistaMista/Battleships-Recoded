using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

using System.Linq;
using System.IO;
using System;

public class BattleSaver : MonoBehaviour
{
    public static string saveFilePath;

    void Start ()
    {
        saveFilePath = Path.Combine( Application.persistentDataPath, Master.vars.saveFileName );
    }

    [Serializable]
    public struct Vector2Serializable
    {
        public float x;
        public float y;

        public Vector2Serializable ( float x, float y )
        {
            this.x = x;
            this.y = y;
        }

        static public implicit operator Vector2 ( Vector2Serializable val )
        {
            return new Vector2( val.x, val.y );
        }

        static public implicit operator Vector2Serializable ( Vector2 val )
        {
            return new Vector2Serializable( val.x, val.y );
        }
    }

    [Serializable]
    public struct Vector3Serializable
    {
        public float x;
        public float y;
        public float z;

        public Vector3Serializable ( float x, float y, float z )
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        static public implicit operator Vector3 ( Vector3Serializable val )
        {
            return new Vector3( val.x, val.y, val.z );
        }

        static public implicit operator Vector3Serializable ( Vector3 val )
        {
            return new Vector3Serializable( val.x, val.y, val.z );
        }
    }

    [Serializable]
    public struct MainBattleData
    {
        /// <summary>
        /// Players participating in the battle. Dead or alive.
        /// </summary>
        public PlayerData[] combatants;
        /// <summary>
        /// The player that is currently active and is responsible for finishing this turn.
        /// </summary>
        public int activePlayerID;
        /// <summary>
        /// Logs all of the player's actions.
        /// </summary>
        public PlayerTurnData[] turnLog;
        /// <summary>
        /// The last selected player.
        /// </summary>
        public int lastSelectedPlayer;
        /// <summary>
        /// Whether the battle has exactly one human player in it.
        /// </summary>
        public bool singleplayer;
    }

    [Serializable]
    public struct PlayerTurnData
    {
        /// <summary>
        /// The type of the turn action.
        /// </summary>
        public TurnActionType type;
        /// <summary>
        /// The player that was active.
        /// </summary>
        public int activePlayerID;
        /// <summary>
        /// The player that was attacked.
        /// </summary>
        public int attackedPlayerID;
        /// <summary>
        /// The ships which were sunk.
        /// </summary>
        public int[] sunkShips;
        /// <summary>
        /// The ships which were hit.
        /// </summary>
        public int[] hitShips;
        /// <summary>
        /// The tiles which were hit.
        /// </summary>
        public Vector2Serializable[] hitTiles;
        /// <summary>
        /// The tiles which were hit directly by torpedo.
        /// </summary>
        public Vector2Serializable[] torpedoImpacts;
    }

    [Serializable]
    public struct PlayerData
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
        public bool alive;
        /// <summary>
        /// Whether this player is controlled by artificial intelligence.
        /// </summary>
        public bool AI;

        /// <summary>
        /// This player's board.
        /// </summary>
        public BoardData board;
        /// <summary>
        /// All ships owned by this player.
        /// </summary>
        public ShipData[] ships;

        //AIRCRAFT CARRIER ASSIGNED BY LOADER USING LOGIC


        //DESTROYER ASSIGNED BY LOADER USING LOGIC


        //BATTLE REFERENCE ASSIGNED BY LOADER

        /// <summary>
        /// All the hits the player has scored on other players and the information about them.
        /// </summary>
        public TileHitData[][] hits;
    }

    [Serializable]
    public struct BoardData
    {
        /// <summary>
        /// The tiles of which the board is composed.
        /// </summary>
        public TileData[,] tiles;
    }

    [Serializable]
    public struct TileData
    {
        //BOARD REFERENCE ASSIGNED BY LOADER

        /// <summary>
        /// The board coordinate.
        /// </summary>
        public Vector2Serializable boardCoordinate;
        /// <summary>
        /// The list of players that have hit this tile.
        /// </summary>
        public int[] hitBy;
        /// <summary>
        /// The list of players who can see whats in the tile.
        /// </summary>
        public int[] revealedBy;
        /// <summary>
        /// The ship that occupies this tile.
        /// </summary>
        public int containedShip;

    }

    [Serializable]
    public struct TileHitData
    {
        /// <summary>
        /// Whether this attack has hit a ship in this tile.
        /// </summary>
        public bool hit;
        /// <summary>
        /// The tile coordinate that has been hit.
        /// </summary>
        public Vector2Serializable tileCoordinate;
    }

    [Serializable]
    public struct ShipData
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        public int ID;

        //OWNER ASSIGNED BY LOADER

        /// <summary>
        /// The length of this ship.
        /// </summary>
        public int length;
        /// <summary>
        /// The positions on the board that this ship occupies.
        /// </summary>
        public Vector2Serializable[] tilePositions;
        /// <summary>
        /// Whether this ship has been destroyed.
        /// </summary>
        public bool destroyed;
        /// <summary>
        /// The number of ship segments still intact.
        /// </summary>
        public int lengthRemaining;
        /// <summary>
        /// All the players who can see exactly where this ship is.
        /// </summary>
        public int[] revealedBy;

        /// <summary>
        /// The type of this ship.
        /// </summary>
        public ShipType type;

        /// <summary>
        /// The position of this ship on the board.
        /// </summary>
        public Vector3Serializable boardPosition;
        /// <summary>
        /// The rotation of this ship on the board.
        /// </summary>
        public Vector3Serializable boardRotation;
    }



    public static bool SaveCurrentBattle ()
    {
        MainBattle battle = UserInterface.managedBattle;
        if (battle.activePlayer == null)
        {
            battle.activePlayer = battle.GetNextPlayer( battle.turnLog[0].activePlayer );
            if (battle.activePlayer == null)
            {
                return false;
            }
        }


        MainBattleData saveData;
        if (battle.activePlayer != null)
        {
            saveData.activePlayerID = battle.activePlayer.ID;
        }
        else
        {
            Player candidate = battle.GetNextPlayer( battle.turnLog[0].activePlayer );
            if (candidate != null)
            {
                saveData.activePlayerID = candidate.ID;
            }
            else
            {
                return false;
            }
        }

        saveData.lastSelectedPlayer = battle.lastSelectedPlayer != null ? battle.lastSelectedPlayer.ID : -1;

        saveData.singleplayer = battle.singleplayer;

        //TURN LOG CONSTRUCTION
        PlayerTurnData[] turnLog = new PlayerTurnData[battle.turnLog.Count];


        for (int i = 0; i < turnLog.Length; i++)
        {
            PlayerTurnActionInformation o = battle.turnLog[i];
            PlayerTurnData c;
            c.activePlayerID = o.activePlayer.ID;
            c.attackedPlayerID = o.attackedPlayer != null ? o.attackedPlayer.ID : -1;

            c.hitShips = new int[o.hitShips.Count];
            c.sunkShips = new int[o.sunkShips.Count];

            c.hitTiles = new Vector2Serializable[o.hitTiles.Count];
            c.torpedoImpacts = new Vector2Serializable[o.torpedoImpacts.Count];



            for (int r = 0; r < c.hitShips.Length; r++)
            {
                c.hitShips[r] = o.hitShips[r].ID;
            }

            for (int r = 0; r < c.sunkShips.Length; r++)
            {
                c.sunkShips[r] = o.sunkShips[r].ID;
            }

            for (int r = 0; r < c.hitShips.Length; r++)
            {
                c.hitTiles[r] = o.hitTiles[r].boardCoordinate;
            }

            for (int r = 0; r < c.torpedoImpacts.Length; r++)
            {
                c.torpedoImpacts[r] = o.torpedoImpacts[r].boardCoordinate;
            }

            c.type = o.type;

            turnLog[i] = c;
        }

        saveData.turnLog = turnLog;

        //PLAYER DATA ARRAY CONSTRUCTION
        PlayerData[] combatants = new PlayerData[battle.combatants.Length];

        for (int i = 0; i < combatants.Length; i++)
        {
            Player o = battle.combatants[i];
            PlayerData c;

            c.AI = o.AI;
            c.alive = o.alive;
            c.ID = o.ID;
            c.label = o.label;


            //SHIP ARRAY CONSTRUCTION
            ShipData[] ships = new ShipData[o.ships.Length];
            for (int s = 0; s < ships.Length; s++)
            {
                Ship x = o.ships[s];
                ShipData y;

                y.boardPosition = x.boardPosition;
                y.boardRotation = x.boardRotation;
                y.destroyed = x.destroyed;
                y.ID = x.ID;
                y.length = x.length;
                y.lengthRemaining = x.lengthRemaining;
                y.type = x.type;
                y.tilePositions = new Vector2Serializable[x.tiles.Length];

                for (int ti = 0; ti < x.tiles.Length; ti++)
                {
                    y.tilePositions[ti] = x.tiles[ti].boardCoordinate;
                }

                y.revealedBy = new int[x.revealedBy.Count];

                for (int oi = 0; oi < x.revealedBy.Count; oi++)
                {
                    y.revealedBy[oi] = x.revealedBy[oi].ID;
                }

                ships[s] = y;
            }

            c.ships = ships;

            //HITS ARRAY CONSTRUCTION
            Player[] keys = o.hits.Keys.ToArray<Player>();
            TileHitData[][] hits = new TileHitData[keys.Length][];

            for (int hi = 0; hi < hits.Length; hi++)
            {
                Dictionary<BoardTile, TileHitInformation> onPlayerHitsInfo = o.hits[keys[hi]];
                hits[hi] = new TileHitData[onPlayerHitsInfo.Count];


                int index = 0;
                foreach (KeyValuePair<BoardTile, TileHitInformation> item in onPlayerHitsInfo)
                {
                    hits[hi][index].hit = item.Value.hit;
                    hits[hi][index].tileCoordinate = item.Key.boardCoordinate;
                    index++;
                }
            }

            c.hits = hits;
            //BOARD DATA STRUCTURE CONSTRUCTION
            int boardDimensions = o.board.sideTileLength;
            c.board.tiles = new TileData[boardDimensions, boardDimensions];

            for (int x = 0; x < boardDimensions; x++)
            {
                for (int y = 0; y < boardDimensions; y++)
                {
                    BoardTile ot = o.board.tiles[x, y];
                    TileData ct;

                    ct.boardCoordinate = ot.boardCoordinate;
                    ct.containedShip = ot.containedShip != null ? ot.containedShip.ID : -1;

                    ct.hitBy = new int[ot.hitBy.Count];
                    for (int l = 0; l < ct.hitBy.Length; l++)
                    {
                        ct.hitBy[l] = ot.hitBy[l].ID;
                    }

                    ct.revealedBy = new int[ot.revealedBy.Count];
                    for (int l = 0; l < ct.revealedBy.Length; l++)
                    {
                        ct.revealedBy[l] = ot.revealedBy[l].ID;
                    }

                    c.board.tiles[x, y] = ct;
                }
            }

            combatants[i] = c;
        }

        saveData.combatants = combatants;

        FileStream stream = new FileStream( saveFilePath, FileMode.Create );
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize( stream, saveData );

        stream.Close();

        return true;
    }

    void OnApplicationQuit ()
    {
        if (UserInterface.managedBattle != null)
        {
            SaveCurrentBattle();
        }
    }

    void OnApplicationPause ( bool pause )
    {
        if (UserInterface.managedBattle != null)
        {
            SaveCurrentBattle();
        }
    }
}
