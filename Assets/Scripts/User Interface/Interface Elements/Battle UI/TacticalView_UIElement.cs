using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalView_UIElement : UIElement
{
    MainBattle battle;
    Vector3 overheadCameraPosition;

    public override void Enable ()
    {
        base.Enable();
        battle = UserInterface.managedBattle;
        overheadCameraPosition.y = Master.vars.mainBattleBoardDistance * 1.2f / Mathf.Tan( Camera.main.fieldOfView * Mathf.Deg2Rad / 2 );
    }

    public override void Disable ()
    {
        base.Disable();
    }

    public override void OnBattleChange ()
    {
        base.OnBattleChange();
        if (UserInterface.managedBattle.activePlayer != null && !gameObject.activeInHierarchy)
        {
            Enable();
        }
        else if (( UserInterface.managedBattle.activePlayer == null || UserInterface.managedBattle.selectedPlayer != null ) && gameObject.activeInHierarchy)
        {
            if (UserInterface.managedBattle.activePlayer != null)
            {
                if (!UserInterface.managedBattle.activePlayer.AI)
                {
                    Disable();
                }
            }
            else
            {
                Disable();
            }
        }

        if (gameObject.activeInHierarchy)
        {
            foreach (Player player in UserInterface.managedBattle.combatants)
            {
                player.board.ResetVisualModules();
            }

            if (battle.activePlayer != null)
            {
                if (battle.selectedPlayer == null || battle.activePlayer.AI)
                {
                    Cameraman.AddWaypoint( overheadCameraPosition, Vector3.down, 1.2f, Mathf.Infinity, 0, false );
                    foreach (Player player in battle.combatants)
                    {
                        player.board.visualModules[1].Enable();
                    }
                }
            }
        }
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        Debug.Log( "Tap" );
        if (battle.selectedPlayer == null)
        {
            Vector3 worldPosition = InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 1 );
            foreach (Player player in battle.combatants)
            {
                Bounds tagBounds = ( (TacticalView_BoardVisualModule)player.board.visualModules[1] ).textBounds;
                Vector3 lowerCorner = player.board.transform.position - tagBounds.extents;
                Vector3 upperCorner = player.board.transform.position + tagBounds.extents;

                if (worldPosition.x > lowerCorner.x && worldPosition.x < upperCorner.x && worldPosition.z > lowerCorner.z && worldPosition.z < upperCorner.z)
                {
                    battle.SelectPlayer( player );
                    break;
                }
            }
        }
    }

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
        Debug.Log( "Drag" );
    }

    protected override void Update ()
    {
        base.Update();
    }

    public void BackToTitle ()
    {
        BattleSaver.SaveCurrentBattle();
        Destroy( UserInterface.managedBattle.gameObject );
        foreach (UIElement element in UserInterface.elements)
        {
            if (element != this)
            {
                element.Disable();
            }
        }

        Disable();
        UserInterface.elements[0].Enable();
    }
}
