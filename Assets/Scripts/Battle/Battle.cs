using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Battle : MonoBehaviour
{
    /// <summary>
    /// Players participating in the battle. Dead or alive.
    /// </summary>
    public Player[] combatants;
    /// <summary>
    /// The player that is currently active and is responsible for finishing this turn.
    /// </summary>
    public Player activePlayer;
    /// <summary>
    /// The player that is currently selected by this player.
    /// </summary>
    public Player selectedPlayer;
    /// <summary>
    /// The module which controls what the player sees.
    /// </summary>
    BattleVisualModule visualModule;
    /// <summary>
    /// Logs all of the player's actions.
    /// </summary>
    List<PlayerTurnActionInformation> turnLog;
    /// <summary>
    /// Whether the battle has ended and only one player is remaining.
    /// </summary>
    public bool finished;

    //INITIALISATION MANAGEMENT
    /// <summary>
    /// Initialises the battle.
    /// </summary>
    /// <param name="combatants">The players to compete in this battle.</param>
    /// <param name="functional">Whether the battle should ignore all visual components.</param>
    public virtual void Initialise(Player[] combatants, bool functional)
    {
        this.combatants = combatants;
        for (int i = 0; i < combatants.Length; i++)
        {
            combatants[i].gameObject.transform.parent = transform;
            combatants[i].battle = this;
        }

        visualModule = (BattleVisualModule)ScriptableObject.CreateInstance("Test_BattleVisualModule");
        visualModule.battle = this;

        ShipPositioner.AddShipsToBattle(this);
    }

    protected virtual void Awake()
    {
        turnLog = new List<PlayerTurnActionInformation>();
    }



    //TIME AND BATTLESTATE MANAGEMENT
    /// <summary>
    /// Time left until the battle is ready for another phase.
    /// </summary>
    public float prepareTime;
    protected virtual void Update()
    {
        if (visualModule.running)
        {
            visualModule.Refresh();
        }
        prepareTime = prepareTime > 0 ? prepareTime - Time.deltaTime : 0;
    }

    /// <summary>
    /// Starts the battle.
    /// </summary>
    public virtual void StartBattle()
    {
        activePlayer = combatants[0];
        BeginTurn();
    }

    //ATTACKS
    /// <summary>
    /// Executes an artillery attack on the target tile of the defending player's board.
    /// </summary>
    /// <param name="tile">Position of the tile to hit.</param>
    /// <returns>Hit successful.</returns>
    public virtual bool ArtilleryAttack(BoardTile tile)
    {
        if (tile != null)
        {
            if (!activePlayer.hits[selectedPlayer].ContainsKey(tile))
            {
                turnLog[0].type = TurnActionType.ARTILLERY_ATTACK;

                if (tile.containedShip && Random.Range(0, 10) == 0 && tile.containedShip.type != ShipType.AIRCRAFT_CARRIER)
                {
                    if (!tile.containedShip.eliminated)
                    {
                        foreach (BoardTile t in tile.containedShip.tiles)
                        {
                            RegisterHitOnTile(t);
                        }
                        //tile.containedShip.RevealTo(attackingPlayer);
                    }
                }
                else
                {
                    RegisterHitOnTile(tile);
                }

                EndTurn();
                visualModule.ProcessArtilleryAttack(turnLog[0]);

                return true;
            }
        }


        Debug.LogWarning("There was an attempt to shoot an invalid tile: " + tile + ". Things may break.");
        return false;
    }

    /// <summary>
    /// Registers a hit on a tile.
    /// </summary>
    void RegisterHitOnTile(BoardTile tile)
    {
        TileHitInformation hitInfo = (TileHitInformation)ScriptableObject.CreateInstance("TileHitInformation");
        hitInfo.hit = false;

        if (tile.containedShip)
        {
            if (!tile.containedShip.eliminated)
            {
                turnLog[0].AddShipHit(tile.containedShip);
                if (tile.hitBy.Count == 0)
                {
                    tile.containedShip.RegisterHit();
                    if (tile.containedShip.eliminated)
                    {
                        turnLog[0].AddSunkShip(tile.containedShip);
                    }
                    hitInfo.hit = true;

                    if (tile.containedShip.owner.ships[0].eliminated)
                    {
                        tile.containedShip.owner.alive = false;
                    }
                }
            }
        }

        activePlayer.hits[selectedPlayer][tile] = hitInfo;
        tile.hitBy.Add(activePlayer);
        turnLog[0].AddTileHit(tile);
    }


    //TURN SWITCHING
    /// <summary>
    /// Begins the turn for the active player.
    /// </summary>
    protected virtual void BeginTurn()
    {
        PlayerTurnActionInformation turnInfo = (PlayerTurnActionInformation)ScriptableObject.CreateInstance("PlayerTurnActionInformation");

        turnInfo.activePlayer = activePlayer;
        selectedPlayer = null;

        turnLog.Insert(0, turnInfo);
        activePlayer.OnBeginTurn();
    }

    /// <summary>
    /// Ends the turn for the active player.
    /// </summary>
    protected virtual void EndTurn()
    {
        if (turnLog[0].type != TurnActionType.SKIP)
        {
            turnLog[0].attackedPlayer = selectedPlayer;
        }

        selectedPlayer = null;
        activePlayer.OnEndTurn();
        activePlayer = null;
    }

    /// <summary>
    /// Executed when the visual module finishes its job.
    /// </summary>
    public virtual void OnVisualFinish()
    {
        activePlayer = GetNextPlayer(turnLog[0].activePlayer);
        if (activePlayer == null)
        {
            OnBattleFinish();
        }
        else
        {
            BeginTurn();
            prepareTime = 1.2f;
        }
    }

    /// <summary>
    /// Finds out what player is after the supplied player.
    /// </summary>
    /// <param name="currentPlayer">The previous player.</param>
    /// <returns>The next player.</returns>
    public Player GetNextPlayer(Player currentPlayer)
    {
        if (currentPlayer == null)
        {
            return combatants[0];
        }
        else
        {
            for (int i = 0; i < combatants.Length; i++)
            {
                if (currentPlayer == combatants[i])
                {
                    for (int x = 1; x < combatants.Length; x++)
                    {
                        Player candidate = combatants[(x + i) % combatants.Length];
                        if (candidate.alive)
                        {
                            return candidate;
                        }
                    }
                }
            }
            return null;
        }
    }

    //TARGETING
    /// <summary>
    /// Selects a player.
    /// </summary>
    /// <param name="player">Player to select.</param>
    public virtual void SelectPlayer(Player player)
    {
        selectedPlayer = player;
    }

    //BATTLE FINALISATION
    /// <summary>
    /// Executed when the battle is finished.
    /// </summary>
    protected virtual void OnBattleFinish()
    {
        finished = true;
    }
}
