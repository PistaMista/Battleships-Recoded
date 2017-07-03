﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardVisualModule : ScriptableObject
{
    protected GameObject visualParent;
    public bool enabled;
    public Board board;
    public virtual void Initialize()
    {

    }

    public virtual void Enable()
    {
        enabled = true;
        visualParent = new GameObject("Visual Parent");
        visualParent.transform.parent = board.visualParent.transform;
    }

    public virtual void Disable()
    {
        enabled = false;
        Destroy(visualParent);
    }

    public virtual void Refresh()
    {

    }
}