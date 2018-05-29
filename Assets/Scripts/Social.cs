using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Social : MonoBehaviour {
    
    public bool isTalking;
    EditorModeController em;
    SimulationDataLogger sdl;
    public float talkDistance;
    public float lookSpeed;
    public GameObject talkPartner;
    public Message choosedMessage;
    public bool isReceivingMessage;
    public float remainingMessageTransmissionTime;

    public GameObject talkCanvas;

    bool onTalkCooldown;
    float TalkCooldownTime;
    bool messageOfInterest;

    float msgDecaymentAtStart = 0;

    void Start () {
        onTalkCooldown = false;

        isTalking = false;
        em = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EditorModeController>();
        sdl = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SimulationDataLogger>();
        talkDistance = 2.0f;
        lookSpeed = 5.0f;
        isReceivingMessage = false;
    }

    public void NPC_Colision(GameObject otherNPC)
    {
        if (!otherNPC.transform.GetComponent<Social>().isTalking && !otherNPC.transform.GetComponent<Social>().onTalkCooldown
            && !onTalkCooldown)
        {
            choosedMessage = null;
            //Randomly choose who starts to talk
            /*
            if (Random.Range(0, 2) == 0)
            {
                //Debug.Log(this.name + " initiated conversation");
                choosedMessage = getBestMessageToTalk(this.GetComponent<NPCData>(), otherNPC.transform.GetComponent<NPCData>());
                otherNPC.transform.GetComponent<Social>().choosedMessage = choosedMessage;
            }
            else
            {
                //Debug.Log(npc.gameObject.name + " initiated conversation");
                choosedMessage = otherNPC.transform.GetComponent<Social>().getBestMessageToTalk(otherNPC.transform.GetComponent<NPCData>(), this.GetComponent<NPCData>());
                otherNPC.transform.GetComponent<Social>().choosedMessage = choosedMessage;
            }
            */
            choosedMessage = getBestMessageToTalk(this.GetComponent<NPCData>(), otherNPC.GetComponent<NPCData>());
            otherNPC.GetComponent<Social>().choosedMessage = choosedMessage;

            messageOfInterest = false;
            //If there is a message and npc is recieving the message 
            //and message is of interest for him (this happens when NPC did not exceed limit of messages and 
            //this message is more interesting then at least one of the messages he is holding)
            if (choosedMessage != null && !this.GetComponent<NPCData>().messages.Contains(choosedMessage) &&
                this.GetComponent<NPCData>().isMessageOfInterest(choosedMessage))
            {
                messageOfInterest = true;
            }
            
            if (choosedMessage != null && messageOfInterest)
            {
                talkPartner = otherNPC;

                if (this.GetComponent<NPCData>().messages.Contains(choosedMessage))
                {
                    isReceivingMessage = false;
                }
                else
                {
                    isReceivingMessage = true;
                    msgDecaymentAtStart = choosedMessage.messageDecayment;
                }
                remainingMessageTransmissionTime = choosedMessage.messageTransmissionTime;
                otherNPC.GetComponentInParent<Social>().remainingMessageTransmissionTime = choosedMessage.messageTransmissionTime;
                this.isTalking = true;
                otherNPC.GetComponentInParent<Social>().isTalking = true;
                otherNPC.GetComponentInParent<Social>().talkPartner = this.gameObject;

                if (!isReceivingMessage)
                {
                    otherNPC.GetComponentInParent<Social>().isReceivingMessage = true;
                }
                else
                {
                    otherNPC.GetComponentInParent<Social>().isReceivingMessage = false;
                }
                //this.GetComponentInChildren<NPCPatrolMovement>().agent.isStopped = true;
                //otherNPC.transform.GetComponentInParent<NPCPatrolMovement>().agent.isStopped = true;
                this.GetComponentInChildren<NPCPatrolMovement>().stopped = true;
                otherNPC.GetComponentInChildren<NPCPatrolMovement>().stopped = true;
            }
        }
    }

    void Update () {
        if (em.npcHolder != null && !isTalking)
        {
            if (onTalkCooldown)
            {
                //Debug.Log("Talk cooldown!!!");
                TalkCooldownTime -= Time.deltaTime;
                if (TalkCooldownTime <= 0.0f)
                {
                    onTalkCooldown = false;
                }
            }
            /*
            else
            {
                foreach (Transform npc in em.npcHolder.transform)
                {
                    //Debug.Log("npcName: " + npc.name + " this.name " + this.name);
                    //Debug.Log("distance: " + Vector3.Distance(npc.position, this.transform.GetChild(1).transform.position));
                    if (!npc.name.Equals(this.name) &&
                        Vector3.Distance(npc.transform.GetChild(1).transform.position, this.transform.GetChild(1).transform.position) <= talkDistance &&
                        !npc.gameObject.GetComponent<Social>().isTalking)
                    {
                        
                    }
                }
            }
            */
        }
        else if (isTalking)
        {
            if (choosedMessage == null)
            {
                choosedMessage = talkPartner.GetComponent<Social>().choosedMessage;
            }
            else
            {
                //It takes a maximum of 5 seconds for Coop and Ass to reach 0
                if (isReceivingMessage)
                {
                    this.GetComponent<NPCData>().currentCooperativenessLevel -= Time.deltaTime / 5.0f;
                }
                else
                {
                    this.GetComponent<NPCData>().currentAssertivenessLevel -= Time.deltaTime / 5.0f;
                }

                if (/*!isReceivingMessage &&*/ !talkCanvas.activeSelf)
                {
                    talkCanvas.SetActive(true);
                    talkCanvas.transform.localPosition = transform.GetChild(1).transform.localPosition;
                    //talkCanvas.GetComponentInChildren<Text>().text = choosedMessage.description;
                    talkCanvas.GetComponentInChildren<Text>().text = "";
                    for (int i = 3; i <= 10; i++)
                    {
                        talkCanvas.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    switch (choosedMessage.tags[0].name)
                    {
                        case "Wood":
                            if (choosedMessage.description.Contains("Golden"))
                            {
                                talkCanvas.transform.GetChild(7).gameObject.SetActive(true);
                            }
                            else
                            {
                                talkCanvas.transform.GetChild(3).gameObject.SetActive(true);
                            }
                            break;
                        case "Rock":
                            if (choosedMessage.description.Contains("Golden"))
                            {
                                talkCanvas.transform.GetChild(8).gameObject.SetActive(true);
                            }
                            else
                            {
                                talkCanvas.transform.GetChild(4).gameObject.SetActive(true);
                            }
                            break;
                        case "Berries":
                            if (choosedMessage.description.Contains("Golden"))
                            {
                                talkCanvas.transform.GetChild(9).gameObject.SetActive(true);
                            }
                            else
                            {
                                talkCanvas.transform.GetChild(5).gameObject.SetActive(true);
                            }
                            break;
                        case "Cactus":
                            if (choosedMessage.description.Contains("Golden"))
                            {
                                talkCanvas.transform.GetChild(10).gameObject.SetActive(true);
                            }
                            else
                            {
                                talkCanvas.transform.GetChild(6).gameObject.SetActive(true);
                            }
                            break;
                    }
                }
                if (talkCanvas.activeSelf && choosedMessage != null)
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
            }
            //Duration of message being sent
            remainingMessageTransmissionTime -= Time.deltaTime;
            if (remainingMessageTransmissionTime <= 0)
            {
                remainingMessageTransmissionTime = 0;
                this.GetComponentInChildren<NPCPatrolMovement>().stopped = false;
                //this.GetComponentInChildren<NPCPatrolMovement>().agent.isStopped = false;
                isTalking = false;
                onTalkCooldown = true;
                TalkCooldownTime = 2.0f;


                talkCanvas.SetActive(false);

                if (isReceivingMessage && choosedMessage != null)
                {
                    bool wasRepeated = false;

                    if (this.GetComponent<NPCData>().messages.Find(x => x.id == choosedMessage.id) == null)
                    {
                        this.GetComponent<NPCData>().messages.Add(new Message(choosedMessage.id,
                            choosedMessage.messageTransmissionTime, choosedMessage.description,
                            choosedMessage.tags));
                       this.GetComponent<NPCFeedbackUpdater>().checkMessageFeedback();
                    }
                    else
                    {
                        this.GetComponent<NPCData>().messages.Find(x => x.id == choosedMessage.id).messageDecayment = 1.0f;
                        wasRepeated = true;
                    }
                    isReceivingMessage = false;

                    sdl.WriteMessageToLog(talkPartner.GetComponent<NPCData>().name + " >> " + this.GetComponent<NPCData>().name + " || "
                        + "{ " + this.GetComponent<NPCData>().messages.Find(x => x.id == choosedMessage.id).ToString() + " }" +
                        " || Decayment: " + msgDecaymentAtStart, choosedMessage.id, wasRepeated, (talkPartner.transform.GetChild(1).position + this.transform.GetChild(1).position) / 2);
                    msgDecaymentAtStart = 0.0f;

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
        float mostAtractiveMessageScore = 0;
        Message mostAttractiveMessage = null;

        if (NPC_A.currentAssertivenessLevel == 1 &&
            NPC_B.currentCooperativenessLevel == 1)
        {
            foreach (Message m1 in NPC_A.messages)
            {
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

                
                Message messageInB = NPC_B.messages.Find(x => x.id == m1.id);
                float scoreA = 0;
                float scoreB = 0;
                float messageScore = 0;

                foreach (Message.Tag tag in m1.tags)
                {
                    foreach (NPCData.Interest interest in NPC_A.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            scoreA += interest.weight * tag.weight;
                        }
                    }
                }
                scoreA *= m1.messageDecayment;
                
                foreach (Message.Tag tag in m1.tags)
                {
                    foreach (NPCData.Interest interest in NPC_B.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            scoreB += interest.weight * tag.weight;
                        }
                    }
                }
                //if it doesn't havve message decayment = 1 So there is no need to decrease value
                //Repeated messages have 20% of the normal value of a messagethat does not exist
                //Repeated messages that decayed a lot more are of more interest to the NPC since
                //he hasn't talked about it in a long time
                if (messageInB != null)
                {
                    scoreB *= messageInB.messageDecayment;
                }

                //If NPC_B has no interest in the message 
                if (scoreB == 0)
                {
                    messageScore = 0;
                }
                else
                {
                    messageScore = scoreA + scoreB;
                }

                //Debug.Log("Message: " + m1.description + " scoreA: " + scoreA + " scoreB: " + scoreB + " Total: " + messageScore);
                
                if (messageScore > mostAtractiveMessageScore)
                {
                    mostAtractiveMessageScore = messageScore;
                    mostAttractiveMessage = m1;
                }
            }
        }

        if (NPC_B.currentAssertivenessLevel == 1 &&
           NPC_A.currentCooperativenessLevel == 1)
        {
            //We want to see if NPC_A prefers to talk about his messages or recieve a message from NPC_B
            foreach (Message m2 in NPC_B.messages)
            {
                Message messageInA = NPC_A.messages.Find(x => x.id == m2.id);
                float scoreA = 0;
                float scoreB = 0;
                float messageScore = 0;

                foreach (Message.Tag tag in m2.tags)
                {
                    foreach (NPCData.Interest interest in NPC_B.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            scoreB += interest.weight * tag.weight;
                        }
                    }
                }
                scoreB *= m2.messageDecayment;

                foreach (Message.Tag tag in m2.tags)
                {
                    foreach (NPCData.Interest interest in NPC_A.interests)
                    {
                        if (interest.name.Equals(tag.name))
                        {
                            scoreA += interest.weight * tag.weight;
                        }
                    }
                }
                if (messageInA != null)
                {
                    scoreA *= messageInA.messageDecayment;
                }

                //If NPC_A has no interest in the message 
                if (scoreA == 0)
                {
                    messageScore = 0;
                }
                else
                {
                    messageScore = scoreA + scoreB;
                }

                //Debug.Log("Message: " + m2.description + " scoreA: " + scoreA + " scoreB: " + scoreB + " Total: " + messageScore);

                if (messageScore > mostAtractiveMessageScore)
                {
                    mostAtractiveMessageScore = messageScore;
                    mostAttractiveMessage = m2;
                }
            }
        }
        //if (mostAttractiveMessage != null)
        //    Debug.Log("Most Atractive: " + mostAttractiveMessage);
        return mostAttractiveMessage;
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