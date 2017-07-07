using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackView_UIElement : UIElement
{
    public override void Enable ()
    {
        base.Enable();
        if (UserInterface.managedBattle.activePlayer != UserInterface.managedBattle.selectedPlayer)
        {
            UserInterface.managedBattle.selectedPlayer.board.visualModules[2].Enable();
        }
        else
        {

        }
    }

    public override void Disable ()
    {
        base.Disable();
        if (UserInterface.managedBattle.activePlayer != UserInterface.managedBattle.selectedPlayer)
        {
            UserInterface.managedBattle.selectedPlayer.board.visualModules[2].Disable();
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
}
