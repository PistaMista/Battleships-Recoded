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

    public override void SetVisualsForTarget ( object target )
    {
        base.SetVisualsForTarget( target );

        ArtilleryMarkCross_DeployableGraphicsElement cross = new GameObject( "Artillery Marker Cross" ).AddComponent<ArtilleryMarkCross_DeployableGraphicsElement>();
        cross.transform.SetParent( transform );
        cross.mainMaterial = Master.vars.targetingUnconfirmedMaterial;
        cross.Set( 0.1f, 1.2f, 1.2f, 1.0f );
        cross.transform.position = ( (BoardTile)target ).transform.position + Vector3.up * 0.111f;
        main = cross;
    }
}
