using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AircraftSweepLine_GraphicsElement : MonoBehaviour
{
    public Material material;

    float length;
    float width;
    float transparency;

    GameObject parent;
    public void Set ( float length, float width, float pulseWidth, float pulseSpeed, float pulseSpacing, float transparency, CircularPulsar_GraphicsElement.FallOffFunction fallOffFunction )
    {
        Destroy( parent );
        parent = new GameObject( "Graphics Parent" );
        parent.transform.SetParent( transform );
        parent.transform.localPosition = Vector3.zero;

        GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
        quad.transform.SetParent( parent.transform );
        quad.transform.localPosition = Vector3.zero;
        quad.transform.localRotation = Quaternion.Euler( Vector3.right * 90 );
        quad.transform.localScale = new Vector3( width, length, 1 );

        Renderer render = quad.GetComponent<Renderer>();
        render.material = material;

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Color color = Color.black;
        color.a *= transparency;
        block.SetColor( "_Color", color );
        render.SetPropertyBlock( block );


        this.transparency = transparency;
        this.length = length;
        this.width = width;

        CircularPulsar_GraphicsElement pulsar = new GameObject( "Pulsar" ).AddComponent<CircularPulsar_GraphicsElement>();
        pulsar.material = material;
        pulsar.Set( pulseWidth, pulseSpeed, pulseSpacing, transparency, fallOffFunction, ( x ) => { return 1; }, 0.5f, 9999999, length / 2.0f, true );

        pulsar.gameObject.transform.SetParent( parent.transform );
        pulsar.transform.localPosition = Vector3.right * width / 2.0f;
        pulsar.transform.localRotation = Quaternion.Euler( Vector3.up * -90 );
    }
}
