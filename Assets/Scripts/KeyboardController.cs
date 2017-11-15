using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {

    GameObject gm;
    public float speedCamera;
    void Start()
    {
        speedCamera = 10.0f;
        gm = GameObject.FindGameObjectWithTag("GameManager");
    }

    void Update()
    {
        if (gm.GetComponent<EditorModeController>().isEditorMode)
        {
            if (Input.GetKey(KeyCode.W))
            {
                Camera.main.transform.Translate(new Vector3(0.0f, Time.deltaTime * speedCamera, 0.0f));
            }
            if (Input.GetKey(KeyCode.A))
            {
                Camera.main.transform.Translate(new Vector3(-Time.deltaTime * speedCamera, 0.0f, 0.0f));
            }
            if (Input.GetKey(KeyCode.S))
            {
                Camera.main.transform.Translate(new Vector3(0.0f, -Time.deltaTime * speedCamera, 0.0f));
            }
            if (Input.GetKey(KeyCode.D))
            {
                Camera.main.transform.Translate(new Vector3(Time.deltaTime * speedCamera, 0.0f, 0.0f));
            }
        }
    }
}
