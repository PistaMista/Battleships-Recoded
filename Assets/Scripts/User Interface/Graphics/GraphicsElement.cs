using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsElement : MonoBehaviour
{
    public float targetTransparencyMod = 1.0f;

    public float transparencyModTransitionTime = 0.31f;

    public Material mainMaterial;

    public float transparencyModTransitionSpeed;

    public float transparencyMod = 1.0f;

    public float defaultTransparency;

    public GameObject visualParent;

    public bool destroyAfterTransparencyTransition;

    void Update ()
    {
        if (Mathf.Abs( transparencyMod - targetTransparencyMod ) > 0.012f)
        {
            SetTransparencyMod( Mathf.SmoothDamp( transparencyMod, targetTransparencyMod, ref transparencyModTransitionSpeed, transparencyModTransitionTime, Mathf.Infinity ) );

        }
        else if (destroyAfterTransparencyTransition)
        {
            Destroy( gameObject );
        }
    }

    protected virtual void SetTransparencyMod ( float transparencyMod )
    {
        this.transparencyMod = transparencyMod;
        GraphicsElement[] children = transform.GetComponentsInChildren<GraphicsElement>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != this)
            {
                children[i].SetTransparencyMod( transparencyMod );
            }
        }
    }

    protected void SetTransparencyForRenderer ( Renderer render, float transparency )
    {
        MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
        Color color = Color.black;
        color.a *= transparency;
        matBlock.SetColor( "_Color", color );

        render.SetPropertyBlock( matBlock );
    }

    protected void BaseReset ()
    {
        Destroy( visualParent );
        visualParent = new GameObject( "Graphical Parent" );
        visualParent.transform.SetParent( transform );
        visualParent.transform.localPosition = Vector3.zero;
    }
}
