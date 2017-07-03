using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPlacementUIIndicator : MonoBehaviour
{

    public GameObject sides;
    public GameObject cap1;
    public GameObject cap2;

    public void Adapt()
    {
        List<BoardTile> selectedTiles = ShipPositioner.selectedTiles;
        if (selectedTiles.Count != 0)
        {
            gameObject.SetActive(true);

            sides.transform.localScale = new Vector3(selectedTiles.Count, 0.1f, 1f);
            Vector3 relative = selectedTiles[selectedTiles.Count - 1].transform.position - selectedTiles[0].transform.position;
            gameObject.transform.position = relative / 2f + selectedTiles[0].transform.position + Vector3.up * 0.12f;
            gameObject.transform.rotation = Quaternion.Euler(Vector3.up * (relative.x == 0 ? 90 : 0));

            cap1.transform.localPosition = Vector3.forward * (relative.magnitude / 2f + 0.45f);
            cap2.transform.localPosition = Vector3.back * (relative.magnitude / 2f + 0.45f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
