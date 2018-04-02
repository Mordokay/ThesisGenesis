﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCData : MonoBehaviour {

    public string npcName;
    public float assertiveness;
    public float cooperativeness;
    public int NPCType;

    public List<Interest> interests;
    public List<Aquaintance> aquaintances;
    public List<Message> messages;
    public List<string> patrolPointIndex;

    public SpriteRenderer Body;
    public SpriteRenderer Head;
    public SpriteRenderer LeftHand;
    public SpriteRenderer RightHand;

    float messageLimit = 3;

    public float currentAssertivenessLevel;
    public float currentCooperativenessLevel;

    UIManager uiManager;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        currentAssertivenessLevel = 0.0f;
        currentCooperativenessLevel = 0.0f;
    }

    private void FixedUpdate()
    {
        DecayMessages();
    }

    private void Update()
    {
        //It takes 50 seconds to reach full assertiveness and cooperativeness
        if (!this.GetComponent<Social>().isTalking)
        {
            currentAssertivenessLevel += (Time.deltaTime / 30.0f )* assertiveness;
            currentCooperativenessLevel += (Time.deltaTime / 30.0f) * cooperativeness;
        }
        else
        {
            if (this.GetComponent<Social>().isReceivingMessage)
            {
                currentAssertivenessLevel += (Time.deltaTime / 30.0f) * assertiveness;
            }
            else
            {
                currentCooperativenessLevel += (Time.deltaTime / 30.0f) * cooperativeness;
            }
        }

        currentAssertivenessLevel = Mathf.Clamp(currentAssertivenessLevel, 0.0f, 1.0f);
        currentCooperativenessLevel = Mathf.Clamp(currentCooperativenessLevel, 0.0f, 1.0f);
        /*
        if (currentAssertivenessLevel > 1.0f)
        {
            currentAssertivenessLevel = 1.0f;
        }
        else if (currentAssertivenessLevel < 0.0f)
        {
            currentAssertivenessLevel = 0.0f;
        }

        if (currentCooperativenessLevel > 1.0f)
        {
            currentCooperativenessLevel = 1.0f;
        }
        else if (currentCooperativenessLevel < 0.0f)
        {
            currentCooperativenessLevel = 0.0f;
        }
        */
    }

    [System.Serializable]
    public class Interest
    {
        public string name;
        public float weight;

        public Interest(string name, float weight)
        {
            this.name = name;
            this.weight = weight;
        }
    };

    [System.Serializable]
    public class Aquaintance
    {
        public string npcName;
        public int friendshipLevel;

        public Aquaintance(string n, int f)
        {
            npcName = n;
            friendshipLevel = f;
        }
    };

    public void DecayMessages()
    {
        //It takes 3 minutes for a message to reach zero
        foreach (Message m in messages)
        {
            if (uiManager.isLinearDecayment)
            {
                //Linear decayment
                m.messageDecayment -= Time.deltaTime / 180.0f;
                if (m.messageDecayment < 0.0f)
                {
                    //string logMessage = "Removed from " + npcName + " the message " + m.description + " with id " + m.id;
                    //GameObject.FindGameObjectWithTag("GameManager").GetComponent<SimulationDataLogger>().WriteTextToLog(logMessage);

                    //messages.Remove(m);

                    m.messageDecayment = 0.0f;
                }
            }
            else
            {
                //Variable decayment
                m.messageDecayment -= (m.messageDecayment / 8.0f) * Time.deltaTime;

               // m.messageDecayment = m.messageDecayment / (1 + Time.deltaTime);
                if (m.messageDecayment < 0.00001f)
                {
                    //string logMessage = "Removed from " + npcName + " the message " + m.description + " with id " + m.id;
                    //GameObject.FindGameObjectWithTag("GameManager").GetComponent<SimulationDataLogger>().WriteTextToLog(logMessage);

                    //messages.Remove(m);

                    m.messageDecayment = 0.0f;
                }
            }
        }
        /*
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            if(messages[i].messageDecayment < 0.0f)
            {
                messages.RemoveAt(i);
            }
        }
        */
    }

    public bool isMessageOfInterest(Message msg)
    {
        //Debug.Log("messages count: " + messages.Count);
        if (messages.Count < messageLimit)
        {
            //Debug.Log("Add new message");
            return true;
        }
        else
        {
            float recievedMessageScore = 0.0f;
            float lessInterestingMessageScore = Mathf.Infinity;
            Message lessInterestingMessage = null;
            foreach (Message.Tag tag in msg.tags)
            {
                Interest foundInterest = interests.Find(x => x.name == tag.name);
                if (foundInterest != null)
                {
                    recievedMessageScore += foundInterest.weight * tag.weight;
                }
            }

            //recievedMessageScore *= msg.messageDecayment;

            foreach (Message m in messages)
            {
                float totalScore = 0.0f;
                foreach (Message.Tag tag in m.tags)
                {
                    Interest foundInterest = interests.Find(x => x.name == tag.name);
                    if (foundInterest != null)
                    {
                        totalScore += foundInterest.weight * tag.weight;
                    }
                }

                //Total score is multiplied by the decayment of the message
                totalScore *= m.messageDecayment;

                if (totalScore < lessInterestingMessageScore)
                {
                    lessInterestingMessage = m;
                    lessInterestingMessageScore = totalScore;
                }
            }

            //If the message we recieved is more interesting than one of our messages ...
            //We replace the least interesting message with our new recieved message
            if (recievedMessageScore > lessInterestingMessageScore)
            {
                if (messages.Find(x => x.id == msg.id) == null)
                {
                    string logMessage = "Removed from " + npcName + " message ID: " + lessInterestingMessage.id + " <> " + lessInterestingMessage.description;
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<SimulationDataLogger>().WriteRemoveToLog(logMessage, lessInterestingMessage.id);

                    messages.Remove(lessInterestingMessage);
                }
                return true;
            }
        }
        return false;
    }

    public void ReceiveMessage(Message msg)
    {
        if (isMessageOfInterest(msg))
        {
            messages.Add(msg);
        }
    }

    public void InitializeNPCData(string npcName, string thisInterests, 
        string aquaintancesText, string messagesText, string patrolPointIndexText,
        float assertivenessLevel, float cooperativenessLevel, int NPCType,
        Color bodyColor, Color headColor, Color handsColor)
    {
        Body.color = bodyColor;
        Head.color = headColor;
        LeftHand.color = handsColor;
        RightHand.color = handsColor;
        
        //Debug.Log("npcName: " + npcName);
        //Debug.Log("thisInterests:<" + thisInterests + ">");
        //Debug.Log("aquaintancesText:<" + aquaintancesText + ">");
        //Debug.Log("messagesText:<" + messagesText + ">");
        this.npcName = npcName;
        this.NPCType = NPCType;
        cooperativeness = cooperativenessLevel;
        assertiveness = assertivenessLevel;
        messages = new List<Message>();

        if (thisInterests != "" && thisInterests != " ")
        {
            string[] interestsList = thisInterests.Split(',');

            //Normalize interest so the total is equal to 100
            float interestTotalWeight = 0;
            foreach (string i in interestsList)
            {
                string[] interestData = i.Split(' ');
                interestTotalWeight += float.Parse(interestData[1]);
            }

            foreach (string i in interestsList)
            {
                string[] interestData = i.Split(' ');
                //All interests are normalized
                interests.Add(new Interest(interestData[0], float.Parse(interestData[1]) / interestTotalWeight));
            }
        }
        if (aquaintancesText != "" && aquaintancesText != " ")
        {
            string[] aquaintancesList = aquaintancesText.Split(',');
            foreach (string j in aquaintancesList)
            {
                string[] aquaintanceData = j.Split(' ');
                aquaintances.Add(new Aquaintance(aquaintanceData[0], System.Int32.Parse(aquaintanceData[1])));
            }
        }
        if (messagesText != "")
        {
            string[] messagesList = messagesText.Split(';');
            foreach (string m in messagesList)
            {
                //Debug.Log("message: " + m);
                string[] messageInfo = m.Split('&');

                string[] messageBasicData = messageInfo[0].Split(' ');
                int id = System.Int32.Parse(messageBasicData[0]);
                float timeOfLife = float.Parse(messageBasicData[1]);
                string description = messageInfo[1];
                string tagsText = messageInfo[2];

                messages.Add(new Message(id, timeOfLife, description, tagsText));
            }
        }
        if (patrolPointIndexText != "" && patrolPointIndexText != " ")
        {
            string[] patrolPointIndexList = patrolPointIndexText.Split(',');
            foreach (string j in patrolPointIndexList)
            {
                patrolPointIndex.Add(j);
            }
        }
    }
}
