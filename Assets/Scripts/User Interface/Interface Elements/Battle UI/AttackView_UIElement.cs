using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackView_UIElement : UIElement
{
    Player viewedPlayer;
    public override void Enable ()
    {
        if (UserInterface.managedBattle.activePlayer != UserInterface.managedBattle.selectedPlayer)
        {
            viewedPlayer = UserInterface.managedBattle.selectedPlayer;
            viewedPlayer.board.visualModules[2].Enable();
            base.Enable();
        }
        else
        {

        }
    }

    public override void Disable ()
    {
        if (UserInterface.managedBattle.activePlayer != UserInterface.managedBattle.selectedPlayer || UserInterface.managedBattle.activePlayer == null)
        {
            viewedPlayer.board.visualModules[2].Disable();
            base.Disable();
        }
        else
        {

        }
    }

    public override void OnBattleChange ()
    {
        base.OnBattleChange();
        if (UserInterface.managedBattle.selectedPlayer != null && !gameObject.activeInHierarchy)
        {
            Enable();
        }
        else if (gameObject.activeInHierarchy)
        {
            Disable();
        }
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        if (UserInterface.managedBattle.selectedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 0.1f ) ) == null)
        {
            UserInterface.managedBattle.SelectPlayer( null );
        }
    }
}
