using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AI : Player
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (battle.activePlayer == this && battle.selectedPlayer != null && battle.prepareTime == 0)
        {
            Attack();
        }
    }

    public override void OnBeginTurn()
    {
        base.OnBeginTurn();
        SelectAircraftTarget();
        SelectTarget();
    }

    void SelectAircraftTarget()
    {

    }

    void SelectTarget()
    {
        Player randomPlayer = battle.combatants[Random.Range(0, battle.combatants.Length)];
        while (!randomPlayer.alive || randomPlayer == this)
        {
            randomPlayer = battle.combatants[Random.Range(0, battle.combatants.Length)];
        }

        battle.SelectPlayer(randomPlayer);
    }

    void Attack()
    {
        ArtilleryAttack();
    }

    void ArtilleryAttack()
    {
        battle.ArtilleryAttack(ChooseTileToShoot());
    }

    BoardTile ChooseTileToShoot()
    {
        Dictionary<BoardTile, TileHitInformation> hits = this.hits[battle.selectedPlayer];

        foreach (BoardTile tile in battle.selectedPlayer.board.tiles)
        {
            if (tile.revealedBy.Contains(this) && !(hits.ContainsKey(tile)))
            {
                return tile;
            }
        }

        List<BoardTile> processedTiles = new List<BoardTile>();
        Dictionary<int, List<BoardTile>> rankedTiles = new Dictionary<int, List<BoardTile>>();

        int highestRank = 0;

        for (int i = 1; i <= 10; i++)
        {
            rankedTiles.Add(i, new List<BoardTile>());
        }

        Vector3[] cardinalDirections = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        //Eliminate the misses
        foreach (KeyValuePair<BoardTile, TileHitInformation> hit in hits)
        {
            if (!hit.Value.hit)
            {
                processedTiles.Add(hit.Key);
            }
        }

        //Analyze hits
        foreach (KeyValuePair<BoardTile, TileHitInformation> hit in hits)
        {
            if (!processedTiles.Contains(hit.Key))
            {
                processedTiles.Add(hit.Key);


                Vector3 examinedDirection = Vector3.zero;
                foreach (Vector3 direction in cardinalDirections)
                {
                    Vector3 checkedPosition = hit.Key.transform.position + direction;
                    BoardTile checkedTile = battle.selectedPlayer.board.GetTileAtWorldPosition(checkedPosition);
                    if (checkedTile != null)
                    {
                        if (hits.ContainsKey(checkedTile))
                        {
                            if (hits[checkedTile].hit)
                            {
                                examinedDirection = direction;
                                break;
                            }
                        }
                    }
                }

                if (examinedDirection != Vector3.zero)
                {
                    for (int direction = -1; direction <= 1; direction += 2)
                    {
                        for (int i = 1; i < Mathf.Sqrt(battle.selectedPlayer.board.tiles.Length); i++)
                        {
                            Vector3 checkedPosition = hit.Key.transform.position + examinedDirection * i * direction;
                            BoardTile checkedTile = battle.selectedPlayer.board.GetTileAtWorldPosition(checkedPosition);
                            if (checkedTile != null)
                            {
                                processedTiles.Add(checkedTile);
                                if (!hits.ContainsKey(checkedTile))
                                {
                                    rankedTiles[10].Add(checkedTile);
                                    if (10 > highestRank)
                                    {
                                        highestRank = 10;
                                    }
                                    break;
                                }
                                else
                                {
                                    if (!hits[checkedTile].hit)
                                    {
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    foreach (Vector3 direction in cardinalDirections)
                    {
                        Vector3 checkedPosition = hit.Key.transform.position + direction;
                        BoardTile checkedTile = battle.selectedPlayer.board.GetTileAtWorldPosition(checkedPosition);

                        if (checkedTile != null)
                        {
                            if (!processedTiles.Contains(checkedTile))
                            {
                                processedTiles.Add(checkedTile);
                                rankedTiles[10].Add(checkedTile);
                                if (10 > highestRank)
                                {
                                    highestRank = 10;
                                }
                            }
                        }
                    }
                }
            }
        }



        //Add the other tiles
        foreach (BoardTile candidateTile in battle.selectedPlayer.board.tiles)
        {
            if (!processedTiles.Contains(candidateTile))
            {
                rankedTiles[1].Add(candidateTile);
                if (1 > highestRank)
                {
                    highestRank = 1;
                }
            }
        }

        BoardTile result = null;
        //Choose a tile to attack
        int targetRank = Random.Range(0, highestRank - 1);
        //Choose a rank to pick a tile from
        for (int i = 1; i <= 10; i++)
        {
            if (targetRank < i && rankedTiles[i].Count > 0)
            {
                targetRank = i;
                break;
            }
        }

        result = rankedTiles[targetRank][Random.Range(0, rankedTiles[targetRank].Count - 1)];
        return result;
    }

    void TorpedoAttack()
    {

    }
}
