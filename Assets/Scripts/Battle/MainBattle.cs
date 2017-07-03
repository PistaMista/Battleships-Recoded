using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattle : Battle
{
    public override void Initialise(Player[] combatants, bool functional)
    {
        base.Initialise(combatants, functional);
        UserInterface.managedBattle = this;
    }

    public override void StartBattle()
    {
        UserInterface.elements[3].Disable();
    }

    protected override void BeginTurn()
    {
        base.BeginTurn();
        UserInterface.RespondToBattleChanges();
    }

    protected override void EndTurn()
    {
        base.EndTurn();
        UserInterface.RespondToBattleChanges();
    }

    public override void OnVisualFinish()
    {
        base.OnVisualFinish();
        UserInterface.RespondToBattleChanges();
    }
}
