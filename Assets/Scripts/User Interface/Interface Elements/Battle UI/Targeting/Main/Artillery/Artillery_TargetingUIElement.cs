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
        Target targetTile = CalculateTargetFromScreenPosition( position );
        if (targetTile.target != null && targetMarkers.Count < UserInterface.managedBattle.activePlayer.gunTargetCap)
        {
            AddTarget( targetTile );
        }

        base.OnFocusableTap( position );
    }

    protected override void OnFocusedEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
        Unfocus();
    }

    protected override void OnFocusableEndPress ( Vector2 initialPosition, Vector2 currentPosition )
    {
        base.OnFocusedEndPress( initialPosition, currentPosition );
    }

    protected override Target CalculateTargetFromScreenPosition ( Vector2 position )
    {
        BoardTile tile = viewedPlayer.board.GetTileAtWorldPosition( InputController.ConvertToWorldPoint( position, Camera.main.transform.position.y ) );
        return new Target( tile, !targetMarkers.Find( ( obj ) => (BoardTile)obj.target.target == tile ) && tile != null && !( tile.hitBy.Contains( UserInterface.managedBattle.activePlayer ) || ( UserInterface.managedBattle.activePlayer == tile.board.owner && tile.hitBy.Count > 0 ) ) );
    }

    protected override TargetMarker AddTargetMarker ( Target target )
    {
        ArtilleryMarkCross_TargetMarker marker = new GameObject( "Artillery Marker" ).AddComponent<ArtilleryMarkCross_TargetMarker>();
        return marker;
    }

    protected override void ConfirmTargeting ()
    {
        base.ConfirmTargeting();
        List<BoardTile> targets = new List<BoardTile>();
        foreach (TargetMarker targetMarker in targetMarkers)
        {
            targets.Add( (BoardTile)targetMarker.target.target );
        }

        UserInterface.managedBattle.ArtilleryAttack( targets.ToArray() );
    }
}
