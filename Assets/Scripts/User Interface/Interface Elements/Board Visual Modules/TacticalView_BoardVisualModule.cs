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
        TextMesh text = new GameObject( "Text" ).AddComponent<TextMesh>();

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
