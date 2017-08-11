using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackViewMisc_AttackViewUIElement : AttackViewUIElement
{
    Player viewedPlayer;
    public override void Enable ()
    {

        viewedPlayer = UserInterface.managedBattle.selectedPlayer;
        viewedPlayer.board.visualModules[2].Enable();
        viewedPlayer.board.visualModules[1].Enable();
        base.Enable();
    }

    public override void Disable ()
    {

        if (viewedPlayer != null)
        {
            viewedPlayer.board.visualModules[2].Disable();
        }
        base.Disable();
    }

    protected override void OnFocusableTap ( Vector2 position )
    {
        base.OnFocusableTap( position );
        if (UserInterface.managedBattle.selectedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 0.1f ) ) == null)
        {
            UserInterface.managedBattle.SelectPlayer( null );
        }
    }
}
