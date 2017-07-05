using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalView_UIElement : UIElement
{
    MainBattle battle;
    Vector3 overheadCameraPosition;

    public override void Enable ()
    {
        base.Enable();
        battle = UserInterface.managedBattle;
        overheadCameraPosition.y = Master.vars.mainBattleBoardDistance * 1.2f / Mathf.Tan( Camera.main.fieldOfView * Mathf.Deg2Rad / 2 );
    }

    public override void Disable ()
    {
        base.Disable();
    }

    public override void OnBattleChange ()
    {
        base.OnBattleChange();
        if (UserInterface.managedBattle.activePlayer != null && !gameObject.activeInHierarchy)
        {
            Enable();
        }
        else if (gameObject.activeInHierarchy)
        {
            Disable();
        }

        if (gameObject.activeInHierarchy)
        {
            foreach (Player player in UserInterface.managedBattle.combatants)
            {
                player.board.ResetVisualModules();
            }

            if (battle.activePlayer != null)
            {
                if (battle.selectedPlayer == null)
                {
                    Cameraman.AddWaypoint( overheadCameraPosition, Vector3.down, 3f, Mathf.Infinity, false );
                    foreach (Player player in battle.combatants)
                    {
                        player.board.visualModules[1].Enable();
                    }
                }
            }
        }
    }

    protected override void OnTap ( Vector2 position )
    {
        base.OnTap( position );
    }

    protected override void OnDrag ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnDrag( initialPosition, currentPosition );
    }

    protected override void Update ()
    {
        base.Update();
    }
}
