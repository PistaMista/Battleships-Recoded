﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendingMultiLine_GraphicsElement : GraphicsElement
{
    public float lineWidth;
    public float lineLength;
    public float lineSpacing;
    public float lineDeployTime;
    public List<Line> lines;

    [System.Serializable]
    public struct Line
    {
        public Renderer renderer;
        public GameObject quad;
        public float lengthChangeRate;
        public Vector3 targetPosition;
        public Vector3 velocity;
        public bool removing;
    }


    public void Reset ( float lineWidth, float lineLength, float lineSpacing, float lineDeployTime, float transparency, bool flat )
    {
        BaseReset();
        if (flat)
        {
            visualParent.transform.rotation = Quaternion.Euler( 90, 0, 0 );
        }
        lines = new List<Line>();

        this.lineWidth = lineWidth;
        this.lineLength = lineLength;
        this.lineSpacing = lineSpacing;
        this.lineDeployTime = lineDeployTime;
        defaultTransparency = transparency;
    }

    public void AddLine ( int count )
    {
        for (int i = 0; i < count; i++)
        {
            Line line = new Line();
            line.quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
            line.quad.transform.SetParent( visualParent.transform );
            line.quad.transform.localScale = new Vector3( lineWidth, 0, 1 );
            line.quad.transform.localRotation = Quaternion.Euler( Vector3.zero );
            line.renderer = line.quad.GetComponent<Renderer>();
            line.renderer.material = MainMaterial;
            line.removing = false;

            lines.Add( line );
            RecalculateTargetPositions();

            line.quad.transform.localPosition = lines[lines.Count - 1].targetPosition;
        }
    }

    public void RemoveLine ( int count )
    {
        for (int i = 0; i < count; i++)
        {
            for (int x = lines.Count - 1; x >= 0; x--)
            {
                Line line = lines[x];
                if (!line.removing)
                {
                    line.removing = true;
                    lines[x] = line;
                    break;
                }
            }
        }
    }

    public void SetLineCount ( int count )
    {
        int actualCount = lines.FindAll( ( obj ) => !obj.removing ).Count;

        if (count > actualCount)
        {
            AddLine( count - actualCount );
        }
        else if (count < actualCount)
        {
            RemoveLine( actualCount - count );
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
            Vector3 currentPosition = new Vector3( line.quad.transform.localPosition.x, 0, 0 );

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
