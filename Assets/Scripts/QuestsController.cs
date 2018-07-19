using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsController : MonoBehaviour {

    public int totalGoldenTreeGathered;
    public int totalGoldenRockGathered;
    public int totalGoldenBerriesGathered;
    public int totalGoldenCactusGathered;

    public int playerStash;
    public int totalGoldenObjectsGathered;
    public Slider progressBar;
    public Text progressBarText;
    GameObject canvas;

    public GameObject[] StashImages;
    bool sentData;

    void Start () {
        sentData = false;
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        totalGoldenObjectsGathered = 0;
        UpdateQuestsBar();
        playerStash = 0;
        totalGoldenTreeGathered = 0;
        totalGoldenRockGathered = 0;
        totalGoldenBerriesGathered = 0;
        totalGoldenCactusGathered = 0;
    }

    public void UpdateQuestsBar()
    {
        if(playerStash > 0)
        {
            totalGoldenObjectsGathered += playerStash;
            playerStash = 0;
        }
        progressBar.value = totalGoldenObjectsGathered / 12.0f;
        progressBarText.text = totalGoldenObjectsGathered + "/12";

        foreach(GameObject imageStash in StashImages)
        {
            imageStash.SetActive(false);
        }
    }

    public void IncrementStash(int type)
    {
        StashImages[playerStash].SetActive(true);
        foreach(Transform child in StashImages[playerStash].transform)
        {
            child.gameObject.SetActive(false);
        }
        StashImages[playerStash].transform.GetChild(type).gameObject.SetActive(true);
        playerStash += 1;
        
        switch (type)
        {
            case 0:
                totalGoldenTreeGathered += 1;
                //sends grabing data to database
                StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("tree"));
                break;
            case 1:
                //sends grabing data to database
                StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("rock"));
                break;
            case 2:
                totalGoldenBerriesGathered += 1;
                //sends grabing data to database
                StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("berries"));
                break;
            case 3:
                totalGoldenCactusGathered += 1;
                //sends grabing data to database
                StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("cactus"));
                break;
        }
    }

    void Update () {
	    if(totalGoldenObjectsGathered == 12 && !sentData)
        {
            //Player wins the game and shows panel!!!
            Time.timeScale = 0.0f;
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            StartCoroutine(this.GetComponent<MySQLManager>().SendsDataToDatabase());
            sentData = true;
        }
	}
}
