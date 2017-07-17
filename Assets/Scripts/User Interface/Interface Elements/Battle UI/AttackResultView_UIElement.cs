using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResultView_UIElement : UIElement
{

    public override void Enable ()
    {
        base.Enable();
        UserInterface.managedBattle.turnLog[0].attackedPlayer.board.visualModules[3].Enable();
    }

    public override void Disable ()
    {
        if (gameObject.activeInHierarchy)
        {
            base.Disable();
            UserInterface.managedBattle.turnLog[0].attackedPlayer.board.visualModules[3].Disable();
            UserInterface.managedBattle.OnVisualFinish();
        }
    }

    public override void OnBattleChange ()
    {
        base.OnBattleChange();
        Disable();
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
        Disable();
    }
}
