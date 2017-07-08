﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicStripedRectangle_GraphicsElement : MonoBehaviour
{
    public Material material;
    public float stripeMovementSpeed;
    public float stripeWidth;
    public float stripeSpacing;
    Rectangle_GraphicsElement baseRectangle;
    GameObject visualParent;
    Vector2 initialPosition;

    /// <summary>
    /// Stripe.
    /// </summary>
    struct Stripe
    {
        public Vector2 position;
        public Vector2 lineDirection;
    }

    List<Stripe> stripes;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start ()
    {

    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update ()
    {
        if (stripes != null)
        {
            MoveStripes();
        }
    }

    /// <summary>
    /// The stripe mesh.
    /// </summary>
    Mesh stripeMesh;

    /// <summary>
    /// Set the specified size, sideWidth, flat, stripeMovementSpeed, stripeWidth and stripeSpacing.
    /// </summary>
    /// <returns>The set.</returns>
    /// <param name="size">Size.</param>
    /// <param name="sideWidth">Side width.</param>
    /// <param name="flat">If set to <c>true</c> flat.</param>
    /// <param name="stripeMovementSpeed">Stripe movement speed.</param>
    /// <param name="stripeWidth">Stripe width.</param>
    /// <param name="stripeSpacing">Stripe spacing.</param>
    public void Set ( Vector2 size, float sideWidth, bool flat, float stripeMovementSpeed, float stripeWidth, float stripeSpacing )
    {
        Destroy( visualParent );
        visualParent = new GameObject( "Visual Parent" );
        visualParent.transform.parent = transform;
        visualParent.transform.localPosition = Vector3.zero;

        baseRectangle = new GameObject( "Base Rectangle" ).AddComponent<Rectangle_GraphicsElement>();
        baseRectangle.material = material;
        baseRectangle.transform.parent = visualParent.transform;
        baseRectangle.transform.localPosition = Vector3.zero;
        baseRectangle.Set( size, sideWidth, false );

        GameObject stripeObject = new GameObject( "Stripes" );
        stripeObject.transform.parent = visualParent.transform;
        stripeObject.transform.localPosition = Vector3.zero;

        stripeMesh = stripeObject.AddComponent<MeshFilter>().mesh;
        stripeMesh.MarkDynamic();

        Renderer renderer = stripeObject.AddComponent<MeshRenderer>();
        renderer.material = this.material;

        stripes = new List<Stripe>();
        Vector2 insideSize = size - Vector2.one * sideWidth;
        initialPosition = new Vector2( -insideSize.x, insideSize.y ) / 2f;
        Vector2 lineDirection = -initialPosition.normalized;

        int stripeCount = (int)Mathf.Floor( initialPosition.magnitude * 2f / ( stripeWidth + stripeSpacing ) );
        stripeSpacing += ( initialPosition.magnitude * 2f - stripeCount * ( stripeWidth + stripeSpacing ) ) / (float)stripeCount;

        Debug.Log( stripeCount );
        float currentLinePosition = 0;

        for (int i = 0; i < stripeCount; i++)
        {
            currentLinePosition += stripeSpacing / 2f + stripeWidth / 2f;
            Stripe stripe;
            stripe.position = initialPosition + lineDirection * currentLinePosition;
            stripe.lineDirection = lineDirection;

            currentLinePosition += stripeSpacing / 2f + stripeWidth / 2f;

            stripes.Add( stripe );
        }

        this.stripeWidth = stripeWidth;
        this.stripeMovementSpeed = stripeMovementSpeed;

        BuildMesh();
        visualParent.transform.rotation = Quaternion.Euler( Vector3.right * ( flat ? 90 : 0 ) );
    }

    /// <summary>
    /// Builds the mesh.
    /// </summary>
    void BuildMesh ()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        Vector2 defCorner = baseRectangle.size / 2f - Vector2.one * baseRectangle.sideWidth;
        Vector2[] corners = new Vector2[] { defCorner, -defCorner, new Vector2( -defCorner.x, defCorner.y ), new Vector2( defCorner.x, -defCorner.y ) };

        foreach (Stripe stripe in stripes)
        {
            Vector2 offset = stripe.lineDirection * stripeWidth / 2f;
            Vector3[] points = new Vector3[4];
            Vector2 endPoint = BorderRaycast( Vector2.zero, stripe.lineDirection, 2000f );
            Vector2 startPoint = BorderRaycast( Vector2.zero, -stripe.lineDirection, 2000f );

            if (( stripe.position + offset ).magnitude < defCorner.magnitude)
            {
                points[0] = BorderRaycast( stripe.position + offset, Vector2.one, 2000f );
                points[1] = BorderRaycast( stripe.position + offset, -Vector2.one, 2000f );
            }
            else
            {
                points[0] = endPoint;
                points[1] = endPoint;
            }

            if (( stripe.position - offset ).magnitude < defCorner.magnitude)
            {
                points[2] = BorderRaycast( stripe.position - offset, Vector2.one, 2000f );
                points[3] = BorderRaycast( stripe.position - offset, -Vector2.one, 2000f );
            }
            else
            {
                if ((Vector2)points[0] == endPoint)
                {
                    continue;
                }
                else
                {
                    points[2] = startPoint;
                    points[3] = startPoint;
                }
            }

            vertices.Add( points[0] );
            vertices.Add( points[1] );
            vertices.Add( points[2] );
            vertices.Add( points[3] );
            int rootIndex = vertices.Count - 4;

            triangles.Add( rootIndex );
            triangles.Add( rootIndex + 1 );
            triangles.Add( rootIndex + 3 );

            triangles.Add( rootIndex + 3 );
            triangles.Add( rootIndex + 2 );
            triangles.Add( rootIndex );


            for (int i = 1; i <= 2; i++)
            {
                Vector3 point1 = points[i - 1];
                Vector3 point2 = points[i + 1];

                Vector2 relativePosition = point1 - point2;
                Vector2 direction = point1 + point2;
                Vector3 point3 = Vector3.zero;

                if (!( Mathf.Abs( relativePosition.x ) < 0.005f ) && !( Mathf.Abs( relativePosition.y ) < 0.005f ))
                {
                    foreach (Vector2 corner in corners)
                    {
                        if ((int)Mathf.Sign( direction.x ) == (int)Mathf.Sign( corner.x ) && (int)Mathf.Sign( direction.y ) == (int)Mathf.Sign( corner.y ))
                        {
                            point3 = corner;
                            break;
                        }
                    }
                }

                if (point3 != Vector3.zero)
                {
                    vertices.Add( point3 );


                    triangles.Add( rootIndex + ( i - 1 ) );
                    if (i == 1)
                    {
                        triangles.Add( rootIndex + ( i + 1 ) );
                        triangles.Add( vertices.Count - 1 );
                    }
                    else
                    {
                        triangles.Add( vertices.Count - 1 );
                        triangles.Add( rootIndex + ( i + 1 ) );
                    }
                }
            }


        }

        List<Vector3> normals = new List<Vector3>();
        for (int i = 0; i < vertices.Count; i++)
        {
            normals.Add( Vector3.up );
        }

        stripeMesh.Clear();
        stripeMesh.SetVertices( vertices );
        stripeMesh.triangles = triangles.ToArray();
        stripeMesh.SetNormals( normals );
    }

    /// <summary>
    /// Casts a ray on the borders.
    /// </summary>
    /// <returns>The raycast.</returns>
    /// <param name="position">Position.</param>
    /// <param name="direction">Direction.</param>
    /// <param name="maxDistance">Max distance.</param>
    Vector2 BorderRaycast ( Vector2 position, Vector2 direction, float maxDistance )
    {
        direction = direction.normalized;
        Vector2 corner = baseRectangle.size / 2f - Vector2.one * baseRectangle.sideWidth;
        Vector2 ray = direction * maxDistance;

        Vector2 chosenAxis = new Vector2( Mathf.Sign( direction.x ), Mathf.Sign( direction.y ) );
        chosenAxis = new Vector2( Mathf.Approximately( chosenAxis.x, 0 ) ? 1 : chosenAxis.x, Mathf.Approximately( chosenAxis.y, 0 ) ? 1 : chosenAxis.y );

        Vector2 comparedCorner = Vector2.Scale( corner, chosenAxis );

        Vector2 positional = ( position - comparedCorner ).normalized;
        Vector2 determinant = Vector2.Scale( ( positional + direction ), chosenAxis );

        Vector2 trespassDirection = new Vector2( Mathf.Clamp01( determinant.x * 20f ), Mathf.Clamp01( determinant.y * 20f ) );
        Vector2 trespass = Vector2.Scale( trespassDirection, position + ray - comparedCorner );

        if (trespassDirection == Vector2.zero)
        {
            return comparedCorner;
        }

        float modifier = 1f - trespass.magnitude / Vector2.Scale( trespassDirection, ray ).magnitude;
        Vector2 result = position + ray * modifier;
        return result;
    }

    /// <summary>
    /// Moves the stripes.
    /// </summary>
    void MoveStripes ()
    {
        for (int i = 0; i < stripes.Count; i++)
        {
            Stripe stripe = stripes[i];
            stripe.position += stripe.lineDirection.normalized * stripeMovementSpeed * Time.deltaTime;
            stripes[i] = stripe;

            if (stripe.position.magnitude > baseRectangle.size.magnitude)
            {
                stripes.Remove( stripe );
                i--;
            }
        }

        if (( stripes[0].position - initialPosition ).magnitude > 1.5f * stripeWidth + stripeSpacing)
        {
            AddStripe();
        }

        BuildMesh();
    }

    /// <summary>
    /// Adds a stripe.
    /// </summary>
    void AddStripe ()
    {
        Stripe stripe;
        stripe.lineDirection = stripes[0].lineDirection;
        stripe.position = initialPosition - stripe.lineDirection * ( stripeWidth / 2f - ( ( stripes[0].position - initialPosition ).magnitude - ( 1.5f * stripeWidth + stripeSpacing ) ) );
        stripes.Insert( 0, stripe );
    }
}
