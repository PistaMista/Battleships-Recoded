using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMarkCross_TargetMarker : TargetMarker
{
    public override bool PositionIntersects ( Vector3 worldPosition )
    {
        BoardTile t = (BoardTile)target;
        return t.board.GetTileAtWorldPosition( worldPosition ) == t;
    }
}
