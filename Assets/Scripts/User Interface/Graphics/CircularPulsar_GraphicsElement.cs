using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularPulsar_GraphicsElement : MonoBehaviour
{

    void Start ()
    {
        //pulses = new List<Pulse>();
    }
    public Material material;

    float pulseWidth;
    float pulseSpeed;
    float pulseSpacing;
    float transparency;
    int pulsesLeft;
    float maxOuterRadius;

    public delegate float FallOffFunction ( float x );
    FallOffFunction transparencyFunction;
    FallOffFunction speedFunction;

    float directionSpacing = 4.0f;

    GameObject parent;
    List<Pulse> pulses;
    struct Pulse
    {
        public float innerRadius;
        public float outerRadius;
        public Renderer render;
        public MeshFilter filter;
    }

    public void Set ( float pulseWidth, float pulseSpeed, float pulseSpacing, float transparency, FallOffFunction transparencyFunction, FallOffFunction speedFunction, float fullness, int maxPulses, float maxOuterRadius, bool startExtended )
    {
        this.pulseWidth = pulseWidth;
        this.pulseSpeed = pulseSpeed;
        this.pulseSpacing = pulseSpacing;
        this.transparency = transparency;
        this.transparencyFunction = transparencyFunction;
        this.speedFunction = speedFunction;
        pulsesLeft = maxPulses;
        this.maxOuterRadius = maxOuterRadius;

        Destroy( parent );
        parent = new GameObject( "Graphical Parent" );
        parent.transform.SetParent( transform );
        pulses = new List<Pulse>();

        float arc = Mathf.PI * 2 * fullness;
        directions = new Vector3[Mathf.CeilToInt( arc / ( Mathf.Deg2Rad * directionSpacing ) )];
        directionSpacing = arc / ( directions.Length - 1 );

        for (int i = 0; i < directions.Length; i++)
        {
            float angle = directionSpacing * i;
            directions[i] = new Vector3( Mathf.Cos( angle ), 0, Mathf.Sin( angle ) );
        }


        if (startExtended)
        {
            for (float i = 0; i < maxOuterRadius; i += pulseSpacing + pulseWidth)
            {
                AddPulse( i );
            }
        }
    }

    Vector3[] directions;
    void Update ()
    {
        for (int i = 0; i < pulses.Count; i++)
        {
            Pulse pulse = pulses[i];
            pulse.outerRadius += speedFunction( pulse.outerRadius / maxOuterRadius ) * pulseSpeed * Time.deltaTime;
            pulse.innerRadius = Mathf.Clamp( pulse.outerRadius - pulseWidth, 0, Mathf.Infinity );

            float transp = transparencyFunction( pulse.outerRadius / maxOuterRadius );

            if (transp <= 0)
            {
                Destroy( pulse.render.gameObject );
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
            if (pulsesLeft > 0)
            {
                AddPulse( 0 );
            }
            else
            {
                Destroy( gameObject );
            }
        }
    }

    void AddPulse ( float headStart )
    {
        if (pulsesLeft > 0)
        {
            Pulse pulse;
            pulse.outerRadius = headStart;
            pulse.innerRadius = 0;
            pulse.render = new GameObject( "Render" ).AddComponent<MeshRenderer>();

            pulse.render.material = material;
            pulse.render.transform.SetParent( parent.transform );
            pulse.render.transform.localPosition = Vector3.zero;
            pulse.render.transform.localRotation = Quaternion.Euler( Vector3.right * 180 );

            pulse.filter = pulse.render.gameObject.AddComponent<MeshFilter>();

            pulses.Insert( 0, pulse );

            pulsesLeft--;
        }
    }
}
