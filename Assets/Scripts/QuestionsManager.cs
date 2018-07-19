using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsManager : MonoBehaviour {

    public int[] questionsAnswers = new int[33];

    public GameObject submitButton;
    public InputField inputField;
    public GameObject quitButton;

    public void SetQuestion(string question_value)
    {
        string[] data = question_value.Split('_');
        int question = System.Convert.ToInt32(data[0]);
        int value = System.Convert.ToInt32(data[1]);
        if (questionsAnswers[question] == 0)
        {
            questionsAnswers[question] = value;
        }
        else
        {
            questionsAnswers[question] = 0;
        }
    }
    
    public void SendQuestionaireData()
    {
        StartCoroutine(SendQuestionaireDataEnumerator());
    }

    public IEnumerator SendQuestionaireDataEnumerator()
    {
        yield return StartCoroutine(this.GetComponent<MySQLManager>().RecordData(questionsAnswers));
        submitButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);
    }

    public void Update()
    {
        bool allAnswered = true;
        for(int i = 0; i < questionsAnswers.Length; i++)
        {
            if(questionsAnswers[i] == 0)
            {
                allAnswered = false;
            }
        }

        if(allAnswered)
        {
            submitButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            submitButton.GetComponent<Button>().interactable = false;
        }
    }
}
