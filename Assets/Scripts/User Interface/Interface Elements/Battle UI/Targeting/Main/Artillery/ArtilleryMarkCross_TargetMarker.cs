using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMarkCross_TargetMarker : TargetMarker
{
    public override bool PositionIntersects ( Vector3 worldPosition )
    {
        BoardTile t = (BoardTile)target.target;
        return t.board.GetTileAtWorldPosition( worldPosition ) == t;
    }

    public override void SetVisualsForTarget ( TargetingUIElement.Target target )
    {
        if (main == null)
        {
            ArtilleryMarkCross_DeployableGraphicsElement cross = new GameObject( "Artillery Marker Cross" ).AddComponent<ArtilleryMarkCross_DeployableGraphicsElement>();
            cross.transform.SetParent( transform );
            cross.MainMaterial = Master.vars.targetValidMaterial;
            cross.Set( 0.35f, 1.65f, 0.85f, 1.0f );
            cross.transform.rotation = Quaternion.Euler( new Vector3( 90, 45, 0 ) );
            main = cross;
        }

        main.transform.position = ( (BoardTile)target.target ).transform.position + Vector3.up * 0.111f;
        base.SetVisualsForTarget( target );
    }
}
