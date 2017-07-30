using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackView_UIElement : UIElement
{
    Player viewedPlayer;
    public override void Enable ()
    {

        viewedPlayer = UserInterface.managedBattle.selectedPlayer;
        viewedPlayer.board.visualModules[2].Enable();
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

    public override void OnBattleChange ()
    {
        base.OnBattleChange();
        if (UserInterface.managedBattle.selectedPlayer != null && !gameObject.activeInHierarchy && !UserInterface.managedBattle.activePlayer.AI)
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
        if (UserInterface.managedBattle.selectedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y - 0.1f ) ) == null && ArtilleryTargeting_UIElement.candidate == null && TorpedoTargeting_UIElement.candidate == Vector3.zero)
        {
            UserInterface.managedBattle.SelectPlayer( null );
        }
    }
}
