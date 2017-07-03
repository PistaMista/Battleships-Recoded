using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_UIElement : UIElement
{
    protected override void Update()
    {
        base.Update();
        if (UserInterface.managedBattle == null)
        {
            Disable();
        }
        else
        {
            RefreshBattleUI();
        }
    }

    protected virtual void RefreshBattleUI()
    {

    }
}
