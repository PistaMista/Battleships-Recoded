using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResultView_BoardVisualModule : BoardVisualModule
{
    public override void Enable ()
    {
        base.Enable();
        //board.visualModules[1].Enable();
        board.ReinitializeGrid();
        Player observer = ( UserInterface.managedBattle.singleplayer && !board.owner.battle.turnLog[0].attackedPlayer.AI ) ? board.owner.battle.turnLog[0].attackedPlayer : board.owner.battle.turnLog[0].activePlayer;

        foreach (Ship ship in board.owner.ships)
        {
            bool revealed = ship.revealedBy.Contains( observer );
            ship.gameObject.SetActive( !ship.destroyed && revealed );

            if (revealed)
            {
                List<int> types = new List<int>();
                List<Vector2> sizes = new List<Vector2>();
                List<Vector3> positions = new List<Vector3>();

                BoardTile rootTile = ship.tiles[0];
                for (int i = 0; i < ship.tiles.Length; i++)
                {
                    BoardTile inspectedTile = ship.tiles[i];
                    int rootType = 1;
                    int inspectedType = 1;

                    if (UserInterface.managedBattle.turnLog[0].hitTiles.Contains( rootTile ))
                    {
                        rootType = 3;
                    }
                    else if (rootTile.hitBy.Contains( observer ) || ( observer == board.owner && rootTile.hitBy.Count > 0 ))
                    {
                        rootType = 2;
                    }
                    else
                    {
                        rootType = 1;
                    }

                    if (UserInterface.managedBattle.turnLog[0].hitTiles.Contains( inspectedTile ))
                    {
                        inspectedType = 3;
                    }
                    else if (inspectedTile.hitBy.Contains( observer ) || ( observer == board.owner && inspectedTile.hitBy.Count > 0 ))
                    {
                        inspectedType = 2;
                    }
                    else
                    {
                        inspectedType = 1;
                    }

                    if (rootType != inspectedType)
                    {
                        BoardTile secondTile = ship.tiles[i - 1];
                        Vector3 relative = secondTile.transform.localPosition - rootTile.transform.localPosition;

                        if (UserInterface.managedBattle.turnLog[0].hitTiles.Contains( rootTile ))
                        {
                            types.Add( 3 );
                        }
                        else if (rootTile.hitBy.Contains( observer ) || ( observer == board.owner && rootTile.hitBy.Count > 0 ))
                        {
                            types.Add( 2 );
                        }
                        else
                        {
                            types.Add( 1 );
                        }

                        sizes.Add( new Vector2( Mathf.Abs( relative.x ), Mathf.Abs( relative.z ) ) + Vector2.one * 0.9f );
                        positions.Add( rootTile.transform.position + relative / 2 );

                        rootTile = inspectedTile;
                    }

                    if (i == ship.tiles.Length - 1)
                    {
                        Vector3 relative = inspectedTile.transform.localPosition - rootTile.transform.localPosition;

                        if (UserInterface.managedBattle.turnLog[0].hitTiles.Contains( rootTile ))
                        {
                            types.Add( 3 );
                        }
                        else if (rootTile.hitBy.Contains( observer ) || ( observer == board.owner && rootTile.hitBy.Count > 0 ))
                        {
                            types.Add( 2 );
                        }
                        else
                        {
                            types.Add( 1 );
                        }
                        sizes.Add( new Vector2( Mathf.Abs( relative.x ), Mathf.Abs( relative.z ) ) + Vector2.one * 0.9f );
                        positions.Add( rootTile.transform.position + relative / 2 );
                    }
                }

                for (int x = 0; x < types.Count; x++)
                {

                    Vector3 position = positions[x];
                    Vector2 size = sizes[x];

                    DynamicStripedRectangle_GraphicsElement rectangle = new GameObject( "Highlight for " + ship.name ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
                    rectangle.transform.SetParent( visualParent.transform );
                    rectangle.transform.position = position + Vector3.up * 0.11f;
                    switch (types[x])
                    {
                        case 1:
                            rectangle.material = Master.vars.intactShipHighlightMaterial;
                            break;
                        case 2:
                            rectangle.material = Master.vars.hitTileIndicatorMaterial;
                            break;
                        case 3:
                            rectangle.material = Master.vars.justHitTileIndicatorMaterial;
                            break;
                    }

                    rectangle.Set( size, 0.1f, true, ( types[x] > 1 ) ? ( types[x] == 2 ? 0.1f : 0.5f ) : 0, 0.1f, 0.1f );
                }
            }
        }

        foreach (BoardTile tile in board.tiles)
        {
            bool activate = true;
            if (tile.containedShip)
            {
                if (tile.containedShip.revealedBy.Contains( observer ))
                {
                    activate = false;
                }
            }

            if (activate && ( tile.hitBy.Contains( UserInterface.managedBattle.turnLog[0].activePlayer ) ))
            {
                if (UserInterface.managedBattle.turnLog[0].hitTiles.Contains( tile ))
                {
                    DynamicStripedRectangle_GraphicsElement indicator = new GameObject( "Just Hit Tile Indicator" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
                    indicator.transform.SetParent( visualParent.transform );
                    indicator.transform.position = tile.transform.position + Vector3.up * 0.1f;

                    indicator.material = ( tile.containedShip == null ? Master.vars.missedTileIndicatorMaterial : Master.vars.justHitTileIndicatorMaterial );

                    indicator.Set( Vector2.one * 0.9f, 0.1f, true, 0.5f, 0.1f, 0.1f );


                }
                else
                {
                    GameObject indicator = GameObject.CreatePrimitive( PrimitiveType.Quad );
                    indicator.transform.SetParent( visualParent.transform );
                    indicator.transform.rotation = Quaternion.Euler( Vector3.right * 90 );
                    indicator.transform.position = tile.transform.position + Vector3.up * 0.1f;
                    indicator.transform.localScale = Vector3.one * 0.9f;

                    indicator.GetComponent<Renderer>().material = ( tile.containedShip == null ? Master.vars.missedTileIndicatorMaterial : Master.vars.hitTileIndicatorMaterial );
                }
            }
        }

        float boardSide = Mathf.Sqrt( board.tiles.Length );
        float elevation = 0.5f + ( boardSide / 2f ) / Mathf.Atan( Camera.main.fieldOfView / 2f * Mathf.Deg2Rad );
        Cameraman.AddWaypoint( new Cameraman.TargetCameraVector3Value( board.owner.transform.position + Vector3.up * elevation, 0.3f, Mathf.Infinity ), new Cameraman.TargetCameraVector3Value( Vector3.down, 0.4f, Mathf.Infinity ), 90f, true );
    }

    public override void Disable ()
    {
        base.Disable();
    }
}
