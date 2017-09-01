using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle_GraphicsElement : GraphicsElement
{
    public float sideWidth;
    public Vector2 size;
    float backgroundTransparency;
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    Renderer[] renderers;
    Renderer bgRenderer;

    public void Set ( Vector2 size, float sideWidth, bool flat, float backgroundTransparency, float transparency )
    {
        BaseReset();

        renderers = new Renderer[4];
        defaultTransparency = transparency;
        this.backgroundTransparency = backgroundTransparency;

        for (int direction = -1; direction < 2; direction += 2)
        {
            Vector2 position = Vector2.right * ( size.x / 2f - sideWidth / 2f ) * direction;
            Vector3 scale = new Vector3( sideWidth, size.y - 2 * sideWidth, 1f );

            GameObject tmp = GameObject.CreatePrimitive( PrimitiveType.Quad );
            tmp.transform.parent = visualParent.transform;
            tmp.transform.localPosition = position;
            tmp.transform.localScale = scale;

            Renderer render = tmp.GetComponent<Renderer>();
            render.material = MainMaterial;
            SetTransparencyForRenderer( render, defaultTransparency * transparencyMod );

            renderers[(int)( direction / 2.0f + 0.5f )] = render;
        }

        for (int direction = -1; direction < 2; direction += 2)
        {
            Vector2 position = Vector2.up * ( size.y / 2f - sideWidth / 2f ) * direction;
            Vector3 scale = new Vector3( size.x, sideWidth, 1f );

            GameObject tmp = GameObject.CreatePrimitive( PrimitiveType.Quad );
            tmp.transform.parent = visualParent.transform;
            tmp.transform.localPosition = position;
            tmp.transform.localScale = scale;

            Renderer render = tmp.GetComponent<Renderer>();
            render.material = MainMaterial;
            SetTransparencyForRenderer( render, defaultTransparency * transparencyMod );

            renderers[(int)( direction / 2.0f + 2.5f )] = render;
        }

        if (backgroundTransparency > 0)
        {
            GameObject bg = GameObject.CreatePrimitive( PrimitiveType.Quad );
            bg.transform.SetParent( visualParent.transform );
            bg.transform.localPosition = Vector3.forward * 0.001f;
            bg.transform.localScale = size - Vector2.one * sideWidth * 2;

            Renderer render = bg.GetComponent<Renderer>();
            bgRenderer = render;
            render.material = MainMaterial;

            SetTransparencyForRenderer( render, backgroundTransparency );

        }

        this.sideWidth = sideWidth;
        this.size = size;
        visualParent.transform.rotation = Quaternion.Euler( Vector3.right * ( flat ? 90 : 0 ) );
    }

    protected override void SetTransparencyMod ( float transparencyMod )
    {
        base.SetTransparencyMod( transparencyMod );
        for (int i = 0; i < renderers.Length; i++)
        {
            SetTransparencyForRenderer( renderers[i], defaultTransparency * transparencyMod );
        }

        SetTransparencyForRenderer( bgRenderer, backgroundTransparency * transparencyMod );
    }
}
