using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackViewMisc_AttackViewUIElement : AttackViewUIElement
{
    public override void Enable ()
    {
        base.Enable();
        viewedPlayer.board.visualModules[2].Enable();
        viewedPlayer.board.visualModules[1].Enable();
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
        if (viewedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 0.1f ) ) == null)
        {
            UserInterface.managedBattle.SelectPlayer( null );
        }
    }
}
