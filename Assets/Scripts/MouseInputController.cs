using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour {

    GameObject gm;

    public LayerMask TerrainLayerMask;
    public LayerMask UndergroundLayerMask;
    public LayerMask ElementLayerMask;
    public LayerMask PatrolLayerMask;

    Vector3 lastMousePos;
    public string lastTerrainTileClicked;
    string lastUndergroundTileClicked;

    void Start () {
        lastMousePos = Vector3.zero;
        gm = GameObject.FindGameObjectWithTag("GameManager");
        lastTerrainTileClicked = "";
        lastUndergroundTileClicked = "";
    }
	
	void Update () {
        if (Input.GetMouseButton(0))
        {
            if (!lastMousePos.Equals(Input.mousePosition) && gm.GetComponent<EditorModeController>().isDrawingTerrain)
            {
                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, TerrainLayerMask);

                if (hit.collider != null && !lastTerrainTileClicked.Equals(hit.collider.gameObject.name))
                {

                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        lastTerrainTileClicked = hit.collider.gameObject.name;
                        Vector3 pos = hit.collider.gameObject.transform.position;
                        //Debug.Log(pos);
                        if (gm.GetComponent<EditorModeController>().removeTerrain)
                        {
                            gm.GetComponent<EditorModeController>().removeTerrainAtPos((int)pos.x, (int)pos.z);
                        }
                        else
                        {
                            gm.GetComponent<EditorModeController>().SetTerrainAtPos((int)pos.x, (int)pos.z);
                        }
                    }
                    Debug.Log("Target Position: " + hit.collider.gameObject.transform.position + " TAG: " + hit.collider.tag);
                }
                lastMousePos = Input.mousePosition;
            }
        }
        //fillLayer
        if (Input.GetMouseButton(1))
        {
            if (!lastMousePos.Equals(Input.mousePosition) && gm.GetComponent<EditorModeController>().isDrawingTerrain)
            {
                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, UndergroundLayerMask);

                if (hit.collider != null && !lastUndergroundTileClicked.Equals(hit.collider.gameObject.name))
                {
                    if (hit.collider.tag.Equals("Underground"))
                    {
                        lastUndergroundTileClicked = hit.collider.gameObject.name;
                        Vector3 pos = hit.collider.gameObject.transform.position;
                        //Debug.Log(pos);
                        gm.GetComponent<EditorModeController>().SetUndergroundAtPos((int)pos.x, (int)pos.z);
                    }
                    //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position + " TAG: " + hit.collider.tag);
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
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, TerrainLayerMask);

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

            else if (gm.GetComponent<EditorModeController>().removeElement)
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, ElementLayerMask);

                if (hit.collider != null)
                {
                    Debug.Log("I just removed an element!!!!");
                    gm.GetComponent<EditorModeController>().RemoveElement(hit.collider.gameObject);
                    //if (hit.collider.tag.Equals("Element"))
                    //{
                    //Vector3 pos = hit.collider.gameObject.transform.position;
                    //    Debug.Log("I just removed an element!!!!");
                    //gm.GetComponent<EditorModeController>().InsertElement(hit.point);
                    //}
                }
            }
            else if (gm.GetComponent<EditorModeController>().removePatrolPoint)
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, PatrolLayerMask);

                if (hit.collider != null)
                {
                    //Debug.Log("I just removed a patrol point!!!!");
                    gm.GetComponent<EditorModeController>().RemovePatrolPoint(hit.collider.gameObject);
                    //if (hit.collider.tag.Equals("Element"))
                    //{
                    //Vector3 pos = hit.collider.gameObject.transform.position;
                    //   Debug.Log("I just removed a patrol point!!!!");
                    //gm.GetComponent<EditorModeController>().InsertElement(hit.point);
                    //}
                }
            }
        }
    }
}
