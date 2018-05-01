using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour {

    public List<string> eventTags;
    public float timeBetweenEventsMin;
    public float timeBetweenEventsMax;
    public int tagValueMin;
    public int tagValueMax;
    public GameObject npcHolder;
    public float messageTime;
    public float timeToNextEvent;

    UIManager uiManager;

    void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        timeToNextEvent = 60.0f;
        /*
        if (npcHolder.transform.childCount > 0) {
            SpawnEvent();
        }
        else
        {
            timeToNextEvent = UnityEngine.Random.Range(timeBetweenEventsMin, timeBetweenEventsMax);
        }*/
    }

    private void SpawnEvent()
    {
        //selectedNPC.GetComponent<NPCData>().isMessageOfInterest(Message msg);

        int eventID = this.GetComponent<PlayModeManager>().getMessageId();
        Message msg = CreateMessage(eventID);
        GameObject selectedNPC = npcHolder.transform.GetChild(UnityEngine.Random.Range(0, npcHolder.transform.childCount)).gameObject;

        //rotates between random NPCs until it finds one who has interest in the message
        while (!selectedNPC.GetComponent<NPCData>().isMessageOfInterest(msg))
        {
            selectedNPC = npcHolder.transform.GetChild(UnityEngine.Random.Range(0, npcHolder.transform.childCount)).gameObject;
        }

        selectedNPC.GetComponent<NPCData>().ReceiveMessage(msg);

        uiManager.messageTrackingID.text = eventID.ToString();
        selectedNPC.GetComponent<NPCFeedbackUpdater>().checkMessageFeedback();

        timeToNextEvent = UnityEngine.Random.Range(timeBetweenEventsMin, timeBetweenEventsMax);

        Debug.Log("Spawned event " + msg.ToString() + "  " + selectedNPC.name);
    }

    public Message CreateMessage(int eventID)
    {
        string tagA = eventTags[UnityEngine.Random.Range(0, eventTags.Count)];
        string tagB = eventTags[UnityEngine.Random.Range(0, eventTags.Count)];

        int valueTagA = UnityEngine.Random.Range(tagValueMin, tagValueMax);
        int valueTagB = UnityEngine.Random.Range(tagValueMin, tagValueMax);

        while (tagA == tagB)
        {
            tagB = eventTags[UnityEngine.Random.Range(0, eventTags.Count)];
        }
        string TAGS = tagA + " " + valueTagA + "," + tagB + " " + valueTagB;

        return new Message(eventID, messageTime, "This is an event " + tagA + " and " + tagB, TAGS);
    }

    void Update () {
        timeToNextEvent -= Time.deltaTime;
        if (timeToNextEvent <= 0)
        {
            if (npcHolder.transform.childCount > 0)
            {
                SpawnEvent();
            }
            else
            {
                timeToNextEvent = UnityEngine.Random.Range(timeBetweenEventsMin, timeBetweenEventsMax);
            }
        }
    }
}
