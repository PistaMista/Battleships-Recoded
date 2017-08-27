using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryMarkCross_DeployableGraphicsElement : DeployableGraphicsElement
{
    public float strokeWidth;
    public float strokeLength;

    public float secondStrokeMiddleExclusion;

    public Renderer[] renderers;
    public void Set ( float strokeWidth, float strokeLength, float deployTime, float transparency )
    {
        BaseReset();
        defaultTransparency = transparency;
        this.deployTime = deployTime;
        this.strokeWidth = strokeWidth;
        this.strokeLength = strokeLength;

        renderers = new Renderer[3];

        for (int i = 0; i < 3; i++)
        {
            Renderer quad = GameObject.CreatePrimitive( PrimitiveType.Quad ).GetComponent<Renderer>();
            quad.transform.SetParent( visualParent.transform );
            quad.material = MainMaterial;
            SetTransparencyForRenderer( quad, defaultTransparency * transparencyMod );
            renderers[i] = quad;
            ManageLine( i, 0 );
        }

        deploying = true;
        deploySpeedLimit = Mathf.Infinity;

        secondStrokeMiddleExclusion = strokeWidth / strokeLength;
    }

    protected override void SetTransparencyMod ( float transparencyMod )
    {
        base.SetTransparencyMod( transparencyMod );
        if (renderers != null)
        {
            foreach (Renderer render in renderers)
            {
                SetTransparencyForRenderer( render, defaultTransparency * transparencyMod );
            }
        }
    }

    protected override void Update ()
    {
        base.Update();
        float primaryProgress = Mathf.Clamp01( deployProgress / 0.5f );
        float secondaryProgress = Mathf.Clamp01( deployProgress / 0.5f - 1.0f );

        if (deployProgress < 0.99f && renderers != null)
        {
            ManageLine( 0, primaryProgress * strokeLength );
            ManageLine( 1, Mathf.Clamp( secondaryProgress, 0f, 0.5f - secondStrokeMiddleExclusion / 2.0f ) * strokeLength );
            ManageLine( 2, Mathf.Clamp01( secondaryProgress - 0.5f - secondStrokeMiddleExclusion / 2.0f ) * strokeLength );
        }
    }

    void ManageLine ( int quadID, float length )
    {
        Renderer quad = renderers[quadID];
        Vector3 origin = Vector3.zero;
        Vector3 destination = Vector3.zero;

        switch (quadID)
        {
            case 0:
                origin = Vector3.left * strokeLength / 2.0f;
                destination = origin + Vector3.right * length;
                break;
            case 1:
                origin = Vector3.up * strokeLength / 2.0f;
                destination = origin + Vector3.down * length;
                break;
            case 2:
                origin = Vector3.down * strokeWidth / 2.0f;
                destination = origin + Vector3.down * length;
                break;
        }

        quad.transform.localPosition = origin / 2.0f + destination / 2.0f;
        quad.transform.localRotation = Quaternion.Euler( Vector3.zero );

        if (quadID == 0)
        {
            quad.transform.localScale = new Vector3( length, strokeWidth, 2.0f );
        }
        else
        {
            quad.transform.localScale = new Vector3( strokeWidth, length, 2.0f );
        }
    }
}
