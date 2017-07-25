using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipPositioner_UIElement : UIElement
{
    Canvas canvas;
    public override void Enable ()
    {
        base.Enable();
        canvas = GetComponent<Canvas>();
        notificationTimeRemaining = 0;
        MoveToCurrentPlayer();
    }

    public override void Disable ()
    {
        base.Disable();
    }

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
        }
        else
        {
            Disable();
        }
    }

    protected override void OnBeginPress ( Vector2 position )
    {
        base.OnBeginPress( position );
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        if (ShipPositioner.currentPlayer != null)
        {
            BoardTile tile = ShipPositioner.currentPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y ) );
            if (tile != null)
            {
                if (tile.containedShip != null)
                {
                    tile.containedShip.transform.position = Vector3.zero;
                    ShipPositioner.RemoveShip( tile.containedShip );
                    nextPlayerButton.gameObject.SetActive( false );
                }
            }
        }
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

        if (tile != null)
        {
            if (ShipPositioner.validTiles.Contains( tile ))
            {
                ShipPositioner.SelectTile( tile );

                if (ShipPositioner.selectedTiles.Count == 0)
                {
                    if (ShipPositioner.shipsToPlace.Count == 0)
                    {
                        nextPlayerButton.gameObject.SetActive( true );
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
        ShipPositioner.currentPlayer.board.visualModules[0].Enable();

        float playerBoardSideLength = Mathf.Sqrt( ShipPositioner.currentPlayer.board.tiles.Length );
        float elevation = 0.5f + ( playerBoardSideLength / 2f ) / Mathf.Atan( Camera.main.fieldOfView / 2f * Mathf.Deg2Rad );

        Cameraman.AddWaypoint( new Cameraman.TargetCameraVector3Value( ShipPositioner.currentPlayer.transform.position + Vector3.up * elevation, 0.3f, Mathf.Infinity ), new Cameraman.TargetCameraVector3Value( Vector3.down, 0.25f, Mathf.Infinity ), 98f, false );
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

    void RefreshBoardVisualModule ()
    {

    }
}
