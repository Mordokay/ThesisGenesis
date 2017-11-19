using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    bool isMoving;
    public float moveSpeed;
    EditorModeController em;

    void Start()
    {
        em = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
    }


    void Update()
    {
        if (!em.isEditorMode)
        {
            Vector3 lookPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(transform.position.x, 10.0f, transform.position.z);
            float angle = Mathf.Atan2(lookPos.x, lookPos.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W)){
                direction += new Vector3(0.0f, 0.0f, moveSpeed);
                //this.GetComponent<Animator>().SetTrigger("SomeMoveAnimation");
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += new Vector3(-moveSpeed, 0.0f, 0.0f);
                //this.GetComponent<Animator>().SetTrigger("SomeMoveAnimation");
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += new Vector3(0.0f, 0.0f, -moveSpeed);
                //this.GetComponent<Animator>().SetTrigger("SomeMoveAnimation");
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += new Vector3(moveSpeed, 0.0f, 0.0f);
                //this.GetComponent<Animator>().SetTrigger("SomeMoveAnimation");
            }
            this.GetComponent<Rigidbody>().velocity = direction;
        }
    }
}