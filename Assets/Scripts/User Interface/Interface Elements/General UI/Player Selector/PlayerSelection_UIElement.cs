using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;

public class PlayerSelection_UIElement : Slidable_UIElement
{

    public float paneWidth;
    public float reservedSpace;
    public float paneSpacing;
    public RectTransform addPlayerButton;
    public Button resumeLastBattleButton;
    public Text saveFileCorruptedText;
    public GameObject panelPrefab;
    public List<PlayerPanel> panels;

    float targetAddPlayerButtonXPosition;
    Vector2 addPlayerButtonVelocity;


    public override void Enable ()
    {
        base.Enable();
        Cameraman.SetAuxiliaryParameter( CameramanAuxParameter.BLUR, 5f, 1f, Mathf.Infinity );
        reservedSpace = 1200f - addPlayerButton.GetComponent<Image>().rectTransform.rect.width;
        carrier.anchoredPosition = originalPosition;
        if (panels == null)
        {
            panels = new List<PlayerPanel>();
        }
        RefreshGraphics();

        resumeLastBattleButton.gameObject.SetActive( false );
        saveFileCorruptedText.gameObject.SetActive( false );

        switch (BattleLoader.LoadData())
        {
            case 0:
                resumeLastBattleButton.gameObject.SetActive( true );
                break;
            case 2:
                saveFileCorruptedText.gameObject.SetActive( true );
                break;
        }
    }

    public override void Disable ()
    {
        base.Disable();
        while (panels.Count > 0)
        {
            RemovePlayerPanel( panels[0] );
        }
    }

    protected override void Update ()
    {
        base.Update();
        addPlayerButton.anchoredPosition = Vector2.SmoothDamp( addPlayerButton.anchoredPosition, Vector2.right * targetAddPlayerButtonXPosition, ref addPlayerButtonVelocity, 1f, Mathf.Infinity, Time.deltaTime );
    }

    /// <summary>
    /// Removes a player panel.
    /// </summary>
    /// <param name="panel"></param>
    public void RemovePlayerPanel ( PlayerPanel panel )
    {
        Destroy( panel.gameObject );
        panels.Remove( panel );
        RefreshGraphics();
    }

    /// <summary>
    /// Refreshes the graphics of the UI.
    /// </summary>
    void RefreshGraphics ()
    {
        if (panels.Count > 0)
        {
            float position = 0;
            if (panels.Count < 3)
            {
                addPlayerButton.gameObject.SetActive( true );
                targetAddPlayerButtonXPosition = 440;
                position = reservedSpace - 320f;
            }
            else
            {
                addPlayerButton.gameObject.SetActive( false );
                position = 950f;
            }
            foreach (PlayerPanel panel in panels)
            {
                panel.targetX = position;
                position -= 320f + paneSpacing;
            }
        }
        else
        {
            targetAddPlayerButtonXPosition = 0;
        }
    }

    /// <summary>
    /// Adds another player.
    /// </summary>
    public void AddPlayer ()
    {
        if (panels.Count < 3)
        {
            PlayerPanel newPanel = Instantiate( panelPrefab ).GetComponent<PlayerPanel>();
            newPanel.name.text = "Player " + ( panels.Count + 1 );
            newPanel.transform.parent = carrier.transform;
            newPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.left * 200f;
            panels.Add( newPanel );
            RefreshGraphics();
        }
    }

    /// <summary>
    /// Resumes the last battle.
    /// </summary>
    public void ResumeLastBattle ()
    {
        BattleLoader.ReconstructBattle();
        resumeLastBattleButton.gameObject.SetActive( false );
        LockInput();
        targetPosition.x = -1200;
    }

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        if (Mathf.Abs( currentPosition.x - initialPosition.x ) > Screen.width / 4.5f)
        {
            base.OnDrag( initialPosition, currentPosition );
            targetPosition.x = ( currentPosition.x - initialPosition.x );
        }
    }

    protected override void OnEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnEndPress( initialPosition, currentPosition );
        float rawDistance = currentPosition.x - initialPosition.x;
        if (Mathf.Abs( rawDistance ) > Screen.width / 3.5f)
        {
            if (rawDistance > 0)
            {
                targetPosition.x = 1200;


                InputController.onEndPress -= OnEndPress;
                InputController.onDrag -= OnDrag;
                UserInterface.elements[0].Enable();
            }
            else if (panels.Count > 1)
            {
                targetPosition.x = -1200;

                InputController.onEndPress -= OnEndPress;
                InputController.onDrag -= OnDrag;

                StartBattle();
                LockInput();
            }
            else
            {
                targetPosition.x = 0f;
            }
        }
        else
        {
            targetPosition.x = 0f;
        }
    }

    /// <summary>
    /// Starts the battle.
    /// </summary>
    void StartBattle ()
    {
        Battle battle = new GameObject( "Main Battle" ).AddComponent<MainBattle>();
        Player[] players = new Player[panels.Count];
        int humans = 0;

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObject = new GameObject( "Player - " + panels[i].name.text );
            Player player;
            if (panels[i].AI.isOn)
            {
                player = playerObject.AddComponent<Player_AI>();
            }
            else
            {
                player = playerObject.AddComponent<Player_Human>();
                humans++;
            }

            player.ID = i;
            player.AI = panels[i].AI.isOn;
            player.label = panels[i].name.text;
            if (player.label.Length == 0)
            {
                player.label = "Player " + ( i + 1 );
            }
            player.board.InitialiseTiles( (int)panels[i].boardSizeSlider.value );
            player.battle = battle;
            players[i] = player;

            float angle = 360f / players.Length * i;
            player.transform.position = new Vector3( Mathf.Cos( angle * Mathf.Deg2Rad ), 0, Mathf.Sin( angle * Mathf.Deg2Rad ) ) * Master.vars.mainBattleBoardDistance;
        }

        for (int i = 0; i < players.Length; i++)
        {
            for (int x = 0; x < players.Length; x++)
            {
                players[i].hits.Add( players[x], new Dictionary<BoardTile, TileHitInformation>() );
            }
        }

        battle.Initialise( players, false );
        if (humans > 0)
        {
            UserInterface.elements[3].Enable();
        }
        else
        {
            Cameraman.SetAuxiliaryParameter( CameramanAuxParameter.BLUR, 0, 1, Mathf.Infinity );
        }
    }

    void LockInput ()
    {
        InputController.onBeginPress -= OnBeginPress;
        InputController.onTap -= OnTap;
        InputController.onDrag -= OnDrag;
        InputController.onEndPress -= OnEndPress;
    }
}
