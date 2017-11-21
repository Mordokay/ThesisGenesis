using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message
{
    public int id;
    public float messageTimeOfLife;
    public List<Tag> tags;
    public string description;

    [System.Serializable]
    public class Tag
    {
        public string name;
        public int weight;

        public Tag(string name, int weight)
        {
            this.name = name;
            this.weight = weight;
        }
    };

    public override string ToString() {

        string msg = "";
        msg += "ID: " + id + " msgTime: " + messageTimeOfLife + " { ";
        foreach(Tag t in tags)
        {
            msg += t.name + " " + t.weight + ",";
        }
        if(tags.Count > 0)
        {
            msg.Substring(0, msg.Length - 1);
        }
        msg += "} Description: " + description;
        return msg;
    }

    public Message(int id, float messageTimeOfLife, string description, string tagsText)
    {
        //Debug.Log("id " + id + " messageTimeOfLife " + messageTimeOfLife + " description " + description + " tagsText " + tagsText);
        this.id = id;
        this.messageTimeOfLife = messageTimeOfLife;

        this.description = description;

        tags = new List<Tag>();
        string[] tagsList = tagsText.Split(',');
        foreach (string t in tagsList)
        {
            string[] tagData = t.Split(' ');
            tags.Add(new Tag(tagData[0], System.Int32.Parse(tagData[1])));
        }
    }
}