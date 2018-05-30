using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : MonoBehaviour {

    GameObject player;

    public float minimumFollowDistance;
    public bool wantsToFollowPlayer;
    public bool isFollowingPlayer;
    string myInterest;
    GameObject sawPlayerCanvas;
    GameObject canvas;

    void Start()
    {
        sawPlayerCanvas = this.transform.parent.transform.GetChild(6).gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        myInterest = this.GetComponentInParent<NPCData>().interests[0].name;
        minimumFollowDistance = 5.0f;
        InvokeRepeating("CheckInterests", 0.0f, 1.0f);
    }

    void CheckInterests()
    {
        if (!wantsToFollowPlayer)
        {
            foreach(Message m in this.GetComponentInParent<NPCData>().messages)
            {
                Message.Tag t = m.tags.Find(x => x.name == this.GetComponentInParent<NPCData>().interests[0].name);
                if(t != null && m.description.Contains("Golden"))
                {
                    wantsToFollowPlayer = true;
                }
                else
                {
                    wantsToFollowPlayer = false;
                }
            }
        }
    }

    void Update () {
        if (sawPlayerCanvas.activeSelf)
        {
            sawPlayerCanvas.transform.position = this.transform.position;
        }
        if (wantsToFollowPlayer)
        {
            if(Vector3.Distance(this.transform.position, player.transform.position) < minimumFollowDistance)
            {
                if (!isFollowingPlayer)
                {
                    isFollowingPlayer = true;
                    sawPlayerCanvas.SetActive(true);
                    Debug.Log("I started following the player!!!");
                }
            }
            else
            {
                if (isFollowingPlayer)
                {
                    isFollowingPlayer = false;
                    sawPlayerCanvas.SetActive(false);
                    this.transform.LookAt(this.GetComponent<NPCPatrolMovement>().currentGoalObject.transform);
                    Debug.Log("I stopped following the player!!!");
                }
            }
            
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && wantsToFollowPlayer)
        {
            Time.timeScale = 0.0f;
            canvas.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
