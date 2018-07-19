using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementController : MonoBehaviour {

    public float health;
    EditorModeController emc;
    PlayModeManager pm;
    public float messageSendDistance;

    public float messageTime;
    public List<Message.Tag> tags;
    public string description;

    public GameObject slider;
    float maxHealth;
    UIManager uiManager;
    QuestsController qc;

    public int watchedEventId;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        maxHealth = health;
        emc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayModeManager>();
        qc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<QuestsController>();

    }

    public void BeaconPulse(bool isGolden)
    {
        foreach (Transform npc in emc.npcHolder.transform)
        {
            //check if NPC is at a close distance;
            if (Vector3.Distance(npc.GetChild(1).position, this.transform.position) < messageSendDistance)
            {
                string tagString = "";
                foreach (Message.Tag t in tags)
                {
                    //AutoMessages always have minimum weight=1
                    tagString += t.name + " " + 1 + ",";
                }
                if (tags.Count > 0)
                {
                    tagString = tagString.Substring(0, tagString.Length - 1);
                }

                if (isGolden)
                {
                    npc.gameObject.GetComponent<NPCData>().ReceiveMessage(new Message(-99, messageTime, "Auto Golden Message", tagString));
                    //Debug.Log("Name: " + npc.gameObject.name + " golden");
                }
                else
                {
                    npc.gameObject.GetComponent<NPCData>().ReceiveMessage(new Message(-99, messageTime, "Auto Message", tagString));
                    //Debug.Log("Name: " + npc.gameObject.name + " NOT golden");
                }

                if (watchedEventId != 99)
                {
                    npc.gameObject.GetComponent<NPCData>().ActivateWatchedEvent(watchedEventId);
                }

                
            }
        }
    }

    void Update () {
		if(health <= 0)
        {
            if (this.name.Contains("Golden"))
            {
                if (this.name.Contains("Tree"))
                {
                    Debug.Log("Updates Wood quest!!!!");
                    qc.IncrementQuestbar(0);
                }
                else if (this.name.Contains("Rock"))
                {
                    Debug.Log("Updates Rock quest!!!!");
                    qc.IncrementQuestbar(1);
                }
                else if (this.name.Contains("Berries"))
                {
                    Debug.Log("Updates Berries quest!!!!");
                    qc.IncrementQuestbar(2);
                }
                else if (this.name.Contains("Cactus"))
                {
                    Debug.Log("Updates Cactus quest!!!!");
                    qc.IncrementQuestbar(3);
                }
            }
            int eventId = pm.getMessageId();
            foreach (Transform npc in emc.npcHolder.transform)
            {
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
                    npc.gameObject.GetComponent<NPCData>().ReceiveMessage(new Message(eventId, messageTime, description, tagString));
                    if (watchedEventId != 99)
                    {
                        npc.gameObject.GetComponent<NPCData>().ActivateWatchedEvent(watchedEventId);
                    }

                    uiManager.messageTrackingID.text = eventId.ToString();
                    npc.gameObject.GetComponent<NPCFeedbackUpdater>().checkMessageFeedback();
                }
                npc.gameObject.GetComponentInChildren<NPCPatrolMovement>().UpdatePatrolPoints();
            }
            /*
            foreach (Transform patrolPoint in emc.patrolPointsHolder.transform)
            {
                //check if Patrol Points are at a close distance;
                if (Vector3.Distance(patrolPoint.position, this.transform.position) < messageSendDistance)
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
                    patrolPoint.gameObject.GetComponent<PatrolPointData>().ReceiveEvent(new Message(eventId, messageTime, description, tagString));
                }
            }
            */
            emc.RemoveElement(this.gameObject);
        }
	}

    public void Attack(float attackDamage)
    {
        health -= attackDamage;
        GameObject myDamageText = Instantiate(Resources.Load("DamageText")) as GameObject;
        myDamageText.GetComponent<DamageTextController>().Initialize(this.transform.position, 0.5f, 1.5f, attackDamage.ToString());
        
        slider.SetActive(true);
        slider.GetComponent<Slider>().value = health / maxHealth;
        //Debug.Log(health / maxHealth);
    }
}
