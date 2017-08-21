using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendingMultiLine_GraphicsElement : GraphicsElement
{
    float lineWidth;
    float lineLength;
    float lineSpacing;
    float lineDeployTime;

    private void Start ()
    {
        Reset( 1, 10, 3, 1.0f, 1 );
        AddLine();
        AddLine();
        //AddLine();
    }

    struct Line
    {
        public Renderer renderer;
        public GameObject quad;
        public float lengthChangeRate;
        public Vector3 targetPosition;
        public Vector3 velocity;
        public bool removing;
    }

    List<Line> lines;

    public void Reset ( float lineWidth, float lineLength, float lineSpacing, float lineDeployTime, float transparency )
    {
        BaseReset();
        lines = new List<Line>();

        this.lineWidth = lineWidth;
        this.lineLength = lineLength;
        this.lineSpacing = lineSpacing;
        this.lineDeployTime = lineDeployTime;
        defaultTransparency = transparency;
    }

    public void AddLine ()
    {
        Line line = new Line();
        line.quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
        line.quad.transform.SetParent( visualParent.transform );
        line.quad.transform.localScale = new Vector3( lineWidth, 0, 1 );
        line.quad.transform.localRotation = Quaternion.Euler( Vector3.zero );
        line.renderer = line.quad.GetComponent<Renderer>();
        line.renderer.material = mainMaterial;
        line.removing = false;

        lines.Add( line );
        RecalculateTargetPositions();

        line.quad.transform.localPosition = lines[lines.Count - 1].targetPosition;
    }

    public void RemoveLine ()
    {
        if (lines.Count > 0)
        {
            Line line = lines[lines.Count - 1];
            line.removing = true;
            lines[lines.Count - 1] = line;
        }
    }

    void RecalculateTargetPositions ()
    {
        float positionStep = lineSpacing + lineWidth;
        float initialPosition = -( lines.Count - 1 ) * positionStep / 2.0f;
        for (int i = 0; i < lines.Count; i++)
        {
            Line line = lines[i];
            line.targetPosition = Vector3.right * ( initialPosition + positionStep * i );
            lines[i] = line;
        }
    }

    protected override void SetTransparencyMod ( float transparencyMod )
    {
        base.SetTransparencyMod( transparencyMod );
        foreach (Line line in lines)
        {
            SetTransparencyForRenderer( line.renderer, defaultTransparency * transparencyMod );
        }
    }

    protected override void Update ()
    {
        base.Update();
        for (int i = 0; i < lines.Count; i++)
        {
            Line line = lines[i];
            Vector3 currentPosition = new Vector3( line.quad.transform.position.x, 0, 0 );

            line.quad.transform.localScale = new Vector3( lineWidth, Mathf.SmoothDamp( line.quad.transform.localScale.y, line.removing ? 0 : lineLength, ref line.lengthChangeRate, lineDeployTime, Mathf.Infinity ), 1 );
            line.quad.transform.localPosition = Vector3.SmoothDamp( currentPosition, line.targetPosition, ref line.velocity, lineDeployTime, Mathf.Infinity ) + Vector3.up * ( line.quad.transform.localScale.y / 2.0f );

            if (line.removing && line.quad.transform.localScale.y < 0.01f)
            {
                lines.RemoveAt( i );
                RecalculateTargetPositions();
                i--;
            }
            else
            {
                lines[i] = line;
            }
        }
    }
}
