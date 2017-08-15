using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMarkCross_GraphicsElement : GraphicsElement
{
    float deployTime;


    public GameObject quad;
    public Renderer quadRenderer;
    public void Set ( float strokeWidth, float strokeLength, float deployTime, float transparency )
    {
        //TEST
        BaseReset();
        defaultTransparency = transparency;
        this.deployTime = deployTime;

        quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
        quad.transform.SetParent( visualParent.transform );
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localRotation = Quaternion.Euler( Vector3.right * 90 );

        quadRenderer = quad.GetComponent<Renderer>();
        quadRenderer.material = mainMaterial;

        SetTransparencyForRenderer( quadRenderer, defaultTransparency * transparencyMod );
    }

    protected override void SetTransparencyMod ( float transparencyMod )
    {
        base.SetTransparencyMod( transparencyMod );
        SetTransparencyForRenderer( quadRenderer, defaultTransparency * transparencyMod );
    }
}
