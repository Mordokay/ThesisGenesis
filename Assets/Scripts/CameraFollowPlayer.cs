using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour {

    GameObject myPlayer;
    public float cameraMoveSpeed;
    
    void Start () {
        myPlayer = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update () {
        if (!this.GetComponent<EditorModeController>().isEditorMode)
        {
            if (myPlayer == null)
            {
                myPlayer = GameObject.FindGameObjectWithTag("Player");
            }
            else
            {
                int mapWidth = this.GetComponent<EditorModeController>().mapWidth;
                int mapHeight = this.GetComponent<EditorModeController>().mapHeight;
                Camera.main.transform.position = new Vector3(myPlayer.transform.position.x, Camera.main.transform.position.y,
                    myPlayer.transform.position.z);
                Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -(mapWidth / 2 - 5.78f), (mapWidth / 2 - 5.78f)), Camera.main.transform.position.y,
                        Mathf.Clamp(Camera.main.transform.position.z, -(mapHeight / 2 - 3.4f), (mapHeight / 2 - 3.4f)));
                /*
                Vector3 camPos = Camera.main.transform.position;
                Vector3 dir = myPlayer.transform.position - camPos;
                int mapWidth = this.GetComponent<EditorModeController>().mapWidth;
                int mapHeight = this.GetComponent<EditorModeController>().mapHeight;

                dir.y = 0.0f;

                if (dir.magnitude > 0.05f)
                {
                    Camera.main.transform.position += dir * Time.deltaTime * cameraMoveSpeed;
                    //Camera.main.transform.position = myPlayer.transform.position;
                    Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -(mapWidth / 2 - 9.0f), (mapWidth / 2 - 9.0f)), 10.0f,
                        Mathf.Clamp(Camera.main.transform.position.z, -(mapHeight / 2 - 5.0f), (mapHeight / 2 - 5.0f)));
                }
                */
                //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(
                //    myPlayer.transform.position.x, 10.0f, myPlayer.transform.position.z), );
            }
        }
	}
}
