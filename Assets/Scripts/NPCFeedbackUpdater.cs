using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCFeedbackUpdater : MonoBehaviour {

    public GameObject feedbackCanvas;
    UIManager uiManager;

    public GameObject npcObject;

    public Text npcNameText;
    public Text timeText;

    void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
    }

    private void Update()
    {
        feedbackCanvas.transform.localPosition = npcObject.transform.localPosition;
        refreshFeedbackCanvas();
    }

    public void refreshFeedbackCanvas()
    {
        Debug.Log("Refreshing feedback canvas of " + this.name);

        npcNameText.text = "Name: " + this.GetComponent<NPCData>().npcName;
        timeText.text = "Time: " + Time.deltaTime.ToString();
    }
}
