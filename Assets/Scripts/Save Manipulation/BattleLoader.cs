using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

using System.Linq;
using System.IO;

public class BattleLoader : MonoBehaviour
{

    static BattleSaver.MainBattleData data;

    public static int LoadData ()
    {
        if (File.Exists( BattleSaver.saveFilePath ))
        {
            FileStream stream = new FileStream( BattleSaver.saveFilePath, FileMode.Open );
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                data = (BattleSaver.MainBattleData)formatter.Deserialize( stream );
            }
            catch
            {
                Debug.LogError( "Battle save file is corrupted - deserialization failed." );
                return 2;
            }

            return 0;
        }
        else
        {
            return 1;
        }
    }

    public static void ReconstructBattle ()
    {
        MainBattle battle = new GameObject( "Main Battle" ).AddComponent<MainBattle>();
        Player[] players = new Player[data.combatants.Length];
        int humans = 0;
        battle.singleplayer = data.singleplayer;
        BattleVisualModule visualModule = (BattleVisualModule)ScriptableObject.CreateInstance( "Cinematic_BattleVisualModule" );
        visualModule.battle = battle;
        battle.visualModule = visualModule;

        //PLAYER INITIALIZATION
        for (int i = 0; i < players.Length; i++)
        {
            BattleSaver.PlayerData playerData = data.combatants[i];
            GameObject playerObject = new GameObject( "Player - " + playerData.label );

            if (playerData.AI)
            {
                players[i] = playerObject.AddComponent<Player_AI>();
            }
            else
            {
                players[i] = playerObject.AddComponent<Player_Human>();
                humans++;
            }

            players[i].AI = playerData.AI;
            players[i].label = playerData.label;
            players[i].alive = playerData.alive;
            players[i].ID = playerData.ID;
            players[i].battle = battle;
            players[i].transform.SetParent( battle.transform );

            if (players[i].label.Length == 0)
            {
                players[i].label = "Player " + ( i + 1 );
            }

            float angle = 360f / players.Length * i;
            players[i].transform.position = new Vector3( Mathf.Cos( angle * Mathf.Deg2Rad ), 0, Mathf.Sin( angle * Mathf.Deg2Rad ) ) * Master.vars.mainBattleBoardDistance;
        }

        //PLAYER DATA MODIFICATION
        //BOARD INIT
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            BattleSaver.PlayerData playerData = data.combatants[i];
            int boardDimensions = (int)Mathf.Sqrt( data.combatants[i].board.tiles.Length );
            player.board.InitialiseTiles( boardDimensions );
        }
        //SHIP INIT
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            BattleSaver.PlayerData playerData = data.combatants[i];

            player.ships = new Ship[playerData.ships.Length];

            //SHIPS
            for (int s = 0; s < player.ships.Length; s++)
            {
                Ship ship;
                BattleSaver.ShipData shipData = playerData.ships[s];

                ship = Instantiate( Master.vars.shipPrefabs[(int)shipData.type] ).GetComponent<Ship>();

                switch (ship.type)
                {
                    case ShipType.AIRCRAFT_CARRIER:
                        player.aircraftCarrier = (AircraftCarrier)ship;
                        break;
                    case ShipType.DESTROYER:
                        player.destroyer = (Destroyer)ship;
                        break;
                }

                player.ships[s] = ship;
            }
        }
        //BOARD MOD
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            BattleSaver.PlayerData playerData = data.combatants[i];

            int boardDimensions = (int)Mathf.Sqrt( data.combatants[i].board.tiles.Length );
            BattleSaver.BoardData boardData = data.combatants[i].board;

            for (int x = 0; x < boardDimensions; x++)
            {
                for (int y = 0; y < boardDimensions; y++)
                {
                    BoardTile tile = player.board.tiles[x, y];
                    BattleSaver.TileData tileData = boardData.tiles[x, y];

                    tile.containedShip = tileData.containedShip >= 0 ? player.ships[tileData.containedShip] : null;

                    for (int r = 0; r < tileData.revealedBy.Length; r++)
                    {
                        tile.revealedBy.Add( players[tileData.revealedBy[r]] );
                    }

                    for (int h = 0; h < tileData.hitBy.Length; h++)
                    {
                        tile.hitBy.Add( players[tileData.hitBy[h]] );
                    }
                }
            }
        }
        //SHIP MOD
        for (int i = 0; i < players.Length; i++)
        {
            Player player = players[i];
            BattleSaver.PlayerData playerData = data.combatants[i];
            for (int s = 0; s < player.ships.Length; s++)
            {
                Ship ship = player.ships[s];
                BattleSaver.ShipData shipData = playerData.ships[s];

                ship.boardPosition = shipData.boardPosition;
                ship.boardRotation = shipData.boardRotation;
                ship.destroyed = shipData.destroyed;
                ship.ID = shipData.ID;
                ship.length = shipData.length;
                ship.lengthRemaining = shipData.lengthRemaining;
                ship.owner = player;
                ship.type = shipData.type;
                ship.transform.SetParent( player.transform );


                ship.tiles = new BoardTile[shipData.tilePositions.Length];
                for (int t = 0; t < ship.tiles.Length; t++)
                {
                    Vector2 pos = shipData.tilePositions[t];
                    ship.tiles[t] = player.board.tiles[(int)pos.x, (int)pos.y];
                }

                ship.revealedBy = new List<Player>();
                foreach (int id in shipData.revealedBy)
                {
                    ship.revealedBy.Add( players[id] );
                }

                ship.PositionOnBoard();
            }
        }

        //HITS
        for (int i = 0; i < players.Length; i++)
        {
            BattleSaver.TileHitData[][] hitsData = data.combatants[i].hits;
            Dictionary<Player, Dictionary<BoardTile, TileHitInformation>> hits = new Dictionary<Player, Dictionary<BoardTile, TileHitInformation>>();

            for (int hi = 0; hi < hitsData.Length; hi++)
            {
                Player target = players[hi];
                Dictionary<BoardTile, TileHitInformation> targetHits = new Dictionary<BoardTile, TileHitInformation>();

                foreach (BattleSaver.TileHitData targetHitData in hitsData[hi])
                {
                    TileHitInformation targetHit = (TileHitInformation)ScriptableObject.CreateInstance( "TileHitInformation" );
                    targetHit.hit = targetHitData.hit;
                    BoardTile tile = target.board.tiles[(int)targetHitData.tileCoordinate.x, (int)targetHitData.tileCoordinate.y];

                    targetHits.Add( tile, targetHit );
                }

                hits.Add( target, targetHits );
            }

            players[i].hits = hits;
        }





        battle.combatants = players;
        battle.activePlayer = players[data.activePlayerID];
        battle.lastSelectedPlayer = data.lastSelectedPlayer >= 0 ? players[data.lastSelectedPlayer] : null;

        //TURN LOG INITIALIZATION
        for (int i = 0; i < data.turnLog.Length; i++)
        {
            BattleSaver.PlayerTurnData dataEntry = data.turnLog[i];
            PlayerTurnActionInformation logEntry = (PlayerTurnActionInformation)ScriptableObject.CreateInstance( "PlayerTurnActionInformation" );
            logEntry.Initialize();

            logEntry.type = dataEntry.type;
            logEntry.activePlayer = players[dataEntry.activePlayerID];
            logEntry.attackedPlayer = dataEntry.attackedPlayerID >= 0 ? players[dataEntry.attackedPlayerID] : null;

            foreach (Vector2 hit in dataEntry.hitTiles)
            {
                logEntry.AddTileHit( logEntry.attackedPlayer.board.tiles[(int)hit.x, (int)hit.y] );
            }

            foreach (Vector2 torpedoHit in dataEntry.torpedoImpacts)
            {
                logEntry.AddTorpedoHit( logEntry.attackedPlayer.board.tiles[(int)torpedoHit.x, (int)torpedoHit.y] );
            }

            foreach (int shipID in dataEntry.hitShips)
            {
                logEntry.AddShipHit( logEntry.attackedPlayer.ships[shipID] );
            }

            foreach (int shipID in dataEntry.sunkShips)
            {
                logEntry.AddSunkShip( logEntry.attackedPlayer.ships[shipID] );
            }

            battle.turnLog.Insert( 0, logEntry );
        }

        UserInterface.managedBattle = battle;
        UserInterface.RespondToBattleChanges();
        Cameraman.SetAuxiliaryParameter( CameramanAuxParameter.BLUR, 0, 0.5f, Mathf.Infinity );
        battle.BeginTurn();
    }
}