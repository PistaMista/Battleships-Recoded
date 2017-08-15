using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AircraftSweepLine_GraphicsElement : GraphicsElement
{
    float length;
    float width;

    GameObject quad;
    public void Set ( float length, float width, float pulseWidth, float pulseSpeed, float pulseSpacing, float transparency, CircularPulsar_GraphicsElement.FallOffFunction fallOffFunction )
    {
        BaseReset();

        quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
        quad.transform.SetParent( visualParent.transform );
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localRotation = Quaternion.Euler( Vector3.right * 90 );
        quad.transform.localScale = new Vector3( width, length, 1 );

        Renderer render = quad.GetComponent<Renderer>();
        render.material = mainMaterial;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Color color = Color.black;
        color.a *= transparency * transparencyMod;
        block.SetColor( "_Color", color );
        render.SetPropertyBlock( block );


        this.defaultTransparency = transparency;
        this.length = length;
        this.width = width;

        CircularPulsar_GraphicsElement pulsar = new GameObject( "Pulsar" ).AddComponent<CircularPulsar_GraphicsElement>();
        pulsar.mainMaterial = mainMaterial;
        pulsar.Set( pulseWidth, pulseSpeed, pulseSpacing, transparency * transparencyMod, fallOffFunction, ( x ) => { return 1; }, 0.5f, 9999999, length / 2.0f, true );

        pulsar.gameObject.transform.SetParent( visualParent.transform );
        pulsar.transform.localPosition = Vector3.right * width / 2.0f;
        pulsar.transform.localRotation = Quaternion.Euler( Vector3.up * -90 );
    }

    protected override void SetTransparencyMod ( float transparencyMod )
    {
        base.SetTransparencyMod( transparencyMod );
        Renderer render = quad.GetComponent<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Color color = Color.black;
        color.a *= defaultTransparency * transparencyMod;
        block.SetColor( "_Color", color );
        render.SetPropertyBlock( block );
    }
}
