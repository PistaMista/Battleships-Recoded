using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPositioner_BoardVisualModule : BoardVisualModule
{
    public override void Initialize ()
    {
        base.Initialize();
    }

    public override void Enable ()
    {
        base.Enable();
        previouslySelectedTiles = 0;
        indicator = new GameObject( "Indicator" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
        indicator.material = Master.vars.shipPlacementIndicatorMaterial;
        indicator.transform.parent = visualParent.transform;
    }

    public override void Disable ()
    {
        base.Disable();
        Destroy( indicator );
    }
    ShipPlacementUIIndicator deprecated;
    DynamicStripedRectangle_GraphicsElement indicator;

    int previouslySelectedTiles;
    public override void Refresh ()
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
                List<BoardTile> selectedTiles = ShipPositioner.selectedTiles;
                if (selectedTiles.Count > 0)
                {
                    indicator.gameObject.SetActive( true );
                    previouslySelectedTiles = selectedTiles.Count;
                    //deprecated.Adapt();

                    Vector3 relative = selectedTiles[selectedTiles.Count - 1].transform.position - selectedTiles[0].transform.position;
                    indicator.transform.position = relative / 2f + selectedTiles[0].transform.position + Vector3.up * 0.12f;

                    indicator.Set( new Vector2( Mathf.Clamp( Mathf.Abs( relative.x ) + 1, 1, Mathf.Infinity ), Mathf.Clamp( Mathf.Abs( relative.z ) + 1, 1, Mathf.Infinity ) ), 0.1f, true, 0.5f, 0.1f, 0.1f );
                }
                else
                {
                    indicator.gameObject.SetActive( false );
                }
            }
        }
    }

}
