﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCData : MonoBehaviour {

    public string npcName;
    public List<Interest> interests;
    public List<Aquaintance> aquaintances;
    public List<Message> messages;
    public List<string> patrolPointIndex;

    public SpriteRenderer Body;
    public SpriteRenderer Head;
    public SpriteRenderer LeftHand;
    public SpriteRenderer RightHand;

    [System.Serializable]
    public class Interest
    {
        public string name;
        public int weight;

        public Interest(string name, int weight)
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

    public void InitializeNPCData(string npcName, string thisInterests, 
        string aquaintancesText, string messagesText, string patrolPointIndexText,
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
        messages = new List<Message>();

        if (thisInterests != "" && thisInterests != " ")
        {
            string[] interestsList = thisInterests.Split(',');
            foreach (string i in interestsList)
            {
                string[] interestData = i.Split(' ');
                interests.Add(new Interest(interestData[0], System.Int32.Parse(interestData[1])));
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
