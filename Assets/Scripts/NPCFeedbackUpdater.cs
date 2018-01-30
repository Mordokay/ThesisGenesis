using System.Collections;
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
    public GameObject feedbackThinkingCanvas;

    public Slider assertivenessSlider;
    public Slider cooperativenessSlider;

    public Transform listAttributes;

    void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
    }

    private void Update()
    {
        if (uiManager.isFeedbackEnabled)
        {
            feedbackCanvas.SetActive(true);
            feedbackCanvas.transform.localPosition = npcObject.transform.localPosition;
            refreshFeedbackCanvas();
        }
        else
        {
            feedbackCanvas.SetActive(false);
        }

        if (feedbackMessageCanvas.activeSelf)
        {
            feedbackMessageCanvas.transform.localPosition = npcObject.transform.localPosition;
        }
        if (feedbackThinkingCanvas.activeSelf)
        {
            feedbackThinkingCanvas.transform.localPosition = npcObject.transform.localPosition;
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
        assertivenessSlider.gameObject.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(this.GetComponent<NPCData>().assertiveness * 100, 20);
        cooperativenessSlider.gameObject.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(this.GetComponent<NPCData>().cooperativeness * 100, 20);

        assertivenessSlider.value = this.GetComponent<NPCData>().currentAssertivenessLevel;
        cooperativenessSlider.value = this.GetComponent<NPCData>().currentCooperativenessLevel;
    }
}
