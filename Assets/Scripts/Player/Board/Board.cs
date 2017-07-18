using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    /// <summary>
    /// The owner of the board.
    /// </summary>
    public Player owner;
    /// <summary>
    /// The parent of all graphical elements created by this board and its tiles.
    /// </summary>
    public GameObject visualParent;
    /// <summary>
    /// The tiles the board is made up of.
    /// </summary>
    public BoardTile[,] tiles;
    /// <summary>
    /// All of the modules that handle visual elements of the board.
    /// </summary>
    public BoardVisualModule[] visualModules;

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update ()
    {
        foreach (BoardVisualModule module in visualModules)
        {
            if (module.enabled)
            {
                module.Refresh();
            }
        }
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake ()
    {
        visualModules = new BoardVisualModule[Master.vars.boardVisualModules.Length];

        for (int i = 0; i < visualModules.Length; i++)
        {
            visualModules[i] = (BoardVisualModule)ScriptableObject.CreateInstance( Master.vars.boardVisualModules[i] + "_BoardVisualModule" );
            visualModules[i].Initialize();
            visualModules[i].board = this;
        }
    }

    /// <summary>
    /// Resets all enabled visual modes.
    /// </summary>
    public void ResetVisualModules ()
    {
        foreach (BoardVisualModule module in visualModules)
        {
            module.Disable();
        }

        Destroy( visualParent );
        visualParent = new GameObject( "Visual Parent" );
        visualParent.transform.parent = transform;
        visualParent.transform.localPosition = Vector3.zero;

        foreach (Ship ship in owner.ships)
        {
            ship.gameObject.SetActive( false );
        }
    }

    /// <summary>
    /// The grid.
    /// </summary>
    GameObject grid;

    /// <summary>
    /// Reinitializes the grid.
    /// </summary>
    public void ReinitializeGrid ()
    {
        DisableGrid();
        grid = new GameObject( "Grid" );
        grid.transform.parent = visualParent.transform;
        grid.transform.localPosition = Vector3.up * 0.1f;
        int sideLength = (int)Mathf.Sqrt( tiles.Length );

        for (int x = 1; x < sideLength; x++)
        {
            float pos = -(float)sideLength / 2f + x;
            AddGridLine( new Vector3( pos, 0f, 0f ), false, sideLength + 0.3f );
        }

        for (int y = 1; y < sideLength; y++)
        {
            float pos = -(float)sideLength / 2f + y;
            AddGridLine( new Vector3( 0f, 0f, pos ), true, sideLength + 0.3f );
        }
    }

    /// <summary>
    /// Disables the grid.
    /// </summary>
    public void DisableGrid ()
    {
        Destroy( grid );
    }

    /// <summary>
    /// Adds a grid line.
    /// </summary>
    /// <param name="localPosition">Local position.</param>
    /// <param name="rotated">If set to <c>true</c> rotated.</param>
    /// <param name="length">Length.</param>
    void AddGridLine ( Vector3 localPosition, bool rotated, float length )
    {
        GameObject line = GameObject.CreatePrimitive( PrimitiveType.Quad );
        line.transform.parent = grid.transform;
        line.transform.localPosition = localPosition;
        line.transform.localScale = new Vector3( Master.vars.boardLineThickness, length, 1f );
        line.transform.rotation = Quaternion.Euler( Vector3.up * ( rotated ? 90 : 0 ) + Vector3.right * 90f );
    }

    /// <summary>
    /// Gets the board tile nearest to position.
    /// </summary>
    /// <param name="position">The world position to search.</param>
    /// <returns>The tile near position.</returns>
    public BoardTile GetTileAtWorldPosition ( Vector3 position )
    {
        int dimensions = (int)Mathf.Sqrt( tiles.Length );
        Vector3 pos = position - this.transform.position + Vector3.one * ( (float)dimensions / 2f );
        if (pos.x < 0 || pos.x >= dimensions || pos.z < 0 || pos.z >= dimensions)
        {
            pos = -Vector3.one;
        }
        if (pos != -Vector3.one)
        {
            return tiles[(int)pos.x, (int)pos.z];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Initialises the tiles that will make up the board.
    /// </summary> 
    /// <param name="dimensions">The side length of the board.</param>
    public void InitialiseTiles ( int dimensions )
    {
        Vector3 initialPosition = -( new Vector3( 1, 0, 1 ) * ( dimensions / 2f - 0.5f ) );
        tiles = new BoardTile[dimensions, dimensions];

        for (int x = 0; x < dimensions; x++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                BoardTile tile = new GameObject( "Board Tile " + x + " , " + y ).AddComponent<BoardTile>();
                tile.transform.parent = transform;
                tile.transform.localPosition = initialPosition + new Vector3( x, 0, y );
                tile.board = this;
                tile.boardCoordinate = new Vector2( x, y );
                tiles[x, y] = tile;
            }
        }
    }
}
