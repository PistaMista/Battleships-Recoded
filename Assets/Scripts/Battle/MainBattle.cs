using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattle : Battle
{
    public Player lastSelectedPlayer;
    public override void Initialise ( Player[] combatants, bool functional )
    {
        base.Initialise( combatants, functional );
        UserInterface.managedBattle = this;
    }

    public override void StartBattle ()
    {
        base.StartBattle();
        UserInterface.RespondToBattleChanges();
    }

    protected override void BeginTurn ()
    {
        base.BeginTurn();
        UserInterface.RespondToBattleChanges();
    }

    protected override void EndTurn ()
    {
        base.EndTurn();
        UserInterface.RespondToBattleChanges();
    }

    public override void OnVisualFinish ()
    {
        base.OnVisualFinish();
        UserInterface.RespondToBattleChanges();
    }

    public override void SelectPlayer ( Player player )
    {
        lastSelectedPlayer = selectedPlayer;
        base.SelectPlayer( player );
        UserInterface.RespondToBattleChanges();
    }

    protected override void OnBattleFinish ()
    {
        base.OnBattleFinish();
        UserInterface.elements[4].Disable();
    }
}
