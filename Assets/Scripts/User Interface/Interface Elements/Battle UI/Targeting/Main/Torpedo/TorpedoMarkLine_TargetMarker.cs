using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoMarkLine_TargetMarker : TargetMarker
{
    int lastTorpedoCount;

    public override bool PositionIntersects ( Vector3 worldPosition )
    {
        return base.PositionIntersects( worldPosition );
    }

    public override void SetVisualsForTarget ( object target )
    {
        Torpedo_TargetingUIElement.TorpedoTarget torpedoTarget = (Torpedo_TargetingUIElement.TorpedoTarget)target;
        if (main == null)
        {
            ExtendingMultiLine_GraphicsElement markLine = new GameObject( "Torpedo Marker Line" ).AddComponent<ExtendingMultiLine_GraphicsElement>();
            markLine.transform.SetParent( transform );
            markLine.mainMaterial = Master.vars.targetingUnconfirmedMaterial;
            markLine.Reset( 0.1f, 30f, 0.05f, 0.75f, 1.0f, true );
            markLine.transform.position = UserInterface.managedBattle.torpedoLaunchPosition;
            markLine.transform.rotation = Quaternion.Euler( 90, 0, 0 );
            main = markLine;
        }

        if (torpedoTarget.torpedoCount > lastTorpedoCount)
        {
            ( (ExtendingMultiLine_GraphicsElement)main ).AddLine();
        }
        else if (torpedoTarget.torpedoCount < lastTorpedoCount)
        {
            ( (ExtendingMultiLine_GraphicsElement)main ).RemoveLine();
        }
        lastTorpedoCount = torpedoTarget.torpedoCount;

        main.transform.rotation = Quaternion.LookRotation( torpedoTarget.direction );

        Debug.Log( torpedoTarget.direction );
    }

    public override void StartMove ()
    {
        base.StartMove();
    }

    public override void Moving ()
    {
        base.Moving();
        Debug.Log( "Torpedo Targeting Line Moving" );
    }

    public override void EndMove ()
    {
        base.EndMove();
    }
}
