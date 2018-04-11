using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulationDataLogger : MonoBehaviour {

    bool isWritingStuff;
    StreamWriter writer;
    StreamWriter writerLocal;

    int[] messageCounter;
    public int[] aliveIDs;
    int[] existsIDs;
    int[] removedCount;

    int[] graphPointsMessagePrevious;
    int[] graphPointsMessageCurrent;

    int[] graphPointsAlivePrevious;
    int[] graphPointsExistsPrevious;

    int repeatedMessageCount;

    string removedMessagesInfo;
    int removedTotalMessages;
    public Color[] GraphColors;
    public GameObject GraphPoint;
    public GameObject GraphLine;
    public GameObject GraphHolderMessage;
    public GameObject GraphHolderAlive;
    public GameObject GraphHolderExists;

    int currentMinute;

    public int resWidth = 5760;
    public int resHeight = 3240;
    string nameForSave;
    public Camera graphCameraMessage;
    public Camera graphCameraAlive;
    public Camera graphCameraExists;
    public GameObject[] graphLabelsMessage;
    public GameObject[] graphLabelsAlive;
    public GameObject[] graphLabelsExists;

    void Start () {
        nameForSave = "";
        currentMinute = -1;

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
        graphPointsMessagePrevious = new int[10];
        foreach (int i in graphPointsMessagePrevious)
        {
            graphPointsMessagePrevious[i] = 0;
        }
        graphPointsMessageCurrent = new int[10];
        foreach (int i in graphPointsMessageCurrent)
        {
            graphPointsMessageCurrent[i] = 0;
        }

        graphPointsAlivePrevious = new int[10];
        foreach (int i in graphPointsAlivePrevious)
        {
            graphPointsAlivePrevious[i] = 0;
        }
        graphPointsExistsPrevious = new int[10];
        foreach (int i in graphPointsExistsPrevious)
        {
            graphPointsExistsPrevious[i] = 0;
        }

        if (!PlayerPrefs.GetString("mapToLoad").Equals("default")){
            isWritingStuff = true;

            string cenarioName = PlayerPrefs.GetString("mapToLoad");
            string timestamp = System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Year + "___" +
                System.DateTime.Now.TimeOfDay.Hours + "-" + System.DateTime.Now.TimeOfDay.Minutes + "-" + System.DateTime.Now.TimeOfDay.Seconds;
            nameForSave = cenarioName + "___" + timestamp;

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
            if (id < 10)
            {
                graphPointsMessageCurrent[id] += 1;
            }

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

    public void AddPoints(int time)
    {
        foreach (Transform npc in this.GetComponent<EditorModeController>().npcHolder.transform)
        {
            foreach (Message m in npc.gameObject.GetComponent<NPCData>().messages)
            {
                if (m.messageDecayment > 0.0f)
                {
                    aliveIDs[m.id] += 1;
                }
                existsIDs[m.id] += 1;
            }
        }

        for (int i = 0; i < this.GetComponent<PlayModeManager>().messageID && i < 10; i++)
        {
            graphLabelsMessage[i].SetActive(true);
            graphLabelsAlive[i].SetActive(true);
            graphLabelsExists[i].SetActive(true);

            //DrawCurrentPoint and set color for Messages
            Debug.Log("Instantiating point message");
            GameObject myPoint = Instantiate(GraphPoint) as GameObject;
            myPoint.GetComponent<SpriteRenderer>().color = GraphColors[i];
            myPoint.transform.position = new Vector3(500.0f + time, 0.0f, 500.0f + graphPointsMessageCurrent[i]);
            myPoint.transform.parent = GraphHolderMessage.transform;

            //DrawCurrentPoint and set color for Alive
            myPoint = Instantiate(GraphPoint) as GameObject;
            myPoint.GetComponent<SpriteRenderer>().color = GraphColors[i];
            myPoint.transform.position = new Vector3(500.0f + time, 0.0f, aliveIDs[i]);
            myPoint.transform.parent = GraphHolderAlive.transform;

            //DrawCurrentPoint and set color for Exists
            myPoint = Instantiate(GraphPoint) as GameObject;
            myPoint.GetComponent<SpriteRenderer>().color = GraphColors[i];
            myPoint.transform.position = new Vector3(500.0f + time, 0.0f, -500.0f + existsIDs[i]);
            myPoint.transform.parent = GraphHolderExists.transform;

            //Draw a Line to previous point and color of line in Messages
            GameObject myLine = Instantiate(GraphLine);
            myLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(500.0f + time - 1, 0.0f, 500.0f + graphPointsMessagePrevious[i]));
            myLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(500.0f + time, 0.0f, 500.0f + graphPointsMessageCurrent[i]));
            myLine.GetComponent<LineRenderer>().startColor = GraphColors[i];
            myLine.GetComponent<LineRenderer>().endColor = GraphColors[i];
            myLine.transform.parent = GraphHolderMessage.transform;

            //Draw a Line to previous point and color of line in Alive
            myLine = Instantiate(GraphLine);
            myLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(500.0f + time - 1, 0.0f, graphPointsAlivePrevious[i]));
            myLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(500.0f + time, 0.0f, aliveIDs[i]));
            myLine.GetComponent<LineRenderer>().startColor = GraphColors[i];
            myLine.GetComponent<LineRenderer>().endColor = GraphColors[i];
            myLine.transform.parent = GraphHolderAlive.transform;

            //Draw a Line to previous point and color of line in Exists
            myLine = Instantiate(GraphLine);
            myLine.GetComponent<LineRenderer>().SetPosition(0, new Vector3(500.0f + time - 1, 0.0f, -500.0f + graphPointsExistsPrevious[i]));
            myLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(500.0f + time, 0.0f, -500.0f + existsIDs[i]));
            myLine.GetComponent<LineRenderer>().startColor = GraphColors[i];
            myLine.GetComponent<LineRenderer>().endColor = GraphColors[i];
            myLine.transform.parent = GraphHolderExists.transform;

            graphPointsMessagePrevious[i] = graphPointsMessageCurrent[i];
            graphPointsMessageCurrent[i] = 0;

            graphPointsAlivePrevious[i] = aliveIDs[i];
            aliveIDs[i] = 0;

            graphPointsExistsPrevious[i] = existsIDs[i];
            existsIDs[i] = 0;
        }

        currentMinute = Mathf.FloorToInt(Time.timeSinceLevelLoad / 60);
    }

    public void CloseLogger()
    {
        if (isWritingStuff)
        {
            ///////////////////////////////////////////////////////////////////////
            /////////////////////////PNG GRAPH LOGGER//////////////////////////////
            ///////////////////////////////////////////////////////////////////////

            nameForSave += "_" + resWidth + "x" + resHeight;
            string pathMessage = Application.persistentDataPath + "/CenarioTests/" + nameForSave + "_Message.png";
            string localPathMessage = "Assets/CenarioTests/" + nameForSave + "_Message.png";

            string pathAlive = Application.persistentDataPath + "/CenarioTests/" + nameForSave + "_Alive.png";
            string localPathAlive = "Assets/CenarioTests/" + nameForSave + "_Alive.png";

            string pathExists = Application.persistentDataPath + "/CenarioTests/" + nameForSave + "_Exists.png";
            string localPathExists = "Assets/CenarioTests/" + nameForSave + "_Exists.png";

            graphCameraMessage.gameObject.SetActive(true);
            graphCameraAlive.gameObject.SetActive(true);
            graphCameraExists.gameObject.SetActive(true);

            //MESSAGE GRAPH
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            graphCameraMessage.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            graphCameraMessage.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            graphCameraMessage.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();

            System.IO.File.WriteAllBytes(pathMessage, bytes);
            System.IO.File.WriteAllBytes(localPathMessage, bytes);

            //ALIVE GRAPH
            rt = new RenderTexture(resWidth, resHeight, 24);
            graphCameraAlive.targetTexture = rt;
            screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            graphCameraAlive.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            graphCameraAlive.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            bytes = screenShot.EncodeToPNG();

            System.IO.File.WriteAllBytes(pathAlive, bytes);
            System.IO.File.WriteAllBytes(localPathAlive, bytes);

            //EXISTS GRAPH
            rt = new RenderTexture(resWidth, resHeight, 24);
            graphCameraExists.targetTexture = rt;
            screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            graphCameraExists.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            graphCameraExists.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            bytes = screenShot.EncodeToPNG();

            System.IO.File.WriteAllBytes(pathExists, bytes);
            System.IO.File.WriteAllBytes(localPathExists, bytes);

            ///////////////////////////////////////////////////////////////////////
            /////////////////////////TEXT FILE LOGGER//////////////////////////////
            ///////////////////////////////////////////////////////////////////////
            int messagesTotalCount = 0;

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
                foreach (Message m in myNPCs[i].GetComponent<NPCData>().messages)
                {
                    if (m.messageDecayment > 0.0f)
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

    private void Update()
    {
        if (Mathf.FloorToInt(Time.timeSinceLevelLoad / 60) != currentMinute)
        {
            //Debug.Log("AddingPoints at minute: " + Mathf.FloorToInt(Time.timeSinceLevelLoad / 60));
            AddPoints(Mathf.FloorToInt(Time.timeSinceLevelLoad / 60));
        }
    }
}
