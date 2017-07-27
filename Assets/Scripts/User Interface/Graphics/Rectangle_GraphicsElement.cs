using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle_GraphicsElement : MonoBehaviour
{
    public Material material;
    public float sideWidth;
    public Vector2 size;
    GameObject visualParent;
    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void Set ( Vector2 size, float sideWidth, bool flat, float backgroundAlpha )
    {
        Destroy( visualParent );
        visualParent = new GameObject( "Visual Parent" );
        visualParent.transform.parent = transform;
        visualParent.transform.localPosition = Vector3.zero;

        for (int direction = -1; direction < 2; direction += 2)
        {
            Vector2 position = Vector2.right * ( size.x / 2f - sideWidth / 2f ) * direction;
            Vector3 scale = new Vector3( sideWidth, size.y - 2 * sideWidth, 1f );

            GameObject tmp = GameObject.CreatePrimitive( PrimitiveType.Quad );
            tmp.transform.parent = visualParent.transform;
            tmp.transform.localPosition = position;
            tmp.transform.localScale = scale;

            tmp.GetComponent<Renderer>().material = material;
        }

        for (int direction = -1; direction < 2; direction += 2)
        {
            Vector2 position = Vector2.up * ( size.y / 2f - sideWidth / 2f ) * direction;
            Vector3 scale = new Vector3( size.x, sideWidth, 1f );

            GameObject tmp = GameObject.CreatePrimitive( PrimitiveType.Quad );
            tmp.transform.parent = visualParent.transform;
            tmp.transform.localPosition = position;
            tmp.transform.localScale = scale;

            tmp.GetComponent<Renderer>().material = material;
        }

        if (backgroundAlpha > 0)
        {
            GameObject bg = GameObject.CreatePrimitive( PrimitiveType.Quad );
            bg.transform.SetParent( visualParent.transform );
            bg.transform.localPosition = Vector3.forward * 0.001f;
            bg.transform.localScale = size - Vector2.one * sideWidth * 2;
            Renderer rd = bg.GetComponent<Renderer>();

            MaterialPropertyBlock matBlock = new MaterialPropertyBlock();
            Color color = material.color;
            color.a = backgroundAlpha;
            matBlock.SetColor( "_Color", color );

            rd.material = material;
            rd.SetPropertyBlock( matBlock );
        }

        this.sideWidth = sideWidth;
        this.size = size;
        visualParent.transform.rotation = Quaternion.Euler( Vector3.right * ( flat ? 90 : 0 ) );
    }
}
