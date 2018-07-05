using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsController : MonoBehaviour {

    public int totalGoldenTreeGathered;
    public int totalGoldenRockGathered;
    public int totalGoldenBerriesGathered;
    public int totalGoldenCactusGathered;

    public Text goldenTreeQuestText;
    public Text goldenRockQuestText;
    public Text goldenBerriesQuestText;
    public Text goldenCactusQuestText;

    GameObject canvas;

    void Start () {
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        totalGoldenTreeGathered = 0;
        totalGoldenRockGathered = 0;
        totalGoldenBerriesGathered = 0;
        totalGoldenCactusGathered = 0;
    }

    public void RefreshQuestsText()
    {
        goldenTreeQuestText.text = totalGoldenTreeGathered + "/4";
        goldenRockQuestText.text = totalGoldenRockGathered + "/4";
        goldenBerriesQuestText.text = totalGoldenBerriesGathered + "/4";
        goldenCactusQuestText.text = totalGoldenCactusGathered + "/4";
    }

    public void IncrementQuestsPanel(int type)
    {
        switch (type)
        {
            case 0:
                totalGoldenTreeGathered += 1;
                if (totalGoldenTreeGathered > 3)
                {
                    totalGoldenTreeGathered = 3;
                }
                else
                {
                    //sends grabing data to database
                    StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("tree"));
                }
                break;
            case 1:
                totalGoldenRockGathered += 1;
                if (totalGoldenRockGathered > 3)
                {
                    totalGoldenRockGathered = 3;
                }
                else
                {
                    //sends grabing data to database
                    StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("rock"));
                }
                break;
            case 2:
                totalGoldenBerriesGathered += 1;
                if (totalGoldenBerriesGathered > 3)
                {
                    totalGoldenBerriesGathered = 3;
                }
                else
                {
                    //sends grabing data to database
                    StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("berries"));
                }
                break;
            case 3:
                totalGoldenCactusGathered += 1;
                if (totalGoldenCactusGathered > 3)
                {
                    totalGoldenCactusGathered = 3;
                }
                else
                {
                    //sends grabing data to database
                    StartCoroutine(this.GetComponent<MySQLManager>().LogEventAtTime("cactus"));
                }
                break;
        }
        RefreshQuestsText();
    }

    void Update () {
	    if(totalGoldenTreeGathered + totalGoldenRockGathered + totalGoldenBerriesGathered + totalGoldenCactusGathered == 12)
        {
            //Player wins the game and shows panel!!!
            Time.timeScale = 0.0f;
            canvas.transform.GetChild(1).gameObject.SetActive(true);
        }
	}
}
