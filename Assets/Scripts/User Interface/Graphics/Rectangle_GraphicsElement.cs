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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Set(Vector2 size, float sideWidth, bool flat)
    {
        Destroy(visualParent);
        visualParent = new GameObject("Visual Parent");
        visualParent.transform.parent = transform;
        visualParent.transform.localPosition = Vector3.zero;

        for (int direction = -1; direction < 2; direction += 2)
        {
            Vector2 position = Vector2.right * (size.x / 2f - sideWidth / 2f) * direction;
            Vector3 scale = new Vector3(sideWidth, size.y - 2 * sideWidth, 1f);

            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tmp.transform.parent = visualParent.transform;
            tmp.transform.localPosition = position;
            tmp.transform.localScale = scale;

            tmp.GetComponent<Renderer>().material = material;
        }

        for (int direction = -1; direction < 2; direction += 2)
        {
            Vector2 position = Vector2.up * (size.y / 2f - sideWidth / 2f) * direction;
            Vector3 scale = new Vector3(size.x, sideWidth, 1f);

            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tmp.transform.parent = visualParent.transform;
            tmp.transform.localPosition = position;
            tmp.transform.localScale = scale;

            tmp.GetComponent<Renderer>().material = material;
        }


        this.sideWidth = sideWidth;
        this.size = size;
        visualParent.transform.rotation = Quaternion.Euler(Vector3.right * (flat ? 90 : 0));
    }
}
