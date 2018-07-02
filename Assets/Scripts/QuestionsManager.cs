using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsManager : MonoBehaviour {

    public int Q1Value;
    public int Q2Value;
    public int Q3Value;
    public int Q4Value;
    public int Q5Value;

    public GameObject submitButton;
    public InputField inputField;
    public GameObject quitButton;

    private void Start()
    {
        Q1Value = -1;
        Q2Value = -1;
        Q3Value = -1;
        Q4Value = -1;
        Q5Value = -1;
    }

    public void SetQuestion1(int value)
    {
        Q1Value = value;
    }

    public void SetQuestion2(int value)
    {
        Q2Value = value;
    }

    public void SetQuestion3(int value)
    {
        Q3Value = value;
    }

    public void SetQuestion4(int value)
    {
        Q4Value = value;
    }

    public void SetQuestion5(int value)
    {
        Q5Value = value;
    }

    public void SendQuestionaireData()
    {
        StartCoroutine(SendQuestionaireDataEnumerator());
    }

    public IEnumerator SendQuestionaireDataEnumerator()
    {
        yield return StartCoroutine(this.GetComponent<MySQLManager>().RecordData(Q1Value, Q2Value, Q3Value, Q4Value, Q5Value, inputField.text));
        submitButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);
    }

    public void Update()
    {
        if(Q1Value != -1 && Q2Value != -1 && Q3Value != -1 && Q4Value != -1 && Q5Value != -1)
        {
            submitButton.GetComponent<Button>().interactable = true;
        }
    }
}
