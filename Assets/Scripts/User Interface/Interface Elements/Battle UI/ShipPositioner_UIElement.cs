using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShipPositioner_UIElement : UIElement
{
    Canvas canvas;
    public float shipSelectorSpacing;
    [Range( 0.5f, 2.0f )]
    public float shipSelectorScale;
    public Vector3[] shipSelectorPositions;
    public static ShipType selectedShipType;
    public int[] shipTypeCount;

    public override void Enable ()
    {
        base.Enable();
        canvas = GetComponent<Canvas>();
        notificationTimeRemaining = 0;
        MoveToCurrentPlayer();
        SelectShipType( selectedShipType );
    }

    public override void Disable ()
    {
        base.Disable();
        Destroy( shipPalette );
    }

    void AddShipTypeSelectorPositions ()
    {
        int types = Enum.GetValues( typeof( ShipType ) ).Length;
        shipSelectorPositions = new Vector3[types];
        float scale = ShipPositioner.currentPlayer.board.sideTileLength / 12f * shipSelectorScale;
        float spacing = shipSelectorSpacing * scale + 1;
        Vector3 startingPosition = new Vector3( -( ShipPositioner.currentPlayer.board.sideTileLength / 2 + 5.0f * scale ), 0, spacing * ( types - 1 ) / 2 );

        for (int i = 0; i < types; i++)
        {
            shipSelectorPositions[i] = startingPosition + Vector3.back * i * spacing;
        }
    }

    int lastShipCount;
    protected override void Update ()
    {
        base.Update();
        if (ShipPositioner.currentPlayer != null)
        {
            if (!ShipPositioner.currentPlayer.AI && notificationTimeRemaining == -1)
            {
                MoveToCurrentPlayer();
            }
            else if (notificationTimeRemaining > 0)
            {
                RefreshNotification();
                notificationTimeRemaining -= Time.deltaTime;
                if (notificationTimeRemaining <= 1f)
                {
                    Cameraman.SetAuxiliaryParameter( CameramanAuxParameter.BLUR, 0, 0.5f, Mathf.Infinity );
                }
            }

            if (lastShipCount != ShipPositioner.shipsToPlace.Count)
            {
                SelectShipType( selectedShipType );
                lastShipCount = ShipPositioner.shipsToPlace.Count;
            }
        }
        else
        {
            Disable();
        }
    }

    protected override void OnBeginPress ( Vector2 position )
    {
        base.OnBeginPress( position );
        if (notificationTimeRemaining < 0 && ShipPositioner.shipsToPlace.Count > 0)
        {
            if (ShipPositioner.shipsToPlace[0].length > 1)
            {
                ProcessInputPosition( position );
            }
        }
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        if (ShipPositioner.currentPlayer != null)
        {
            Vector3 worldPosition = InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y );
            BoardTile tile = ShipPositioner.currentPlayer.board.GetTileAtWorldPosition( worldPosition );
            if (tile != null)
            {
                if (tile.containedShip != null)
                {
                    tile.containedShip.transform.position = Vector3.zero;
                    nextPlayerButton.gameObject.SetActive( false );
                    SelectShipType( tile.containedShip.type );
                    ShipPositioner.RemoveShip( tile.containedShip );
                }
                else if (ShipPositioner.shipsToPlace.Count > 0)
                {
                    if (ShipPositioner.shipsToPlace[0].length == 1)
                    {
                        ProcessInputPosition( position );
                    }
                }
            }
            else
            {
                Vector3 localPos = ShipPositioner.currentPlayer.board.transform.InverseTransformPoint( worldPosition );
                if (localPos.x < -ShipPositioner.currentPlayer.board.sideTileLength / 2)
                {
                    float smallestDistance = Mathf.Infinity;
                    int smallestDistanceSelectorID = -1;
                    for (int i = 0; i < shipSelectorPositions.Length; i++)
                    {
                        float distance = Vector3.Distance( shipSelectorPositions[i], localPos );
                        if (distance < smallestDistance)
                        {
                            smallestDistance = distance;
                            smallestDistanceSelectorID = i;
                        }
                    }

                    SelectShipType( ( (ShipType[])Enum.GetValues( typeof( ShipType ) ) )[smallestDistanceSelectorID] );
                }
            }
        }
    }

    void SelectShipType ( ShipType type )
    {
        shipTypeCount = new int[shipSelectorPositions.Length];
        List<Ship> toMove = new List<Ship>();

        foreach (Ship ship in ShipPositioner.shipsToPlace)
        {
            if (ship.type == type)
            {
                toMove.Add( ship );
            }
            shipTypeCount[(int)ship.type]++;
        }

        foreach (Ship ship in toMove)
        {
            ShipPositioner.shipsToPlace.Remove( ship );
            ShipPositioner.shipsToPlace.Insert( 0, ship );
        }

        selectedShipType = type;
        DrawShipPalette();
    }


    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
        if (notificationTimeRemaining < 0)
        {
            ProcessInputPosition( currentPosition );
        }
    }

    protected override void OnEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnEndPress( initialPosition, currentPosition );
        ShipPositioner.selectedTiles = new List<BoardTile>();
        ShipPositioner.ValidateTiles();
    }

    void ProcessInputPosition ( Vector2 screenInputPosition )
    {
        Vector3 worldPosition = InputController.ConvertToWorldPoint( screenInputPosition, Camera.main.transform.position.y );
        BoardTile tile = ShipPositioner.currentPlayer.board.GetTileAtWorldPosition( worldPosition );

        if (tile != null && shipTypeCount[(int)selectedShipType] > 0)
        {
            if (ShipPositioner.validTiles.Contains( tile ))
            {
                ShipPositioner.SelectTile( tile );

                if (ShipPositioner.selectedTiles.Count == 0)
                {
                    if (ShipPositioner.shipsToPlace.Count == 0)
                    {
                        nextPlayerButton.gameObject.SetActive( true );
                        Destroy( shipPalette );
                    }
                }
            }
        }
    }

    //Notification effect
    public Text playerNameText;
    Vector2 nameTextVelocity;
    public Text instructionText;
    public Button nextPlayerButton;
    Vector2 instructionTextVelocity;
    float notificationTimeRemaining = -1f;
    void MoveToCurrentPlayer ()
    {
        nextPlayerButton.gameObject.SetActive( false );
        playerNameText.text = ShipPositioner.currentPlayer.label;
        playerNameText.rectTransform.anchoredPosition = Vector2.right * ( Screen.width / 2f + playerNameText.rectTransform.rect.width / 2f ) + Vector2.down * ( canvas.GetComponent<RectTransform>().rect.height / 2f - playerNameText.rectTransform.rect.height / 2f );
        instructionText.rectTransform.anchoredPosition = Vector2.left * ( Screen.width / 2f + instructionText.rectTransform.rect.width / 2f ) + Vector2.up * ( canvas.GetComponent<RectTransform>().rect.height / 2f - instructionText.rectTransform.rect.height / 2f );
        notificationTimeRemaining = 3f;

        ShipPositioner.currentPlayer.board.ResetVisualModules();
        ShipPositioner.currentPlayer.board.ReinitializeGrid();
        AddShipTypeSelectorPositions();
        ShipPositioner.currentPlayer.board.visualModules[0].Enable();

        float playerBoardSideLength = Mathf.Sqrt( ShipPositioner.currentPlayer.board.tiles.Length );
        float elevation = 0.5f + ( playerBoardSideLength / 2f ) / Mathf.Atan( Camera.main.fieldOfView / 2f * Mathf.Deg2Rad );

        Cameraman.AddWaypoint( new Cameraman.TargetCameraVector3Value( ShipPositioner.currentPlayer.transform.position + Vector3.up * elevation + Vector3.left * ShipPositioner.currentPlayer.board.sideTileLength / 3.0f, 0.3f, Mathf.Infinity ), new Cameraman.TargetCameraVector3Value( Vector3.down, 0.25f, Mathf.Infinity ), 98f, false );
    }

    void RefreshNotification ()
    {
        if (notificationTimeRemaining > 1f)
        {
            playerNameText.rectTransform.anchoredPosition = Vector2.SmoothDamp( playerNameText.rectTransform.anchoredPosition, Vector2.down * ( canvas.GetComponent<RectTransform>().rect.height / 2f - playerNameText.rectTransform.rect.height / 2f ), ref nameTextVelocity, 0.65f, Mathf.Infinity, Time.deltaTime );
            instructionText.rectTransform.anchoredPosition = Vector2.SmoothDamp( instructionText.rectTransform.anchoredPosition, Vector2.up * ( canvas.GetComponent<RectTransform>().rect.height / 2f - instructionText.rectTransform.rect.height / 2f ), ref instructionTextVelocity, 0.65f, Mathf.Infinity, Time.deltaTime );
            Cameraman.SetAuxiliaryParameter( CameramanAuxParameter.BLUR, 5f, 0.3f, Mathf.Infinity );
        }
        else
        {
            playerNameText.rectTransform.anchoredPosition = Vector2.SmoothDamp( playerNameText.rectTransform.anchoredPosition, Vector2.down * playerNameText.rectTransform.rect.height / 1.5f, ref nameTextVelocity, 0.15f, Mathf.Infinity, Time.deltaTime );
            instructionText.rectTransform.anchoredPosition = Vector2.SmoothDamp( instructionText.rectTransform.anchoredPosition, Vector2.up * instructionText.rectTransform.rect.height / 1.5f, ref instructionTextVelocity, 0.15f, Mathf.Infinity, Time.deltaTime );
            Cameraman.SetAuxiliaryParameter( CameramanAuxParameter.BLUR, 0f, 0.3f, Mathf.Infinity );
        }
    }

    public void NextPlayer ()
    {
        if (ShipPositioner.NextPlayer())
        {
            notificationTimeRemaining = -1f;
        }
        else
        {
            UserInterface.managedBattle.StartBattle();
        }
    }

    /// <summary>
    /// The ship palette.
    /// </summary>
    GameObject shipPalette;
    /// <summary>
    /// Draws the ship palette.
    /// </summary>
    void DrawShipPalette ()
    {
        Destroy( shipPalette );
        shipPalette = new GameObject( "Ship Palette" );
        shipPalette.transform.SetParent( transform );
        shipPalette.transform.position = ShipPositioner.currentPlayer.transform.position;

        for (int i = 0; i < shipSelectorPositions.Length; i++)
        {
            Ship ship = Instantiate( Master.vars.shipPrefabs[i] ).GetComponent<Ship>();

            ship.transform.SetParent( shipPalette.transform );
            ship.transform.localPosition = shipSelectorPositions[i];
            ship.transform.localScale *= ShipPositioner.currentPlayer.board.sideTileLength / 12f * shipSelectorScale;
            ;
            ship.transform.localRotation = Quaternion.Euler( Vector3.up * 90 );

            float spacing = 1.2f;
            float initialPosition = -( ship.length - 1 ) * spacing / 2;
            for (int x = 0; x < ship.length; x++)
            {
                GameObject cube = GameObject.CreatePrimitive( PrimitiveType.Cube );
                cube.transform.SetParent( ship.transform );
                cube.transform.localScale = new Vector3( 1, 0.1f, 1 );
                cube.transform.localPosition = Vector3.forward * ( initialPosition + x * spacing );
                cube.GetComponent<Renderer>().material = ( (int)selectedShipType == i && shipTypeCount[i] > 0 ) ? Master.vars.shipPlacementSelectedShipMaterial : Master.vars.shipPlacementNotSelectedShipMaterial;
            }

            TextMesh textMesh = new GameObject( "Ship Count Label" ).AddComponent<TextMesh>();
            textMesh.transform.SetParent( ship.transform );
            textMesh.transform.localPosition = Vector3.forward * ( ship.length / 2f + 1.2f );
            textMesh.transform.localScale = Vector3.one / 10f;
            textMesh.transform.rotation = Quaternion.Euler( Vector3.right * 90 );
            textMesh.font = Master.vars.defaultFont;
            textMesh.fontSize = 200;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.text = "x" + shipTypeCount[i];

            textMesh.GetComponent<Renderer>().material = Master.vars.fontMaterial;
        }
    }
}
