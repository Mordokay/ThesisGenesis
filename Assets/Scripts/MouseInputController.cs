using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour {

    GameObject gm;

    public LayerMask TerrainLayerMask;
    public LayerMask UndergroundLayerMask;
    public LayerMask ElementLayerMask;
    public LayerMask PlacingElementLayerMask;
    public LayerMask PatrolLayerMask;
    public LayerMask NPCLayerMask;

    Vector3 lastMousePos;
    public string lastTerrainTileClicked;
    string lastUndergroundTileClicked;

    GameObject player;

    UIManager uiManager;

    void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastMousePos = Vector3.zero;
        gm = GameObject.FindGameObjectWithTag("GameManager");
        lastTerrainTileClicked = "";
        lastUndergroundTileClicked = "";
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!lastMousePos.Equals(Input.mousePosition) && gm.GetComponent<EditorModeController>().isDrawingTerrain)
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, TerrainLayerMask);

                if (hit.collider != null && !lastTerrainTileClicked.Equals(hit.collider.gameObject.name))
                {
                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        lastTerrainTileClicked = hit.collider.gameObject.name;
                        Vector3 pos = hit.collider.gameObject.transform.position;
                        if (gm.GetComponent<EditorModeController>().removeTerrain)
                        {
                            gm.GetComponent<EditorModeController>().removeTerrainAtPos((int)pos.x, (int)pos.z);
                        }
                        else
                        {
                            gm.GetComponent<EditorModeController>().SetTerrainAtPos((int)pos.x, (int)pos.z);
                        }
                    }
                }
                lastMousePos = Input.mousePosition;
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (!lastMousePos.Equals(Input.mousePosition) && gm.GetComponent<EditorModeController>().isDrawingTerrain)
            {
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, UndergroundLayerMask);

                if (hit.collider != null && !lastUndergroundTileClicked.Equals(hit.collider.gameObject.name))
                {
                    if (hit.collider.tag.Equals("Underground"))
                    {
                        lastUndergroundTileClicked = hit.collider.gameObject.name;
                        Vector3 pos = hit.collider.gameObject.transform.position;
                        gm.GetComponent<EditorModeController>().SetUndergroundAtPos((int)pos.x, (int)pos.z);
                    }
                }
                lastMousePos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            //player stops attacking
            if (!gm.GetComponent<EditorModeController>().isEditorMode)
            {
                player.GetComponent<PlayerMovement>().isAtacking = false;
                player.GetComponent<Animator>().SetBool("Attack", false);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            //player attacks
            if (!gm.GetComponent<EditorModeController>().isEditorMode)
            {
                player.GetComponent<PlayerMovement>().isAtacking = true;
                player.GetComponent<Animator>().SetBool("Attack", true);
            }
            else
            {
                if (gm.GetComponent<EditorModeController>().isPlacingElements)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, PlacingElementLayerMask);

                    if (hit.collider != null)
                    {
                        if (hit.collider.tag.Equals("Terrain"))
                        {
                            Vector3 pos = hit.collider.gameObject.transform.position;
                            gm.GetComponent<EditorModeController>().InsertElement(pos);
                        }
                    }
                }
                else if (gm.GetComponent<EditorModeController>().isPlacingNPC)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, TerrainLayerMask);

                    if (hit.collider != null)
                    {
                        if (hit.collider.tag.Equals("Terrain"))
                        {
                            Vector3 pos = hit.collider.gameObject.transform.position;
                            gm.GetComponent<EditorModeController>().InsertNPC(pos);
                        }
                    }
                }

                else if (gm.GetComponent<EditorModeController>().removeElement)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, ElementLayerMask);

                    if (hit.collider != null)
                    {
                        gm.GetComponent<EditorModeController>().RemoveElement(hit.collider.gameObject);
                    }
                }
                else if (gm.GetComponent<EditorModeController>().removePatrolPoint)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, PatrolLayerMask);

                    if (hit.collider != null)
                    {
                        gm.GetComponent<EditorModeController>().RemovePatrolPoint(hit.collider.gameObject);
                    }
                }
                else if (gm.GetComponent<EditorModeController>().removeNPC)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, NPCLayerMask);

                    if (hit.collider != null && hit.collider.tag == "NPC")
                    {
                        Destroy(hit.collider.gameObject.transform.parent.gameObject);
                    }
                }
                else if (gm.GetComponent<EditorModeController>().isInspectingElement)
                {
                    RaycastHit hit;
                    Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.down, out hit, Mathf.Infinity, NPCLayerMask);

                    if (hit.collider != null && hit.collider.tag == "NPC")
                    {
                        Debug.Log("I am gonna inspect NPC called: " + hit.collider.gameObject.transform.parent.gameObject.name);
                        uiManager.RefreshNPCUpdater(hit.collider.gameObject.GetComponentInParent<NPCData>());
                    }
                }
            }
        }
    }
}