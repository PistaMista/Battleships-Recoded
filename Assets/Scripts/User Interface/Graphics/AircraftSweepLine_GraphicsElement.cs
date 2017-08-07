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
        public Renderer render;
        public MeshFilter filter;
    }

    void Start ()
    {
        pulses = new List<Pulse>();
    }

    Vector3[] directions;
    public void Set ( float length, float width, float pulseWidth, float pulseSpeed, float pulseSpacing, float transparency, FallOffFunction fallOffFunction )
    {
        Destroy( parent );
        parent = new GameObject( "Graphics Parent" );
        parent.transform.SetParent( transform );
        parent.transform.localPosition = Vector3.zero;
        pulses = new List<Pulse>();

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

        this.pulseWidth = pulseWidth;
        this.pulseSpeed = pulseSpeed;
        this.pulseSpacing = pulseSpacing;
        this.fallOffFunction = fallOffFunction;

        directions = new Vector3[25];
        int index = 0;
        float step = Mathf.PI / ( directions.Length - 1 );
        for (float i = -Mathf.PI / 2; i <= Mathf.PI / 2; i += step)
        {
            directions[index] = new Vector3( Mathf.Cos( i ), 0, Mathf.Sin( i ) );
            index++;
        }
    }

    void Update ()
    {
        for (int i = 0; i < pulses.Count; i++)
        {
            Pulse pulse = pulses[i];
            pulse.outerRadius += pulseSpeed * Time.deltaTime;
            pulse.innerRadius = Mathf.Clamp( pulse.outerRadius - pulseWidth, 0, Mathf.Infinity );

            float transp = fallOffFunction( pulse.outerRadius / ( length / 2.0f ) );
            if (transp <= 0)
            {
                Destroy( pulse.render );
                pulses.RemoveAt( i );
                i--;
                continue;
            }
            else
            {
                //MESH CONSTRUCTION
                Mesh mesh = new Mesh();

                Vector3[] vertices = new Vector3[directions.Length * 2];
                int[] triangles = new int[( directions.Length * 2 - 2 ) * 3];

                for (int a = 1; a < directions.Length; a++)
                {
                    if (a == 1)
                    {
                        vertices[0] = directions[0] * pulse.innerRadius;
                        vertices[1] = directions[0] * pulse.outerRadius;
                    }

                    vertices[a * 2] = directions[a] * pulse.innerRadius;
                    vertices[a * 2 + 1] = directions[a] * pulse.outerRadius;

                    triangles[a * 6 - 6] = a * 2 - 2;
                    triangles[a * 6 - 4] = a * 2;
                    triangles[a * 6 - 5] = a * 2 - 1;


                    triangles[a * 6 - 3] = a * 2;
                    triangles[a * 6 - 1] = a * 2 + 1;
                    triangles[a * 6 - 2] = a * 2 - 1;
                }

                mesh.vertices = vertices;
                mesh.triangles = triangles;

                //FINALIZATION
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                Color color = Color.black;
                color.a *= transp * transparency;
                block.SetColor( "_Color", color );

                pulse.render.SetPropertyBlock( block );

                pulse.filter.mesh = mesh;
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
        pulse.render = new GameObject( "Render" ).AddComponent<MeshRenderer>();

        pulse.render.material = material;
        pulse.render.transform.SetParent( parent.transform );
        pulse.render.transform.localPosition = Vector3.right * width / 2.0f;
        pulse.render.transform.localRotation = Quaternion.Euler( Vector3.right * 180 );

        pulse.filter = pulse.render.gameObject.AddComponent<MeshFilter>();

        pulses.Insert( 0, pulse );
    }
}
