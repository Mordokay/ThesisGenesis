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
        goldenTreeQuestText.text = totalGoldenTreeGathered + "/10";
        goldenRockQuestText.text = totalGoldenRockGathered + "/10";
        goldenBerriesQuestText.text = totalGoldenBerriesGathered + "/10";
        goldenCactusQuestText.text = totalGoldenCactusGathered + "/10";
    }

    public void IncrementQuestsPanel(int type)
    {
        switch (type)
        {
            case 0:
                totalGoldenTreeGathered += 1;
                if (totalGoldenTreeGathered > 10)
                {
                    totalGoldenTreeGathered = 10;
                }
                break;
            case 1:
                totalGoldenRockGathered += 1;
                if (totalGoldenRockGathered > 10)
                {
                    totalGoldenRockGathered = 10;
                }
                break;
            case 2:
                totalGoldenBerriesGathered += 1;
                if (totalGoldenBerriesGathered > 10)
                {
                    totalGoldenBerriesGathered = 10;
                }
                break;
            case 3:
                totalGoldenCactusGathered += 1;
                if (totalGoldenCactusGathered > 10)
                {
                    totalGoldenCactusGathered = 10;
                }
                break;
        }
        RefreshQuestsText();
    }

    void Update () {
	    if(totalGoldenTreeGathered + totalGoldenRockGathered + totalGoldenBerriesGathered + totalGoldenCactusGathered == 40)
        {
            //Player wins the game and shows panel!!!
            Time.timeScale = 0.0f;
            canvas.transform.GetChild(1).gameObject.SetActive(true);
        }
	}
}
