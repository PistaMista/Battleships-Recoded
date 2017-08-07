using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AircraftSweepLine_GraphicsElement : MonoBehaviour
{
    public Material material;
    float length;
    float transparency;
    float pulseWidth;
    float pulseSpeed;
    float pulseSpacing;
    float pulseTransparencyFalloff;
    public delegate float FallOffFunction ( float x );
    FallOffFunction fallOffFunction;

    GameObject parent;
    List<Pulse> pulses;
    struct Pulse
    {
        public float innerRadius;
        public float outerRadius;
        public GameObject render;
    }

    //TODO TEST
    void Start ()
    {
        Set( 10, 1, 1, 1, 1, 1, ( x ) => { return 1 - x; } );
        pulses = new List<Pulse>();
    }
    //TEST

    public void Set ( float length, float width, float pulseWidth, float pulseSpeed, float pulseSpacing, float transparency, FallOffFunction fallOffFunction )
    {
        Destroy( parent );
        parent = new GameObject( "Graphics Parent" );
        parent.transform.SetParent( transform );
        pulses = new List<Pulse>();

        GameObject quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
        quad.transform.rotation = Quaternion.Euler( Vector3.right * 90 );
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

        this.pulseWidth = pulseWidth;
        this.pulseSpeed = pulseSpeed;
        this.pulseSpacing = pulseSpacing;
        this.fallOffFunction = fallOffFunction;
    }

    void Update ()
    {
        for (int i = 0; i < pulses.Count; i++)
        {
            Pulse pulse = pulses[i];
            pulse.outerRadius += pulseSpeed * Time.deltaTime;
            pulse.innerRadius = Mathf.Clamp( pulse.outerRadius - pulseWidth, 0, Mathf.Infinity );

            float transp = fallOffFunction( pulse.outerRadius / length );
            if (transp <= 0)
            {
                Destroy( pulse.render );
                pulses.RemoveAt( i );
                i--;
                continue;
            }
            else
            {

            }


            pulses[i] = pulse;
        }

        if (pulses.Count > 0)
        {
            float difference = pulses[0].innerRadius - pulseSpacing;
            if (difference >= 0)
            {
                AddPulse( difference );
            }
        }
        else
        {
            AddPulse( 0 );
        }
    }

    void AddPulse ( float headStart )
    {
        Pulse pulse;
        pulse.outerRadius = headStart;
        pulse.innerRadius = 0;
        pulse.render = new GameObject( "Render" );
        pulse.render.transform.SetParent( parent.transform );

        pulses.Insert( 0, pulse );
    }
}
