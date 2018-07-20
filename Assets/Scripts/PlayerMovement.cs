using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isWalking;
    public bool isAtacking;
    public GameObject objectBeingAtacked;
    float attackIntervalTime;
    float timeSinceLastAtack;
    float attackDamage;

    public float moveSpeed;
    EditorModeController em;
    MouseInputController mic;

    void Start()
    {
        em = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        mic = GameObject.FindGameObjectWithTag("GameManager").GetComponent<MouseInputController>();
        timeSinceLastAtack = Time.timeSinceLevelLoad;
        attackDamage = 8.0f;
        attackIntervalTime = 0.5f;
    }


    void Update()
    {
        if (!em.isEditorMode)
        {
            if(objectBeingAtacked == null)
            {
                if (isAtacking)
                {
                    isAtacking = false;
                    this.GetComponentInChildren<Animator>().SetBool("Attack", false);
                }
            }
            if (isAtacking && Time.timeSinceLevelLoad - timeSinceLastAtack > attackIntervalTime)
            {
                objectBeingAtacked.GetComponent<ElementController>().Attack(attackDamage);
                timeSinceLastAtack = Time.timeSinceLevelLoad;
            }

            if (mic.eventSpawnerArea.activeSelf && Input.mousePosition.x > 0.76 * Screen.width)
            {
                this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                return;
            }

            Vector3 lookPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(transform.position.x, 10.0f, transform.position.z);
            float angle = Mathf.Atan2(lookPos.x, lookPos.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W)){
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                
                direction -= Vector3.right;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction -= Vector3.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            //Debug.Log(direction.ToString());
            direction.Normalize();
            //Debug.Log("NNNNN: " + direction.ToString());

            this.GetComponent<Rigidbody>().velocity = direction * moveSpeed;

            if (direction.Equals(Vector3.zero)){
                this.GetComponentInChildren<Animator>().SetBool("Walk", false);
                isWalking = false;
            }
            else
            {
                this.GetComponentInChildren<Animator>().SetBool("Walk", true);
                isWalking = true;
            }

            int mapWidth = em.mapWidth;
            int mapHeight = em.mapHeight;
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -mapWidth / 2 + 1, mapWidth / 2 - 1), transform.position.y,
                    Mathf.Clamp(transform.position.z, -mapHeight / 2 + 1, mapHeight / 2 - 1));

        }
    }
}