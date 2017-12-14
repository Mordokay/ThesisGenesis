﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCFeedbackUpdater : MonoBehaviour {

    public GameObject feedbackCanvas;
    UIManager uiManager;

    public GameObject npcObject;
    public GameObject npcFeedbackLine;

    public Text feedbackMessageNumberText;
    public GameObject feedbackMessageCanvas;

    public Text npcNameText;
    public Text timeText;
    public Text assertivenessText;
    public Text cooperativenessText;

    public Transform listAttributes;

    void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        //A Line to separate Stuff
        GameObject myLine = Instantiate(npcFeedbackLine, listAttributes);
        myLine.GetComponentInChildren<Text>().text = "-------------------------------";

        foreach (NPCData.Interest i in GetComponent<NPCData>().interests)
        {
            myLine = Instantiate(npcFeedbackLine, listAttributes);
            myLine.GetComponentInChildren<Text>().text = i.name + " : " + (i.weight * 100).ToString("F2");
        }
    }

    private void Update()
    {
        if (this.gameObject.GetComponent<Social>().isTalking)
        {
            feedbackCanvas.SetActive(false);
        }
        else
        {
            if (uiManager.isFeedbackEnabled)
            {
                feedbackCanvas.SetActive(true);
                feedbackCanvas.transform.localPosition = npcObject.transform.localPosition;
                refreshFeedbackCanvas();
            }
        }

        if (feedbackMessageCanvas.activeSelf)
        {
            feedbackMessageCanvas.transform.localPosition = npcObject.transform.localPosition;
        }
    }

    public void checkMessageFeedback()
    {
        if (uiManager.isMessageLayerEnabled)
        {
            if (uiManager.messageTrackingID.text != "")
            {
                //Check if NPC has the message being tracked
                if (GetComponent<NPCData>().messages.Find(x => x.id == System.Int32.Parse(uiManager.messageTrackingID.text)) != null)
                {
                    feedbackMessageNumberText.text = uiManager.messageTrackingID.text;
                    feedbackMessageCanvas.SetActive(true);
                }
                else
                {
                    feedbackMessageCanvas.SetActive(false);
                }
            }
        }
    }

    public void refreshFeedbackCanvas()
    {
        //Debug.Log("Refreshing feedback canvas of " + this.name);

        npcNameText.text = "Name: " + this.GetComponent<NPCData>().npcName;
        timeText.text = "Time: " + Time.deltaTime.ToString();
        assertivenessText.text = "Assertive: " + this.GetComponent<NPCData>().assertiveness.ToString("F4");
        cooperativenessText.text = "Cooperative: " + this.GetComponent<NPCData>().cooperativeness.ToString("F4");
    }
}
