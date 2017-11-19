using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData : MonoBehaviour {

    public string npcName;
    public List<Interest> interests;
    public List<Aquaintance> aquaintances;
    public List<Message> messages;

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

    public NPCData(string npcName, string thisInterests, string aquaintancesText)
    {
        this.npcName = npcName;
        messages = new List<Message>();

        string[] interestsList = thisInterests.Split(',');
        foreach(string i in interestsList)
        {
            string[] interestData = i.Split(' ');
            interests.Add(new Interest(interestData[0], System.Int32.Parse(interestData[1])));
        }

        string[] aquaintancesList = aquaintancesText.Split(',');
        foreach (string j in aquaintancesList)
        {
            string[] aquaintanceData = j.Split(' ');
            aquaintances.Add(new Aquaintance(aquaintanceData[0], System.Int32.Parse(aquaintanceData[1])));
        }
    }
}
