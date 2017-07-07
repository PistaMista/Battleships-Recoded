using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackView_UIElement : UIElement
{
    public override void Enable ()
    {
        base.Enable();
        UserInterface.managedBattle.selectedPlayer.board.visualModules[2].Enable();
    }

    public override void Disable ()
    {
        base.Disable();
        UserInterface.managedBattle.selectedPlayer.board.visualModules[2].Disable();
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
}
