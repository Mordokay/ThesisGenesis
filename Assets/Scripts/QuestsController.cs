using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsController : MonoBehaviour {

    public int totalGoldenTreeGathered;
    public int totalGoldenRockGathered;
    public int totalGoldenBerriesGathered;
    public int totalGoldenCactusGathered;

    public int totalGoldenObjectsGathered;
    public Slider progressBar;
    public Text progressBarText;
    GameObject canvas;

    void Start () {
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        totalGoldenObjectsGathered = 0;
        UpdateQuestsBar();

        totalGoldenTreeGathered = 0;
        totalGoldenRockGathered = 0;
        totalGoldenBerriesGathered = 0;
        totalGoldenCactusGathered = 0;
    }

    public void UpdateQuestsBar()
    {
        progressBar.value = totalGoldenObjectsGathered / 12.0f;
        progressBarText.text = totalGoldenObjectsGathered + "/12";
    }

    public void IncrementQuestbar(int type)
    {
        totalGoldenObjectsGathered += 1;
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
        UpdateQuestsBar();
    }

    void Update () {
	    if(totalGoldenTreeGathered + totalGoldenRockGathered + totalGoldenBerriesGathered + totalGoldenCactusGathered == 12)
        {
            //Player wins the game and shows panel!!!
            Time.timeScale = 0.0f;
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            StartCoroutine(this.GetComponent<MySQLManager>().SendsDataToDatabase());
        }
	}
}
