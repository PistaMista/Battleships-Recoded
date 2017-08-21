using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery_TargetingUIElement : TargetingUIElement
{
    public override void Enable ()
    {
        base.Enable();
    }

    public override void Disable ()
    {
        base.Disable();
    }

    protected override void OnFocusableBeginPress ( Vector2 position )
    {
        base.OnFocusableBeginPress( position );
        OnFocusedBeginPress( position );

        if (selectedTargetMarker != null)
        {
            Focus();
        }
    }

    protected override void OnFocusableTap ( Vector2 position )
    {
        BoardTile tile = (BoardTile)CalculateTargetFromScreenPosition( position );
        if (tile != null && targetMarkers.Count < UserInterface.managedBattle.activePlayer.gunTargetCap)
        {
            AddTarget( tile );
        }

        base.OnFocusableTap( position );
    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
        Unfocus();
    }

    protected override object CalculateTargetFromScreenPosition ( Vector2 position )
    {
        BoardTile tile = viewedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y ) );
        if (tile != null && !( tile.hitBy.Contains( UserInterface.managedBattle.activePlayer ) || ( UserInterface.managedBattle.activePlayer == tile.board.owner && tile.hitBy.Count > 0 ) ))
        {
            return tile;
        }

        return null;
    }

    protected override TargetMarker AddTargetMarker ( object target )
    {
        ArtilleryMarkCross_TargetMarker marker = new GameObject( "Marker" ).AddComponent<ArtilleryMarkCross_TargetMarker>();
        return marker;
    }

    protected override void ConfirmTargeting ()
    {
        base.ConfirmTargeting();
        List<BoardTile> targets = new List<BoardTile>();
        foreach (TargetMarker targetMarker in targetMarkers)
        {
            targets.Add( (BoardTile)targetMarker.target );
        }

        UserInterface.managedBattle.ArtilleryAttack( targets.ToArray() );
    }
}
