using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalView_BoardVisualModule : BoardVisualModule
{
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
        text.gameObject.transform.parent = visualParent.transform;
        text.gameObject.transform.localPosition = Vector3.zero;
        text.gameObject.transform.localRotation = Quaternion.Euler( Vector3.right * 90 );
        text.characterSize *= Master.vars.mainBattleBoardDistance / 15f / ( text.fontSize / 30f );
        text.text = board.owner.label;
        text.anchor = TextAnchor.MiddleCenter;

        Bounds textBounds = text.GetComponent<Renderer>().bounds;
        DynamicStripedRectangle_GraphicsElement background = new GameObject( "Background" ).AddComponent<DynamicStripedRectangle_GraphicsElement>();
        background.gameObject.transform.parent = visualParent.transform;
        background.gameObject.transform.localPosition = Vector3.zero;
        background.material = Master.vars.playerTagBackgroundMaterial;
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
