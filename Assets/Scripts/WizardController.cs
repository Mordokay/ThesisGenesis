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
        sawPlayerCanvas = this.transform.parent.transform.GetChild(5).gameObject;
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
            if(this.GetComponentInParent<NPCData>().messages.Count > 0)
            {
                wantsToFollowPlayer = true;
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
                    Debug.Log("I stopped following the player!!!");
                }
            }
            
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Time.timeScale = 0.0f;
            canvas.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
