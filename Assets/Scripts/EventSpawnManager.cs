using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpawnManager : MonoBehaviour {

    PlayModeManager pm;
    EditorModeController emc;
    UIManager uiManager;

    public List<Message.Tag> tags;
    public float messageSendDistance;
    public float messageTime;
    public string description;

    private void Start()
    {
        emc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayModeManager>();
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
    }

    public void InitializeSpawnEvent(List<Message.Tag> myTags, float distance, float time, string desc)
    {
        tags = myTags;
        messageSendDistance = distance;
        messageTime = time;
        description = desc;
        Debug.Log("Initializing event: " + desc + " with time: " + time + " and distance: " + distance);
    }

    private void Update()
    {
        StartEvent();
    }

    public void StartEvent()
    {
        if (!description.Equals(""))
        {
            int eventId = pm.getMessageId();
            foreach (Transform npc in emc.npcHolder.transform)
            {
                //Debug.Log(npc.name + " messageSendDistance: " + messageSendDistance + " Distance: " + Vector3.Distance(npc.GetChild(1).position, this.transform.position) + " description: " + description);
                //check if NPC is at a close distance;
                if (Vector3.Distance(npc.GetChild(1).position, this.transform.position) < messageSendDistance)
                {
                    string tagString = "";
                    foreach (Message.Tag t in tags)
                    {
                        tagString += t.name + " " + t.weight + ",";
                    }
                    if (tags.Count > 0)
                    {
                        tagString = tagString.Substring(0, tagString.Length - 1);
                    }
                    Debug.Log("tagString: " + tagString);
                    npc.gameObject.GetComponent<NPCData>().RecieveMessage(new Message(eventId, messageTime, description, tagString));

                    uiManager.messageTrackingID.text = eventId.ToString();
                    npc.gameObject.GetComponent<NPCFeedbackUpdater>().checkMessageFeedback();
                }
                //npc.gameObject.GetComponentInChildren<NPCPatrolMovement>().UpdatePatrolPoints();
            }
            Destroy(this.gameObject);
        }
    }
}
