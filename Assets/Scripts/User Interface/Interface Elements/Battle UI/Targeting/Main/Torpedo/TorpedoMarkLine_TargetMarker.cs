using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoMarkLine_TargetMarker : TargetMarker
{
    int lastTorpedoCount;

    public override bool PositionIntersects ( Vector3 worldPosition )
    {
        Vector3 localPos = main.transform.InverseTransformPoint( worldPosition );
        Debug.Log( "Torpedo target marker:" + localPos );
        return Mathf.Abs( localPos.x ) < 0.2f;
    }

    public override void SetVisualsForTarget ( TargetingUIElement.Target target )
    {
        int torpedoCount = ( (Torpedo_TargetingUIElement.TorpedoTarget)target.target ).torpedoCount;
        if (main == null)
        {
            ExtendingMultiLine_GraphicsElement markLine = new GameObject( "Torpedo Marker Line" ).AddComponent<ExtendingMultiLine_GraphicsElement>();
            markLine.transform.SetParent( transform );
            markLine.MainMaterial = Master.vars.targetValidMaterial;
            markLine.Reset( 0.1f, 60.0f, 0.1f, 0.35f, 1.0f, true );
            markLine.transform.position = UserInterface.managedBattle.torpedoLaunchPosition + Vector3.up * 0.112f;
            markLine.transform.rotation = Quaternion.Euler( 90, 0, 0 );
            main = markLine;
        }

        ( (ExtendingMultiLine_GraphicsElement)main ).SetLineCount( torpedoCount );
        lastTorpedoCount = torpedoCount;
        main.transform.rotation = Quaternion.LookRotation( ( (Torpedo_TargetingUIElement.TorpedoTarget)target.target ).direction );
        base.SetVisualsForTarget( target );
    }

    public override void StartMove ()
    {
        base.StartMove();
    }

    public override void Moving ()
    {
        base.Moving();
    }

    public override void EndMove ()
    {
        base.EndMove();
    }
}
