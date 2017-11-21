using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Social : MonoBehaviour {
    
    public bool isTalking;
    EditorModeController em;
    public float talkDistance;
    public float lookSpeed;
    GameObject talkPartner;
    public Message choosedMessage;
    public bool isReceivingMessage;
    public float remainingMessageTransmissionTime;
    /*
    * listenerLevel value goes from 0 to 1. 
    * 0 -> NPC choses a message NPC_Other doesn't know (sends message)
    * 1 -> NPC choses a message he doesn't know from NPC_Other (receives message)
    * 0.5 -> Equal weight between messages from NPC ot NPC_Other
    */
    [Range(0.0f, 1.0f)]
    public float listenerLevel;

    void Start () {
        isTalking = false;
        em = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        talkDistance = 1.0f;
        lookSpeed = 5.0f;
        isReceivingMessage = false;
        listenerLevel = 0.5f;
    }
	
	void Update () {
        if (em.npcHolder != null && !isTalking)
        {
            foreach (Transform npc in em.npcHolder.transform)
            {
                if (!npc.name.Equals(this.name) && Vector3.Distance(npc.position, this.transform.position) <= talkDistance &&
                    !npc.gameObject.GetComponent<Social>().isTalking)
                {
                    //Randomly choose who starts to talk
                    if (Random.Range(0, 2) == 0)
                    {
                        //Debug.Log(this.name + " initiated conversation");
                        choosedMessage = getBestMessageToTalk(this.GetComponent<NPCData>(), npc.gameObject.GetComponent<NPCData>());
                        npc.gameObject.GetComponent<Social>().choosedMessage = choosedMessage;
                    }
                    else{
                       // Debug.Log(npc.gameObject.name + " initiated conversation");
                        choosedMessage = npc.gameObject.GetComponent<Social>().getBestMessageToTalk(
                            npc.gameObject.GetComponent<NPCData>(), this.GetComponent<NPCData>());
                        npc.gameObject.GetComponent<Social>().choosedMessage = choosedMessage;
                    }

                    if (choosedMessage != null)
                    {
                        talkPartner = npc.gameObject;

                        if (this.GetComponent<NPCData>().messages.Contains(choosedMessage))
                        {
                            isReceivingMessage = false;
                        }
                        else
                        {
                            isReceivingMessage = true;
                        }
                        remainingMessageTransmissionTime = choosedMessage.messageTimeOfLife;
                        npc.gameObject.GetComponent<Social>().remainingMessageTransmissionTime = choosedMessage.messageTimeOfLife;
                        this.isTalking = true;
                        npc.gameObject.GetComponent<Social>().isTalking = true;
                        npc.gameObject.GetComponent<Social>().talkPartner = this.gameObject;
                        
                        if (!isReceivingMessage)
                        {
                            npc.gameObject.GetComponent<Social>().isReceivingMessage = true;
                        }
                        else
                        {
                            npc.gameObject.GetComponent<Social>().isReceivingMessage = false;
                        }
                        this.GetComponent<NPCPatrolMovement>().agent.isStopped = true;
                        npc.GetComponent<NPCPatrolMovement>().agent.isStopped = true;
                    }
                }
            }
        }
        else if (isTalking)
        {
            if (Mathf.Abs(Vector3.Angle(this.transform.forward, this.transform.position - talkPartner.transform.position) - 180.0f) > 1.0f)
            {
                Vector3 targetDir = talkPartner.transform.position - transform.position;
                float step = lookSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                //Debug.DrawRay(transform.position, newDir, Color.green);
                transform.rotation = Quaternion.LookRotation(newDir);
                //Debug.Log("Rotating Towards player!!!");
            }

            //Duration of message being sent
            remainingMessageTransmissionTime -= Time.deltaTime;
            if (remainingMessageTransmissionTime <= 0)
            {
                remainingMessageTransmissionTime = 0;
                this.GetComponent<NPCPatrolMovement>().agent.isStopped = false;
                isTalking = false;
                if (isReceivingMessage && choosedMessage != null)
                {
                    this.GetComponent<NPCData>().messages.Add(choosedMessage);
                    isReceivingMessage = false;
                }
            }
            //Debug.Log("I am talking with " + talkPartner.name);
        }
	}

    Message getBestMessageToTalk(NPCData NPC_A, NPCData NPC_B)
    {
        float mostAtractiveMessageScore = 0;
        Message mostAttractiveMessage = null;

        //When considering messages fromn self, if listenerLevel is 1 this messages will not be considered
        foreach (Message m1 in NPC_A.messages)
        {
            bool doesntHaveMessage = false;
            if (NPC_B.messages.Count == 0 || NPC_B.messages.Find(x => x.id == m1.id) == null)
            {
                doesntHaveMessage = true;
            }

            if (doesntHaveMessage)
            {
                float messageScore = 0;
                foreach (Message.Tag tag in m1.tags)
                {
                    foreach (NPCData.Interest interest in NPC_B.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            messageScore += interest.weight * tag.weight * (1 - listenerLevel);
                        }
                    }
                }

                foreach (Message.Tag tag in m1.tags)
                {
                    foreach (NPCData.Interest interest in NPC_A.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            messageScore += interest.weight * tag.weight * (1 - listenerLevel);
                        }
                    }
                }
                if (messageScore > mostAtractiveMessageScore)
                {
                    mostAtractiveMessageScore = messageScore;
                    mostAttractiveMessage = m1;
                }
            }
        }

        //We want to see if NPC_A prefers to talk about his messages or recieve a message from NPC_B
        foreach (Message m2 in NPC_B.messages)
        {
            bool doesntHaveMessage = false;
            if (NPC_A.messages.Count == 0 || NPC_A.messages.Find(x => x.id == m2.id) == null)
            {
                doesntHaveMessage = true;
            }

            if (doesntHaveMessage)
            {
                float messageScore = 0;
                foreach (Message.Tag tag in m2.tags)
                {
                    foreach (NPCData.Interest interest in NPC_A.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            messageScore += interest.weight * tag.weight * listenerLevel;
                        }
                    }
                }

                foreach (Message.Tag tag in m2.tags)
                {
                    foreach (NPCData.Interest interest in NPC_B.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            messageScore += interest.weight * tag.weight * listenerLevel;
                        }
                    }
                }
                if (messageScore > mostAtractiveMessageScore)
                {
                    mostAtractiveMessageScore = messageScore;
                    mostAttractiveMessage = m2;
                }
            }
        }

        if (mostAtractiveMessageScore * GetFriendshipLevel(NPC_A, NPC_B) > Constants.MINIMUM_SCORE_FOR_MESSAGE)
        {
            Debug.Log("mostAtractiveMessageScore: " + mostAtractiveMessageScore);
            Debug.Log("Friendship Level: " + GetFriendshipLevel(NPC_A, NPC_B));
            Debug.Log("TotalScore: " + mostAtractiveMessageScore * GetFriendshipLevel(NPC_A, NPC_B));
            Debug.Log("mostAttractiveMessage: " + mostAttractiveMessage.ToString());

            return mostAttractiveMessage;
        }
        else
        {
            return null;
        }
    }

    int GetFriendshipLevel(NPCData NPC_A, NPCData NPC_B)
    {
        NPCData.Aquaintance foundAquaintanceA = NPC_A.aquaintances.Find(x => x.npcName == NPC_B.gameObject.name);
        NPCData.Aquaintance foundAquaintanceB = NPC_B.aquaintances.Find(x => x.npcName == NPC_A.gameObject.name);

        //only if both NPCs are aquaintances of each other is the friendship level positive
        if (foundAquaintanceA != null && foundAquaintanceB != null)
        {
            return foundAquaintanceA.friendshipLevel * foundAquaintanceB.friendshipLevel;
        }
        //returns 0 if NPC_A is not an aquaintance of NPC_B and vice versa
        return 0;
    }
}