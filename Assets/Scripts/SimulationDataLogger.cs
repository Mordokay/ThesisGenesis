using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulationDataLogger : MonoBehaviour {

    bool isWritingStuff;
    StreamWriter writer;
    StreamWriter writerLocal;

    void Start () {
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
	
    public void WriteLine(string line)
    {
        if (isWritingStuff)
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
                secondsText = "0" + Mathf.RoundToInt(Time.timeSinceLevelLoad % 60).ToString();
            }
            else
            {
                secondsText = Mathf.RoundToInt(Time.timeSinceLevelLoad % 60).ToString();
            }

            string SimulationTime = "[" + minutesText + ":" + secondsText + "]  ";

            writer.WriteLine(SimulationTime + line);
            writerLocal.WriteLine(SimulationTime + line);
        }
    }

    public void CloseLogger()
    {
        if (isWritingStuff)
        {
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
