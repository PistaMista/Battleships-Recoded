using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackView_BoardVisualModule : BoardVisualModule
{
    public override void Initialize ()
    {
        base.Initialize();
    }

    public override void Enable ()
    {
        base.Enable();
        //board.visualModules[1].Enable();
        board.ReinitializeGrid();
        Player observer = UserInterface.managedBattle.activePlayer;
        if (observer == null)
        {
            observer = ( UserInterface.managedBattle.singleplayer && !board.owner.battle.turnLog[0].attackedPlayer.AI ) ? board.owner.battle.turnLog[0].attackedPlayer : board.owner.battle.turnLog[0].activePlayer;
        }

        foreach (Ship ship in board.owner.ships)
        {
            bool revealed = ship.revealedBy.Contains( observer );
            ship.gameObject.SetActive( !ship.destroyed && revealed );

            if (revealed)
            {
                if (ship.destroyed)
                {
                    Vector3 relative = ship.tiles[0].transform.localPosition - ship.tiles[ship.tiles.Length - 1].transform.localPosition;
                    DynamicStripedRectangle_GraphicsElement rectangle = new GameObject( "Highlight for " + ship.name ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
                    rectangle.transform.SetParent( visualParent.transform );
                    rectangle.transform.position = ship.transform.position + Vector3.up * 0.11f;

                    rectangle.material = Master.vars.hitTileIndicatorMaterial;

                    rectangle.Set( new Vector2( Mathf.Abs( relative.x ), Mathf.Abs( relative.z ) ) + Vector2.one * 0.9f, 0.1f, true, 0, 0, 0.1f, 0.1f, 1.0f );
                }
                else
                {
                    List<bool> types = new List<bool>();
                    List<Vector2> sizes = new List<Vector2>();
                    List<Vector3> positions = new List<Vector3>();

                    BoardTile rootTile = ship.tiles[0];
                    for (int i = 0; i < ship.tiles.Length; i++)
                    {
                        BoardTile inspectedTile = ship.tiles[i];

                        if (( inspectedTile.hitBy.Contains( observer ) || ( observer == board.owner && inspectedTile.hitBy.Count > 0 ) ) != ( rootTile.hitBy.Contains( observer ) || ( observer == board.owner && rootTile.hitBy.Count > 0 ) ))
                        {
                            BoardTile secondTile = ship.tiles[i - 1];
                            Vector3 relative = secondTile.transform.localPosition - rootTile.transform.localPosition;

                            types.Add( rootTile.hitBy.Contains( observer ) || ( observer == board.owner && rootTile.hitBy.Count > 0 ) );
                            sizes.Add( new Vector2( Mathf.Abs( relative.x ), Mathf.Abs( relative.z ) ) + Vector2.one * 0.9f );
                            positions.Add( rootTile.transform.position + relative / 2 );

                            rootTile = inspectedTile;
                        }

                        if (i == ship.tiles.Length - 1)
                        {
                            Vector3 relative = inspectedTile.transform.localPosition - rootTile.transform.localPosition;

                            types.Add( rootTile.hitBy.Contains( observer ) || ( observer == board.owner && rootTile.hitBy.Count > 0 ) );
                            sizes.Add( new Vector2( Mathf.Abs( relative.x ), Mathf.Abs( relative.z ) ) + Vector2.one * 0.9f );
                            positions.Add( rootTile.transform.position + relative / 2 );
                        }
                    }

                    for (int x = 0; x < types.Count; x++)
                    {
                        Material material = types[x] ? Master.vars.hitTileIndicatorMaterial : Master.vars.intactShipHighlightMaterial;
                        Vector3 position = positions[x];
                        Vector2 size = sizes[x];

                        DynamicStripedRectangle_GraphicsElement rectangle = new GameObject( "Highlight for " + ship.name ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
                        rectangle.transform.SetParent( visualParent.transform );
                        rectangle.transform.position = position + Vector3.up * 0.11f;
                        rectangle.material = material;

                        rectangle.Set( size, 0.1f, true, 0, types[x] ? 0 : 0.1f, 0.1f, 0.1f, 1.0f );
                    }
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

            if (activate && ( tile.hitBy.Contains( observer ) || ( observer == board.owner && tile.hitBy.Count > 0 ) ))
            {
                GameObject indicator = GameObject.CreatePrimitive( PrimitiveType.Quad );
                indicator.transform.SetParent( visualParent.transform );
                indicator.transform.rotation = Quaternion.Euler( Vector3.right * 90 );
                indicator.transform.position = tile.transform.position + Vector3.up * 0.1f;
                indicator.transform.localScale = Vector3.one * 0.9f;

                indicator.GetComponent<Renderer>().material = tile.containedShip == null ? Master.vars.missedTileIndicatorMaterial : Master.vars.hitTileIndicatorMaterial;
            }
        }

        float boardSide = Mathf.Sqrt( board.tiles.Length );
        float elevation = 0.5f + ( boardSide / 2f ) / Mathf.Atan( Camera.main.fieldOfView / 2f * Mathf.Deg2Rad );
        Cameraman.AddWaypoint( new Cameraman.TargetCameraVector3Value( board.owner.transform.position + Vector3.up * elevation, 0.3f, Mathf.Infinity ), new Cameraman.TargetCameraVector3Value( Vector3.down, 0.4f, Mathf.Infinity ), 90f, false );
    }

    public override void Disable ()
    {
        base.Disable();
        board.DisableGrid();
    }

    public override void Refresh ()
    {
        base.Refresh();
    }
}
