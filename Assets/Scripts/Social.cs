using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Social : MonoBehaviour {
    
    public bool isTalking;
    EditorModeController em;
    public float talkDistance;
    public float lookSpeed;
    public GameObject talkPartner;
    public Message choosedMessage;
    public bool isReceivingMessage;
    public float remainingMessageTransmissionTime;

    bool choosedMessageRepeated;

    public GameObject talkCanvas;

    /*
    * listenerLevel value goes from 0 to 1. 
    * 0 -> NPC choses a message NPC_Other doesn't know (sends message)
    * 1 -> NPC choses a message he doesn't know from NPC_Other (receives message)
    * 0.5 -> Equal weight between messages from NPC ot NPC_Other
    */
     
    void Start () {
        isTalking = false;
        em = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        talkDistance = 2.0f;
        lookSpeed = 5.0f;
        isReceivingMessage = false;
    }
	
	void Update () {
        if (em.npcHolder != null && !isTalking)
        {
            talkCanvas.SetActive(false);
            foreach (Transform npc in em.npcHolder.transform)
            {
                //Debug.Log("npcName: " + npc.name + " this.name " + this.name);
                //Debug.Log("distance: " + Vector3.Distance(npc.position, this.transform.GetChild(1).transform.position));
                if (!npc.name.Equals(this.name) && 
                    Vector3.Distance(npc.transform.GetChild(1).transform.position, this.transform.GetChild(1).transform.position) <= talkDistance &&
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
                        //Debug.Log(npc.gameObject.name + " initiated conversation");
                        choosedMessage = npc.gameObject.GetComponent<Social>().getBestMessageToTalk(
                            npc.gameObject.GetComponent<NPCData>(), this.GetComponent<NPCData>());
                        npc.gameObject.GetComponent<Social>().choosedMessage = choosedMessage;
                    }

                    bool messageOfInterest = false;
                    //If there is a message and npc is recieving the message 
                    //and message is of interest for him (this happens when NPC did not exceed limit of messages and 
                    //this message is more interesting then at least one of the messages he is holding)
                    if (choosedMessage != null && !this.GetComponent<NPCData>().messages.Contains(choosedMessage) &&
                        this.GetComponent<NPCData>().isMessageOfInterest(choosedMessage))
                    {
                        messageOfInterest = true;
                    }
                    //Debug.Log("messageOfInterest: " + messageOfInterest.ToString());
                    if (choosedMessage != null && messageOfInterest)
                    {
                        //Debug.Log("Start talking!!!");
                        talkPartner = npc.gameObject;

                        if (this.GetComponent<NPCData>().messages.Contains(choosedMessage))
                        {
                            isReceivingMessage = false;
                        }
                        else
                        {
                            isReceivingMessage = true;
                        }
                        remainingMessageTransmissionTime = choosedMessage.messageTransmissionTime;
                        npc.gameObject.GetComponent<Social>().remainingMessageTransmissionTime = choosedMessage.messageTransmissionTime;
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
                        this.GetComponentInChildren<NPCPatrolMovement>().agent.isStopped = true;
                        npc.GetComponentInChildren<NPCPatrolMovement>().agent.isStopped = true;
                    }
                }
            }
        }
        else if (isTalking)
        {

            //Decreases cooperativenessLevel and assertivenessLevel by 10 every second NPCs are talking
            this.GetComponent<NPCData>().currentAssertivenessLevel -= Time.deltaTime / 10.0f;
            this.GetComponent<NPCData>().currentCooperativenessLevel -= Time.deltaTime / 10.0f;

            if (!isReceivingMessage && !talkCanvas.activeSelf)
            {
                talkCanvas.SetActive(true);
                talkCanvas.transform.localPosition = transform.GetChild(1).transform.localPosition;
                talkCanvas.GetComponentInChildren<Text>().text = choosedMessage.description;
            }
            if (talkCanvas.activeSelf)
            {
                talkCanvas.GetComponentInChildren<Slider>().value = 1 - (remainingMessageTransmissionTime / choosedMessage.messageTransmissionTime);
            }
            if (Mathf.Abs(Vector3.Angle(this.transform.GetChild(1).transform.forward, 
                this.transform.GetChild(1).transform.position - talkPartner.transform.GetChild(1).transform.position) - 180.0f) > 1.0f)
            {
                Vector3 targetDir = talkPartner.transform.GetChild(1).transform.position - this.transform.GetChild(1).transform.position;
                float step = lookSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(this.transform.GetChild(1).transform.forward, targetDir, step, 0.0F);
                this.transform.GetChild(1).transform.rotation = Quaternion.LookRotation(newDir);
            }
            
            //Duration of message being sent
            remainingMessageTransmissionTime -= Time.deltaTime;
            if (remainingMessageTransmissionTime <= 0)
            {
                remainingMessageTransmissionTime = 0;
                this.GetComponentInChildren<NPCPatrolMovement>().agent.isStopped = false;
                isTalking = false;
                if (isReceivingMessage && choosedMessage != null)
                {
                    if (this.GetComponent<NPCData>().messages.Find(x => x.id == choosedMessage.id) == null)
                    {
                        this.GetComponent<NPCData>().messages.Add(new Message(choosedMessage.id,
                            choosedMessage.messageTransmissionTime, choosedMessage.description,
                            choosedMessage.tags));
                    }
                    else
                    {
                        this.GetComponent<NPCData>().messages.Find(x => x.id == choosedMessage.id).messageDecayment = 1.0f;
                    }
                    isReceivingMessage = false;

                    //Checks if the message recieved is the message being tracked
                    this.GetComponent<NPCFeedbackUpdater>().checkMessageFeedback();
                }
                else if (choosedMessage != null)
                {
                    choosedMessage.messageDecayment = 1.0f;
                }
            }
        }
	}

    Message getBestMessageToTalk(NPCData NPC_A, NPCData NPC_B)
    {
        //each of this values foes from 0 to 1. Total possible value is AA + CA + AB + CB.
        //Only if this total value is bigger than 1.5f do they talk.
        if (NPC_A.currentAssertivenessLevel * NPC_A.assertiveness +
            NPC_A.currentCooperativenessLevel * NPC_A.cooperativeness +
            NPC_B.currentAssertivenessLevel * NPC_B.assertiveness +
            NPC_B.currentCooperativenessLevel * NPC_B.cooperativeness > 1.5f)
        {
            /*
            Debug.Log(NPC_A.currentAssertivenessLevel * NPC_A.assertiveness +
            NPC_A.currentCooperativenessLevel * NPC_A.cooperativeness +
            NPC_B.currentAssertivenessLevel * NPC_B.assertiveness +
            NPC_B.currentCooperativenessLevel * NPC_B.cooperativeness);
            */
            float mostAtractiveMessageScore = 0;
            Message mostAttractiveMessage = null;

            foreach (Message m1 in NPC_A.messages)
            {
                /*
                bool doesntHaveMessage = false;
                if (NPC_B.messages.Count == 0 || NPC_B.messages.Find(x => x.id == m1.id) == null)
                {
                    doesntHaveMessage = true;
                }

                if (doesntHaveMessage)
                {
                */

                bool repeatedMessage = false;
                if (NPC_B.messages.Find(x => x.id == m1.id) != null)
                {
                    repeatedMessage = true;
                }

                /**
                 * AA = Assertiveness A
                 * CA = Cooperativeness A
                 * AB = Assertiveness B
                 * CB = Cooperativeness B
                 * M1 = Message1
                 * 
                 * TagsM1_IA = Tags that are in M1 that match an Interest of A
                 * 
                 * To determine how good M1 M1 is we use this formula:
                 * MessageScore = TagsM1_IA * AA + TagsM1_IB * CA + TagsM1_IB * AB + TagsM1_IA * CB
                 * or 
                 * MessageScore = TagsM1_IA * (AA, CB) + TagsM1_IB * (CA + AB)
                 */
                float messageScore = 0;

                    foreach (Message.Tag tag in m1.tags)
                    {
                        //TagsM1_IA * (AA, CB)
                        foreach (NPCData.Interest interest in NPC_A.interests)
                        {
                            if (interest.name.Equals(tag.name))
                            {
                                messageScore += interest.weight * tag.weight * (NPC_A.assertiveness * NPC_B.cooperativeness);
                            }
                        }
                        //TagsM1_IB * (CA + AB)
                        //foreach (NPCData.Interest interest in NPC_B.interests)
                        //{
                        //    if (interest.name.Equals(tag.name))
                        //    {
                        //        messageScore += interest.weight * tag.weight * (NPC_A.cooperativeness * NPC_B.assertiveness);
                        //    }
                        //}
                    }

                    messageScore *= (0.5f + m1.messageDecayment);
                if (repeatedMessage)
                {
                    messageScore *= 0.1f;
                }
                if (messageScore > mostAtractiveMessageScore)
                    {
                        mostAtractiveMessageScore = messageScore;
                        mostAttractiveMessage = m1;
                    //Debug.Log("mostAtractiveMessageScore: " + mostAtractiveMessageScore);
                    //Debug.Log("mostAttractiveMessage: " + mostAttractiveMessage.ToString());
                }
                }
            //}

            //We want to see if NPC_A prefers to talk about his messages or recieve a message from NPC_B
            foreach (Message m2 in NPC_B.messages)
            {
                /*
                bool doesntHaveMessage = false;
                if (NPC_A.messages.Count == 0 || NPC_A.messages.Find(x w=> x.id == m2.id) == null)
                {
                    doesntHaveMessage = true;
                }

                if (doesntHaveMessage)
                {
                */
                bool repeatedMessage = false;
                if (NPC_A.messages.Find(x => x.id == m2.id) != null)
                {
                    repeatedMessage = true;
                }
                float messageScore = 0;
                    foreach (Message.Tag tag in m2.tags)
                    {
                        //TagsM2_IA * (AA, CB)
                        //foreach (NPCData.Interest interest in NPC_A.interests)
                        //{
                        //    if (interest.name.Equals(tag.name))
                        //    {
                        //        messageScore += interest.weight * tag.weight * (NPC_A.assertiveness * NPC_B.cooperativeness);
                        //    }
                        //}

                        //TagsM2_IB * (CA + AB)
                        foreach (NPCData.Interest interest in NPC_B.interests)
                        {
                            if (interest.name.Equals(tag.name))
                            {
                                messageScore += interest.weight * tag.weight * (NPC_A.cooperativeness * NPC_B.assertiveness);
                            }
                        }
                    }

                    messageScore *= (0.5f + m2.messageDecayment);
                if (repeatedMessage)
                {
                    messageScore *= 0.1f;
                }
                    if (messageScore > mostAtractiveMessageScore)
                    {
                        mostAtractiveMessageScore = messageScore;
                        mostAttractiveMessage = m2;
                    //Debug.Log("mostAtractiveMessageScore: " + mostAtractiveMessageScore);
                    //Debug.Log("mostAttractiveMessage: " + mostAttractiveMessage.ToString());
                }
                //}
            }
            return mostAttractiveMessage;
        }
        else
        {
            /*
            Debug.Log(NPC_A.currentAssertivenessLevel * NPC_A.assertiveness +
            NPC_A.currentCooperativenessLevel * NPC_A.cooperativeness +
            NPC_B.currentAssertivenessLevel * NPC_B.assertiveness +
            NPC_B.currentCooperativenessLevel * NPC_B.cooperativeness);
            */
            return null;
        }
        /*
        if (//mostAtractiveMessageScore * GetFriendshipLevel(NPC_A, NPC_B) +*
            mostAtractiveMessageScore > Constants.MINIMUM_SCORE_FOR_MESSAGE)
        {
            //Debug.Log("mostAtractiveMessageScore: " + mostAtractiveMessageScore);
            //Debug.Log("Friendship Level: " + GetFriendshipLevel(NPC_A, NPC_B));
            Debug.Log("TotalScore: " + mostAtractiveMessageScore);
            Debug.Log("mostAttractiveMessage: " + mostAttractiveMessage.ToString());

            return mostAttractiveMessage;
        }
        else
        {
            return null;
        }
        */
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