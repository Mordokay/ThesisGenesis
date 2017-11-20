using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {

    public float zoomSpeed;
    public float orthographicSizeMin;
    public float orthographicSizeMax;
    private Camera myCamera;
    public GameObject leftUIBox;
    public GameObject rightUIBox;
    public bool zoomToPlayMode;

    EditorModeController em;

    // Use this for initialization
    void Start()
    {
        em = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        zoomToPlayMode = false;
        myCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // leftUIBox.transform.position = myCamera.ScreenToWorldPoint(new Vector3(0.0449f * Screen.width, 0.97f * Screen.height, 5.0f));
        // leftUIBox.transform.position = new Vector3(-9.0f + (Camera.main.orthographicSize - 5) * 1.8f, 5.3f + (Camera.main.orthographicSize - 5) * 0.8f, 5.0f);
        //Debug.Log(myCamera.WorldToScreenPoint(leftUIBox.transform.position));
        //Debug.Log(myCamera.WorldToScreenPoint(rightUIBox.transform.position));
        if (zoomToPlayMode)
        {
            if (Camera.main.orthographicSize - 5.0f < 0.05f)
            {
                Camera.main.orthographicSize = 5.0f;
                zoomToPlayMode = false;
            }
            else if (Camera.main.orthographicSize > 5.0)
            {
                Camera.main.orthographicSize -= Time.deltaTime * 5.0f;
            }
            else
            {
                Camera.main.orthographicSize += Time.deltaTime * 5.0f;
            }
        }

        if (em.isEditorMode && Input.mousePosition.x < 0.78 * Screen.width)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                myCamera.orthographicSize += zoomSpeed;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                myCamera.orthographicSize -= zoomSpeed;
            }
            myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
        }
    }
}
