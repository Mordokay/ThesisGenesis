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
    public bool initialized;

    public int eventId;

    private void Start()
    {
        emc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayModeManager>();
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        eventId = pm.getMessageId();
    }

    public void InitializeSpawnEvent(List<Message.Tag> myTags, float distance, float time, string desc)
    {
        this.tags = myTags;
        this.messageSendDistance = distance;
        this.messageTime = time;
        this.description = desc;
    }

    private void Update()
    {
        bool foundNPCClose = false;

        if (!description.Equals(""))
        {
            foreach (Transform npc in emc.npcHolder.transform)
            {
                //Debug.Log(npc.name + " messageSendDistance: " + messageSendDistance + " Distance: " + Vector3.Distance(npc.GetChild(1).position, this.transform.position) + " description: " + description);
                //check if NPC is at a close distance;
                if (Vector3.Distance(npc.GetChild(1).position, this.transform.position) < messageSendDistance)
                {
                    foundNPCClose = true;
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
        }
        if (foundNPCClose)
        {
            Destroy(this.gameObject);
        }
    }
}
