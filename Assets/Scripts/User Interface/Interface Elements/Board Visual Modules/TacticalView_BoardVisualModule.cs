using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalView_BoardVisualModule : BoardVisualModule
{
    public Bounds textBounds;

    public override void Initialize ()
    {
        base.Initialize();
    }

    public override void Enable ()
    {
        base.Enable();

        visualParent.transform.localPosition = Vector3.up;

        TextMesh text = new GameObject( "Text" ).AddComponent<TextMesh>();
        text.fontSize = 210;
        text.transform.parent = visualParent.transform;
        text.transform.localPosition = Vector3.zero;
        text.transform.localRotation = Quaternion.Euler( Vector3.right * 90 );
        text.characterSize *= Master.vars.mainBattleBoardDistance / 15f / ( text.fontSize / 30f );
        text.text = board.owner.label;

        text.anchor = TextAnchor.MiddleCenter;

        textBounds = text.GetComponent<Renderer>().bounds;
        DynamicStripedRectangle_GraphicsElement background = new GameObject( "Background" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
        background.transform.parent = visualParent.transform;
        background.transform.localPosition = Vector3.zero;

        float boardDimensions = Mathf.Sqrt( board.tiles.Length );
        visualParent.transform.localPosition = Vector3.forward * ( textBounds.extents.z * 1.2f + boardDimensions );
        if (board.owner.battle.activePlayer == board.owner)
        {
            background.material = Master.vars.activePlayerTagBackgroundMaterial;
        }
        else
        {
            background.material = Master.vars.playerTagBackgroundMaterial;
        }
        background.Set( new Vector2( textBounds.extents.x * 2 * 1.2f, textBounds.extents.z * 2 * 1.2f ), textBounds.extents.z / 8f, true, textBounds.extents.x / 10f, textBounds.extents.z / 8f, textBounds.extents.z / 8f );
    }

    public override void Disable ()
    {
        base.Disable();
    }

    public override void Refresh ()
    {
        base.Refresh();
    }
}
