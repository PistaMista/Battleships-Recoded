using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
    /// <summary>
    /// Contains all of the miscellaneous variables, that are adjustable in the inspector.
    /// </summary>
    public static InspectorVariableContainer vars;

    /// <summary>
    /// Attaches the inspector variable container.
    /// </summary>
    void Start ()
    {
        vars = gameObject.GetComponent<InspectorVariableContainer>();
        Cameraman.AddWaypoint( Vector3.up * 100f, Vector3.down, 0, Mathf.Infinity, 0, true );
    }


    public void EndEventTest ()
    {
        Debug.Log( "END EVENT FIRED" );
    }

    public static void StartSecondaryBattle ( int playerAmount, int boardDimensions )
    {
        Battle battle = new GameObject( "Secondary Battle" ).AddComponent<Battle>();
        Player[] players = new Player[playerAmount];

        for (int i = 0; i < playerAmount; i++)
        {
            Player player = (Player)new GameObject( "Player " + ( i + 1 ) ).AddComponent<Player_AI>();
            player.AI = true;
            player.board.InitialiseTiles( boardDimensions );
            player.battle = battle;
            players[i] = player;

            float angle = 360f / playerAmount * i;
            player.transform.position = new Vector3( Mathf.Cos( angle * Mathf.Deg2Rad ), 0, Mathf.Sin( angle * Mathf.Deg2Rad ) ) * Master.vars.secondaryBattleBoardDistance;
        }

        for (int i = 0; i < playerAmount; i++)
        {
            for (int x = 0; x < playerAmount; x++)
            {
                if (x != i)
                {
                    players[i].hits.Add( players[x], new Dictionary<BoardTile, TileHitInformation>() );
                }
            }
        }

        battle.Initialise( players, false );
    }
}
