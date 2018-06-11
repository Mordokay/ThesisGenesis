using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Beacon : MonoBehaviour {

    public List<string> eventTags;
    public float timeBetweenEventsMin;
    public float timeBetweenEventsMax;
    public int tagValueMin;
    public int tagValueMax;
    public GameObject npcHolder;
    public float messageTime;
    public float timeToNextEvent;

    public bool isOnSequence;

    UIManager uiManager;

    public Message[] messageSequence;
    public float[] messageTimeOfSpawn;

    int currentMessageOnSequence;

    bool sequenceInitialized = false;

    public Text messageSequenceText;

    float timeToNextElementalBeaconEvent;


    void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        timeToNextEvent = 60.0f;
        currentMessageOnSequence = 0;

        if (!sequenceInitialized)
        {
            sequenceInitialized = true;
            messageSequence = new Message[15];
            messageTimeOfSpawn = new float[15];
        }
        //messageSequence = new Message[10];
        //messageTimeOfSpawn = new float[10];
        //GenerateMessageSequence();
        isOnSequence = false;


        //InvokeRepeating("SpawnElementalEvent", 10.0f, 20.0f);
        timeToNextElementalBeaconEvent = UnityEngine.Random.Range(5.0f, 15.0f);
        Debug.Log("timeToNextElementalBeaconEvent: " + timeToNextElementalBeaconEvent);
    }

    void SpawnElementalEvent()
    {
        // 10% of times, every 5 to 15 seconds, a pulse will happen on a normal object (NOT golden)
        if (UnityEngine.Random.Range(0, 100) < 10)
        {
            EditorModeController.Element element =
                this.GetComponent<EditorModeController>().NormalElementList[UnityEngine.Random.Range(0, this.GetComponent<EditorModeController>().NormalElementList.Count)];
            element.elementObject.GetComponent<ElementController>().BeaconPulse(false);
        }
        else
        {
            EditorModeController.Element element =
                this.GetComponent<EditorModeController>().GoldenElementList[UnityEngine.Random.Range(0, this.GetComponent<EditorModeController>().GoldenElementList.Count)];
            element.elementObject.GetComponent<ElementController>().BeaconPulse(true);
        }
    }

    public void RefreshMessageSequenceText()
    {
        string outputText = "";
        for(int i = 0; i < messageSequence.Length; i++)
        {
            if (messageTimeOfSpawn[i] - Time.timeSinceLevelLoad > 0.0f)
            {
                outputText += "TimeOfSpawn: " + (messageTimeOfSpawn[i] - Time.timeSinceLevelLoad) + " ID: " + messageSequence[i].id + System.Environment.NewLine;
                foreach (Message.Tag t in messageSequence[i].tags)
                {
                    outputText += "< " + t.name + " " + t.weight + "> ";
                }
                outputText += System.Environment.NewLine;
            }
        }
        messageSequenceText.text = outputText;
    }

    public void InitializeSequenceMessages()
    {
        sequenceInitialized = true;
        messageSequence = new Message[15];
        messageTimeOfSpawn = new float[15];
    }

    public void GenerateMessageSequence()
    {
        float rangeMin = 0.0f;
        float rangeMax = 50.0f;

        for (int i = 0; i < 15; i++)
        {
            //int eventID = this.GetComponent<PlayModeManager>().getMessageId();
            Message msg = CreateMessage(i);
            messageSequence[i] = msg;
            messageTimeOfSpawn[i] = UnityEngine.Random.Range(rangeMin, rangeMax);

            rangeMin += 50.0f;
            rangeMax += 50.0f;
        }
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
        timeToNextElementalBeaconEvent -= Time.deltaTime;
        if(timeToNextElementalBeaconEvent <= 0.0f)
        {
            SpawnElementalEvent();
            timeToNextElementalBeaconEvent = UnityEngine.Random.Range(5.0f, 15.0f);
            Debug.Log("timeToNextElementalBeaconEvent: " + timeToNextElementalBeaconEvent);
        }

        if (messageSequenceText.gameObject.transform.parent.gameObject.activeSelf)
        {
            RefreshMessageSequenceText();
        }
        if (isOnSequence && sequenceInitialized)
        {
            //Check if its time for the message to me spawned
            if(messageTimeOfSpawn[currentMessageOnSequence] <= Time.timeSinceLevelLoad)
            {
                GameObject selectedNPC = npcHolder.transform.GetChild(UnityEngine.Random.Range(0, npcHolder.transform.childCount)).gameObject;

                //rotates between random NPCs until it finds one who has interest in the message
                while (!selectedNPC.GetComponent<NPCData>().isMessageOfInterest(messageSequence[currentMessageOnSequence]))
                {
                    selectedNPC = npcHolder.transform.GetChild(UnityEngine.Random.Range(0, npcHolder.transform.childCount)).gameObject;
                }

                int eventID = this.GetComponent<PlayModeManager>().getMessageId();
                messageSequence[currentMessageOnSequence].id = eventID;

                selectedNPC.GetComponent<NPCData>().ReceiveMessage(new Message(messageSequence[currentMessageOnSequence]));

                uiManager.messageTrackingID.text = messageSequence[currentMessageOnSequence].id.ToString();
                selectedNPC.GetComponent<NPCFeedbackUpdater>().checkMessageFeedback();

                currentMessageOnSequence += 1;
            }
        }
        else
        {
            //Temporary removal of beacon ... only premade message sequence works

            /*
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
            */
        }
    }
}
