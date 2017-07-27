using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        indicator.transform.SetParent( visualParent.transform );
    }

    public override void Disable ()
    {
        base.Disable();
        Destroy( indicator );
    }


    public override void Refresh ()
    {
        base.Refresh();
        if (ShipPositioner.currentPlayer != board.owner)
        {
            Disable();
        }
        else
        {
            DrawShipPlacementIndicator();
            HighlightPlacedShips();
        }
    }

    DynamicStripedRectangle_GraphicsElement indicator;
    int previouslySelectedTiles;
    void DrawShipPlacementIndicator ()
    {
        if (ShipPositioner.selectedTiles.Count != previouslySelectedTiles)
        {
            List<BoardTile> selectedTiles = ShipPositioner.selectedTiles;
            if (selectedTiles.Count > 0)
            {
                indicator.gameObject.SetActive( true );
                previouslySelectedTiles = selectedTiles.Count;

                Vector3 relative = selectedTiles[selectedTiles.Count - 1].transform.position - selectedTiles[0].transform.position;
                indicator.transform.position = relative / 2f + selectedTiles[0].transform.position + Vector3.up * 0.12f;

                indicator.Set( new Vector2( Mathf.Clamp( Mathf.Abs( relative.x ) + 1, 1, Mathf.Infinity ), Mathf.Clamp( Mathf.Abs( relative.z ) + 1, 1, Mathf.Infinity ) ), 0.1f, true, 0.2f, 0.5f, 0.1f, 0.1f );
            }
            else
            {
                indicator.gameObject.SetActive( false );
            }
        }
    }

    GameObject highlightParent;
    int previouslyPlacedShips;
    void HighlightPlacedShips ()
    {
        if (ShipPositioner.shipsToPlace.Count != previouslyPlacedShips)
        {
            Destroy( highlightParent );
            highlightParent = new GameObject( "Highlight Parent" );
            highlightParent.transform.SetParent( visualParent.transform );
            highlightParent.transform.localPosition = Vector3.zero;

            foreach (Ship ship in board.owner.ships)
            {
                if (ship.gameObject.activeInHierarchy)
                {
                    Vector3 relative = ship.tiles[0].transform.localPosition - ship.tiles[ship.tiles.Length - 1].transform.localPosition;
                    Rectangle_GraphicsElement rectangle = new GameObject( "Highlight Rectangle for " + ship.name ).AddComponent<Rectangle_GraphicsElement>();
                    rectangle.transform.SetParent( highlightParent.transform );
                    rectangle.transform.position = ship.transform.position + Vector3.up * 0.11f;
                    rectangle.material = Master.vars.shipPlacementIndicatorMaterial;

                    float modifier = 1f - 0.1f;
                    rectangle.Set( new Vector2( Mathf.Abs( relative.x ), Mathf.Abs( relative.z ) ) + Vector2.one * modifier, 0.1f, true, 0.2f );
                }
            }
            previouslyPlacedShips = ShipPositioner.shipsToPlace.Count;
        }
    }


}
