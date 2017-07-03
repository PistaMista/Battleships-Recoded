using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositioner_BoardVisualModule : BoardVisualModule
{
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Enable()
    {
        base.Enable();
        previouslySelectedTiles = 0;
        indicator = Instantiate(Master.vars.shipPlacementIndicator).GetComponent<ShipPlacementUIIndicator>();
        indicator.transform.parent = visualParent.transform;
    }

    public override void Disable()
    {
        base.Disable();
        Destroy(indicator);
    }
    ShipPlacementUIIndicator indicator;
    int previouslySelectedTiles;
    public override void Refresh()
    {
        base.Refresh();
        if (ShipPositioner.currentPlayer != board.owner)
        {
            Disable();
        }
        else
        {
            if (ShipPositioner.selectedTiles.Count != previouslySelectedTiles)
            {
                previouslySelectedTiles = ShipPositioner.selectedTiles.Count;
                indicator.Adapt();
            }
        }
    }

}
