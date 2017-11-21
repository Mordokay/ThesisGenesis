using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementController : MonoBehaviour {

    public float health;
    EditorModeController emc;
    PlayModeManager pm;
    public float messageSendDistance;

    public float messageTime;
    public List<Message.Tag> tags;
    public string description;

    private void Start()
    {
        messageSendDistance = 3.0f;
        emc = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        pm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PlayModeManager>();
    }

    void Update () {
		if(health <= 0)
        {
            int eventId = pm.getMessageId();
            foreach (Transform npc in emc.npcHolder.transform)
            {
                //check if NPC is at a close distance;
                if (Vector3.Distance(npc.position, this.transform.position) < messageSendDistance)
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
                    npc.gameObject.GetComponent<NPCData>().messages.Add(new Message(pm.getMessageId(), messageTime, description, tagString));
                }
                npc.gameObject.GetComponent<NPCPatrolMovement>().UpdatePatrolPoints();
            }

            emc.RemoveElement(this.gameObject);
        }
	}

    public void Attack(float attackDamage)
    {
        health -= attackDamage;
        GameObject myDamageText = Instantiate(Resources.Load("DamageText")) as GameObject;
        myDamageText.GetComponent<DamageTextController>().Initialize(this.transform.position, 0.5f, 1.5f, attackDamage.ToString());
    }
}
