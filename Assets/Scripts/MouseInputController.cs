using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour {

    GameObject gm;

    public LayerMask TerrainLayerMask;
    Vector3 lastMousePos;
    string lastTerrainTileClicked;

    void Start () {
        lastMousePos = Vector3.zero;
        gm = GameObject.FindGameObjectWithTag("GameManager");
        lastTerrainTileClicked = "";
    }
	
	void Update () {
        if (Input.GetMouseButton(0))
        {
            if (!lastMousePos.Equals(Input.mousePosition) && gm.GetComponent<EditorModeController>().isDrawingTerrain)
            {
                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, TerrainLayerMask);

                if (hit.collider != null && !lastTerrainTileClicked.Equals(hit.collider.gameObject.name))
                {
                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        lastTerrainTileClicked = hit.collider.gameObject.name;
                        Vector3 pos = hit.collider.gameObject.transform.position;
                        //Debug.Log(pos);
                        gm.GetComponent<EditorModeController>().SetTerrainAtPos((int)pos.x, (int)pos.z);
                    }
                    //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                }
                lastMousePos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (gm.GetComponent<EditorModeController>().isPlacingElements)
            {
                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, TerrainLayerMask);

                if (hit.collider != null)
                {

                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        //Vector3 pos = hit.collider.gameObject.transform.position;
                        //Debug.Log(pos);
                        gm.GetComponent<EditorModeController>().InsertElement(hit.point);
                    }
                    //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                }
            }
        }
    }
}
