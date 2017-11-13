using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour {

    GameObject gm;

	void Start () {
        gm = GameObject.FindGameObjectWithTag("GameManager");
    }
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                if(hit.collider.tag.Equals("Terrain") && gm.GetComponent<EditorModeController>().isDrawingTerrain)
                {
                    Vector3 pos = hit.collider.gameObject.transform.position;
                    gm.GetComponent<EditorModeController>().SetTerrainAtPos((int)pos.x, (int)pos.y);
                }
                //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
            }
        }
    }
}
