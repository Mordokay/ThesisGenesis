using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour {

    public GameObject prophetBaloon;

    public int tutorialStage;
    public GameObject[] tutorials;

    bool usedW;
    bool usedA;
    bool usedS;
    bool usedD;

    public Text TextW;
    public Text TextA;
    public Text TextS;
    public Text TextD;

    void Start () {
        tutorialStage = 0;
        usedW = false;
        usedA = false;
        usedS = false;
        usedD = false;
    }
	
    public void NextTutorial()
    {
        tutorialStage += 1;
        if (tutorialStage < tutorials.Length)
        {
            RefreshTutorialTexts();
        }
    }

    void RefreshTutorialTexts()
    {
        foreach(GameObject tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }
        tutorials[tutorialStage].SetActive(true);
    }

	void Update () {
        //Check if the tutorial has ended
        if (tutorialStage >= tutorials.Length)
        {
            prophetBaloon.SetActive(false);
        }
        else if(tutorialStage == 6)
        {
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                usedW = true;
                TextW.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                usedA = true;
                TextA.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                usedS = true;
                TextS.color = Color.red;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                usedD = true;
                TextD.color = Color.red;
            }

            if (usedW && usedA && usedS && usedD)
            {
                NextTutorial();
            }
        }
        else if (tutorialStage == 7 && Input.GetMouseButtonDown(0))
        {
            NextTutorial();
        }
        else if (tutorialStage == 14 && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)))
        {
            NextTutorial();
        }
    }
}
