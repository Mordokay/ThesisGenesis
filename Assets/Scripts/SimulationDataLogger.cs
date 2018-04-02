using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulationDataLogger : MonoBehaviour {

    bool isWritingStuff;
    StreamWriter writer;
    StreamWriter writerLocal;

    int[] messageCounter;
    int[] aliveIDs;
    int[] existsIDs;
    int[] removedCount;
    int repeatedMessageCount;

    string removedMessagesInfo;
    int removedTotalMessages;

    void Start () {
        removedMessagesInfo = "";
        removedTotalMessages = 0;

        repeatedMessageCount = 0;

        messageCounter = new int[100];
        foreach(int i in messageCounter)
        {
            messageCounter[i] = 0;
        }
        aliveIDs = new int[100];
        foreach (int i in aliveIDs)
        {
            aliveIDs[i] = 0;
        }
        existsIDs = new int[100];
        foreach (int i in existsIDs)
        {
            existsIDs[i] = 0;
        }
        removedCount = new int[100];
        foreach (int i in removedCount)
        {
            removedCount[i] = 0;
        }

        if (!PlayerPrefs.GetString("mapToLoad").Equals("default")){
            isWritingStuff = true;

            string cenarioName = PlayerPrefs.GetString("mapToLoad");
            string timestamp = System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Year + "___" +
                System.DateTime.Now.TimeOfDay.Hours + "-" + System.DateTime.Now.TimeOfDay.Minutes + "-" + System.DateTime.Now.TimeOfDay.Seconds;
            string nameForSave = cenarioName + "___" + timestamp;

            string path = Application.persistentDataPath + "/CenarioTests/" + nameForSave + ".txt";
            string localPath = "Assets/CenarioTests/" + nameForSave + ".txt";

            //Write some text to the test.txt file
            writer = new StreamWriter(path, false);
            writerLocal = new StreamWriter(localPath, false);

            Debug.Log(nameForSave);
        }
        else
        {
            isWritingStuff = false;
        }
    }

    public string getCurrentTime()
    {
        string minutesText = "";
        string secondsText = "";

        if (Time.timeSinceLevelLoad / 60 < 10)
        {
            minutesText = "0" + Mathf.FloorToInt(Time.timeSinceLevelLoad / 60).ToString();
        }
        else
        {
            minutesText = Mathf.FloorToInt(Time.timeSinceLevelLoad / 60).ToString();
        }

        if (Time.timeSinceLevelLoad % 60 < 10)
        {
            secondsText = "0" + Mathf.FloorToInt(Time.timeSinceLevelLoad % 60).ToString();
        }
        else
        {
            secondsText = Mathf.FloorToInt(Time.timeSinceLevelLoad % 60).ToString();
        }

        string SimulationTime = "[" + minutesText + ":" + secondsText + "]  ";

        return SimulationTime;
    }

    public void WriteMessageToLog(string line, int id, bool wasRepeated)
    {
        if (isWritingStuff)
        {
            messageCounter[id] += 1;
            if (wasRepeated)
            {
                repeatedMessageCount += 1;
            }

            string currentTime = getCurrentTime();

            writer.WriteLine(currentTime + line);
            writerLocal.WriteLine(currentTime + line);
        }
    }

    public void WriteTextToLog(string line)
    {
        if (isWritingStuff)
        {
            string currentTime = getCurrentTime();

            writer.WriteLine(currentTime + line);
            writerLocal.WriteLine(currentTime + line);
        }
    }
    public void WriteRemoveToLog(string line, int id)
    {
        if (isWritingStuff)
        {
            removedTotalMessages += 1;
            removedCount[id] += 1;

            removedMessagesInfo += getCurrentTime() + " " + line + System.Environment.NewLine;

        }
    }

    public void CloseLogger()
    {
        int messagesTotalCount = 0;

        if (isWritingStuff)
        {
            writer.WriteLine(System.Environment.NewLine);
            writerLocal.WriteLine(System.Environment.NewLine);

            writer.WriteLine(removedMessagesInfo);
            writerLocal.WriteLine(removedMessagesInfo);

            for (int i = 0; i < this.GetComponent<PlayModeManager>().messageID; i++)
            {
                writer.WriteLine("Message ID = " + i + " was removed " + removedCount[i] + " times");
                writerLocal.WriteLine("Message ID = " + i + " was removed " + removedCount[i] + " times");
            }

            writer.WriteLine("Removed " + removedTotalMessages + " total messages");
            writerLocal.WriteLine("Removed " + removedTotalMessages + " total messages");

            writer.WriteLine(System.Environment.NewLine);
            writerLocal.WriteLine(System.Environment.NewLine);

            for (int i = 0; i < this.GetComponent<PlayModeManager>().messageID; i++)
            {
                writer.WriteLine("Message ID = " + i + " Count = " + messageCounter[i]);
                writerLocal.WriteLine("Message ID = " + i + " Count = " + messageCounter[i]);
                messagesTotalCount += messageCounter[i];
            }

            writer.WriteLine(System.Environment.NewLine);
            writerLocal.WriteLine(System.Environment.NewLine);

            writer.WriteLine("Total Messages = " + messagesTotalCount + System.Environment.NewLine);
            writerLocal.WriteLine("Total Messages = " + messagesTotalCount + System.Environment.NewLine);

            writer.WriteLine("Repeated Messages = " + repeatedMessageCount + System.Environment.NewLine);
            writerLocal.WriteLine("Repeated Messages = " + repeatedMessageCount + System.Environment.NewLine);

            writer.WriteLine("New Messages = " + (messagesTotalCount - repeatedMessageCount) + System.Environment.NewLine);
            writerLocal.WriteLine("New Messages = " + (messagesTotalCount - repeatedMessageCount) + System.Environment.NewLine);

            writer.WriteLine("Total Simulation Time: " + getCurrentTime() + System.Environment.NewLine);
            writerLocal.WriteLine("Total Simulation Time: " + getCurrentTime() + System.Environment.NewLine);

            writer.WriteLine(System.Environment.NewLine);
            writerLocal.WriteLine(System.Environment.NewLine);

            List<GameObject> myNPCs = new List<GameObject>();
            foreach (Transform npc in this.GetComponent<EditorModeController>().npcHolder.transform)
            {
                myNPCs.Add(npc.gameObject);
            }

            for (int i = 0; i < myNPCs.Count; i++)
            {
                foreach(Message m in myNPCs[i].GetComponent<NPCData>().messages)
                {
                    if(m.messageDecayment > 0.0f)
                    {
                        aliveIDs[m.id] += 1;
                    }

                    existsIDs[m.id] += 1;
                }
            }

            writer.WriteLine("There are  " + myNPCs.Count + " NPCs");
            writerLocal.WriteLine("There are  " + myNPCs.Count + " NPCs");

            for (int i = 0; i < this.GetComponent<PlayModeManager>().messageID; i++)
            {
                writer.WriteLine("Message With ID = " + i + " Is alive in " + aliveIDs[i] + " NPCs and is present in " + existsIDs[i] + " NPCs");
                writerLocal.WriteLine("Message With ID = " + i + " Is alive in = " + aliveIDs[i] + " NPCs and is present in " + existsIDs[i] + " NPCs");
                messagesTotalCount += messageCounter[i];
            }

            writer.Close();
            writerLocal.Close();
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("mapToLoad", "default");
        CloseLogger();
    }

}
