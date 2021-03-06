﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBattle : Battle
{
    public Player lastSelectedPlayer;
    public bool singleplayer;

    public override void Initialise ( Player[] combatants, bool functional )
    {
        base.Initialise( combatants, functional );
        UserInterface.managedBattle = this;
        singleplayer = false;
        foreach (Player player in combatants)
        {
            if (!player.AI)
            {
                if (!singleplayer)
                {
                    singleplayer = true;
                }
                else
                {
                    singleplayer = false;
                    break;
                }
            }
        }
    }

    public override void StartBattle ()
    {
        base.StartBattle();
        UserInterface.RespondToBattleChanges();
    }

    public override void BeginTurn ()
    {
        base.BeginTurn();
        UserInterface.RespondToBattleChanges();
        resultsShown = false;
        if (activePlayer.AI)
        {
            prepareTime = 2;
        }
    }

    public override void EndTurn ()
    {
        Aircraft_FocusableUIElement.FinalizeTargeting();
        base.EndTurn();
        UserInterface.RespondToBattleChanges();
    }

    bool resultsShown;
    public override void OnVisualFinish ()
    {
        if (( !turnLog[0].activePlayer.AI || !turnLog[0].attackedPlayer.AI ) && !resultsShown)
        {
            UserInterface.elements[6].Enable();
            resultsShown = true;
        }
        else
        {
            base.OnVisualFinish();
            UserInterface.RespondToBattleChanges();
        }
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

    public override void ArtilleryAttack ( BoardTile[] tiles )
    {
        foreach (Player player in UserInterface.managedBattle.combatants)
        {
            player.board.ResetVisualModules();
        }
        base.ArtilleryAttack( tiles );
    }

    protected override void Update ()
    {
        base.Update();
        if (Input.GetKeyDown( KeyCode.Space ))
        {
            Debug.Log( BattleSaver.SaveCurrentBattle() );
        }
    }
}
