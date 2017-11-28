﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorModeController : MonoBehaviour {

    public UIManager UI_Manager;
    public InputField widthOfMap;
    public InputField heightOfMap;
    public InputField nameForSave;
    public InputField nameForLoad;

    public GameObject terrainHolder;
    public GameObject undergroundHolder;
    public GameObject elementHolder;
    public GameObject patrolPointsHolder;
    public GameObject npcHolder;

    public bool patrolPointEnabled;
    public GameObject patrolPointPrefab;

    public List<string> textureNames;

    public GameObject npcNameInput;
    public GameObject npcInterestHolder;
    public GameObject npcFriendsHolder;
    public GameObject listPatrolPointNPCIndex;

    public Image bodyColorImage;
    public Image headColorImage;
    public Image handsColorImage;

    public Slider assertivenessSlider;
    public Slider cooperativenessSlider;

    class TexturePack
    {
        //"Terrain/WhitePack/0-BasicTerrain"
        public string BasicTerrain;
        public string BarPoint_Horizontal;
        public string BarPoint_Vertical;
        public string Curve;
        public string ExtraParts;
        public string InnerCurves;
        public string ThreePoints;
        public string Tips;
        public string TwoPointsBar;
        public string TwoPoints;
        public string Diagonal;

        public TexturePack(string t1, string t2, string t3, string t4, string t5, string t6, string t7, string t8, string t9, string t10, string t11)
        {
            BasicTerrain = t1;
            BarPoint_Horizontal = t2;
            BarPoint_Vertical = t3;
            Curve = t4;
            ExtraParts = t5;
            InnerCurves = t6;
            ThreePoints = t7;
            Tips = t8;
            TwoPointsBar = t9;
            TwoPoints = t10;
            Diagonal = t11;
        }
    }

    public List<Image> terrainButtonImageList;
    public List<Image> naturalElementButtonImageList;
    public List<Image> constructElementButtonImageList;

    public Image removeTerrainButtonImage;
    public Image removeElementButtonImage;
    public Image removePatrolButtonImage;
    public Image removeNPCButtonImage;
    public Image insertPatrolPointButtonImage;

    List<TexturePack> texturePacks;
    public List<Element> elementList;
    public List<Element> patrolPointsList;

    public int currentTerrainType;
    public int currentElementIdSelected;
    public int currentConstructIdSelected;
    public int mapWidth;
    public int mapHeight;

    public bool isDrawingTerrain = false;
    public bool isPlacingElements = false;
    public bool isPlacingPlayer = false;
    public bool isPlacingNPC = false;
    public bool isEditorMode = true;
    public bool isInspectingElement = false;
    public bool removeTerrain = false;   
    public bool removePatrolPoint = false;
    public bool removeElement = false;
    public bool removeNPC = false;

    [System.Serializable]
    public class Terrain
    {
        public int terrainType;
        public GameObject terrainObject;

        public Terrain()
        {
            terrainType = -1;
        }
    }

    [System.Serializable]
    public class Element
    {
        public int elementID;
        public Vector3 elementPos;
        public GameObject elementObject;
        public string type;

        public Element(GameObject obj, string t, int id)
        {
            elementObject = obj;
            elementID = id;
            type = t;
        }
    }

    public GameObject terrainBasicObject;
    public GameObject undergroundBasicObject;
    public List<GameObject> naturalElementsPrefabs;
    public List<GameObject> constructElementsPrefabs;
    public Terrain[,] tMap;
    public Terrain[,] uMap;

    public List<GameObject> npcTypes;
    public int selectedNPCType;

    private void Start()
    {
        selectedNPCType = 0;
        currentTerrainType = -1;
        currentElementIdSelected = -1;
        currentConstructIdSelected = -1;
        patrolPointEnabled = false;
        Time.timeScale = 0.0f;
        texturePacks = new List<TexturePack>();
        GenerateTerrainReferences();
    }

    void GenerateTerrainReferences()
    {
        foreach (string textureString in textureNames)
        {
            texturePacks.Add(new TexturePack("Terrain/" + textureString + "/0-BasicTerrain",
                "Terrain/" + textureString + "/1-BarPoint_Horizontal",
                "Terrain/" + textureString + "/2-BarPoint_Vertical",
                "Terrain/" + textureString + "/3-Curve",
                "Terrain/" + textureString + "/4-ExtraParts",
                "Terrain/" + textureString + "/5-InnerCurves",
                "Terrain/" + textureString + "/6-ThreePoints",
                "Terrain/" + textureString + "/7-Tips",
                "Terrain/" + textureString + "/8-TwoPointsBar",
                "Terrain/" + textureString + "/9-TwoPoints",
                "Terrain/" + textureString + "/10-Diagonal"));
        }
    }

    public void togglePatrolPoint()
    {
        removeElementButtonImage.color = Color.white;
        removePatrolButtonImage.color = Color.white;
        removeElement = false;
        removePatrolPoint = false;

        if (patrolPointEnabled)
        {
            isPlacingElements = false;
            patrolPointEnabled = false;
        }
        else
        {
            isPlacingElements = true;
            patrolPointEnabled = true;
            currentElementIdSelected = -1;
            currentConstructIdSelected = -1;
        }
        UpdateFeedbackElementSelection();
    }

    public void UpdateFeedbackTerrainSelection()
    {
        foreach (Image img in terrainButtonImageList)
        {
            img.color = Color.white;
        }
        if (currentTerrainType != -1)
        {
            terrainButtonImageList[currentTerrainType].color = Color.yellow;
        }
    }

    public void UpdateFeedbackElementSelection()
    {
        foreach (Image img in naturalElementButtonImageList)
        {
            img.color = Color.white;
        }
        foreach (Image img in constructElementButtonImageList)
        {
            img.color = Color.white;
        }

        if (patrolPointEnabled)
        {
            insertPatrolPointButtonImage.color = Color.yellow;
        }
        else
        {
            insertPatrolPointButtonImage.color = Color.white;

            if (currentElementIdSelected != -1)
            {
                naturalElementButtonImageList[currentElementIdSelected].color = Color.yellow;
            }
            if (currentConstructIdSelected != -1)
            {
                constructElementButtonImageList[currentConstructIdSelected].color = Color.yellow;
            }
        }
    }

    public void RemoveElement(GameObject obj)
    {
        for (int i = elementList.Count-1 ; i >=0; i--)
        {
            if (elementList[i].elementObject.Equals(obj))
            {
                elementList.RemoveAt(i);
                Destroy(obj);
            }
        }
    }

    void refreshPatrolPointNumber()
    {
        for(int i = 0; i < patrolPointsList.Count; i++)
        {
            patrolPointsList[i].elementObject.GetComponentInChildren<TextMesh>().text = i.ToString();
        }
    }

    public void RemovePatrolPoint(GameObject obj)
    {
        for (int i = patrolPointsList.Count - 1; i >= 0; i--)
        {
            if (patrolPointsList[i].elementObject.Equals(obj))
            {
                patrolPointsList.RemoveAt(i);
                Destroy(obj);
            }
        }
        foreach (Transform npc in npcHolder.transform)
        {
            Debug.Log("banana");
            npc.gameObject.GetComponentInChildren<NPCPatrolMovement>().UpdatePatrolPoints();
        }
        refreshPatrolPointNumber();
    }
    public void InsertNPC(Vector3 pos)
    {
        if (npcNameInput.GetComponent<InputField>().textComponent.text != "")
        {
            List<GameObject> NPCs = new List<GameObject>();
            foreach (Transform npc in npcHolder.transform)
            {
                NPCs.Add(npc.gameObject);
            }
            //Only creates a NPC if it has a unique name
            if (NPCs.Find(x => x.name == npcNameInput.GetComponent<InputField>().textComponent.text) == null)
            {
                GameObject myNPC = Instantiate(npcTypes[selectedNPCType]);

                myNPC.transform.parent = npcHolder.transform;
                myNPC.transform.localPosition = new Vector3(pos.x, 0.0f, pos.z);

                myNPC.name = npcNameInput.GetComponent<InputField>().text;

                List<GameObject> Interests = new List<GameObject>();
                foreach (Transform npc in npcInterestHolder.transform)
                {
                    Interests.Add(npc.gameObject);
                }

                List<GameObject> Friends = new List<GameObject>();
                foreach (Transform npc in npcFriendsHolder.transform)
                {
                    Friends.Add(npc.gameObject);
                }

                List<GameObject> PatrolPointNumber = new List<GameObject>();
                foreach (Transform patrol in listPatrolPointNPCIndex.transform)
                {
                    PatrolPointNumber.Add(patrol.gameObject);
                }

                string interestString = "";
                if (Interests.Count > 0)
                {
                    for (int i = 0; i < Interests.Count; i = i + 4)
                    {
                        if (!Interests[i].GetComponentInChildren<Text>().text.Equals("Interest Name"))
                        {
                            interestString += Interests[i].GetComponentInChildren<Text>().text +
                            " " + Interests[i + 2].GetComponent<InputField>().text + ",";
                        }
                    }
                    if (interestString != null)
                    {
                        interestString = interestString.Substring(0, interestString.Length - 1);
                    }
                }
                Debug.Log(interestString);

                string friendsString = "";
                if (Friends.Count > 0)
                {
                    for (int i = 0; i < Friends.Count; i = i + 3)
                    {
                        friendsString += Friends[i].GetComponent<InputField>().text +
                            " " + Friends[i + 1].GetComponent<InputField>().text + ",";
                    }
                    friendsString = friendsString.Substring(0, friendsString.Length - 1);
                }
                Debug.Log(friendsString);

                string patrolPointsString = "";
                if (PatrolPointNumber.Count > 0)
                {
                    for (int i = 0; i < PatrolPointNumber.Count; i++)
                    {
                        patrolPointsString += PatrolPointNumber[i].GetComponent<InputField>().text + ",";
                    }
                    patrolPointsString = patrolPointsString.Substring(0, patrolPointsString.Length - 1);
                }
                Debug.Log(patrolPointsString);

                myNPC.GetComponent<NPCData>().InitializeNPCData(myNPC.name, interestString, friendsString, "",
                    patrolPointsString, assertivenessSlider.value, cooperativenessSlider.value, selectedNPCType,
                    bodyColorImage.color, headColorImage.color, handsColorImage.color);
                if (UI_Manager.isFeedbackEnabled)
                {
                    myNPC.GetComponent<NPCFeedbackUpdater>().feedbackCanvas.SetActive(true);
                }
                myNPC.GetComponentInChildren<NPCPatrolMovement>().setUpPatrolMovementPoints();
            }
        }
    }

    public void InsertElement(Vector3 pos)
    {
        if (patrolPointEnabled)
        {
            GameObject myPatrolPoint = Instantiate(patrolPointPrefab);

            myPatrolPoint.transform.parent = patrolPointsHolder.transform;
            myPatrolPoint.transform.position = new Vector3(pos.x, 0.0f, pos.z);
            myPatrolPoint.GetComponentInChildren<TextMesh>().text = patrolPointsList.Count.ToString();
            patrolPointsList.Add(new Element(myPatrolPoint,  "p", -99));
            foreach (Transform npc in npcHolder.transform)
            {
                npc.gameObject.GetComponentInChildren<NPCPatrolMovement>().UpdatePatrolPoints();
            }
        }
        else
        {
            if (currentElementIdSelected != -1)
            {
                GameObject myElement = Instantiate(naturalElementsPrefabs[currentElementIdSelected]);

                myElement.transform.parent = elementHolder.transform;
                myElement.transform.position = new Vector3(pos.x, 0.0f, pos.z);
                elementList.Add(new Element(myElement, "n", currentElementIdSelected));
            }
            else if (currentConstructIdSelected != -1)
            {
                GameObject myElement = Instantiate(constructElementsPrefabs[currentConstructIdSelected]);

                myElement.transform.parent = elementHolder.transform;
                myElement.transform.position = new Vector3(pos.x, 0.0f, pos.z);
                elementList.Add(new Element(myElement, "c", currentConstructIdSelected));
            }
        }
    }

    public void SetNaturalElementId(int id)
    {
        SetCurrentElementId(id, "n");
    }

    public void SetConstructElementId(int id)
    {
        SetCurrentElementId(id, "c");
    }

    void SetCurrentElementId(int id, string type)
    {
        if (patrolPointEnabled)
        {
            patrolPointEnabled = false;
        }
        switch (type){
            case "c":
                if (currentConstructIdSelected == id)
                {
                    currentConstructIdSelected = -1;
                    isPlacingElements = false;
                }
                else
                {
                    currentConstructIdSelected = id;
                    currentElementIdSelected = -1;
                    isPlacingElements = true;
                }
                break;
            case "n":
                if (currentElementIdSelected == id)
                {
                    currentElementIdSelected = -1;
                    isPlacingElements = false;
                }
                else
                {
                    currentElementIdSelected = id;
                    currentConstructIdSelected = -1;
                    isPlacingElements = true;
                }
                break;
        }

        removeElementButtonImage.color = Color.white;
        removePatrolButtonImage.color = Color.white;
        removeElement = false;
        removePatrolPoint = false;
        UpdateFeedbackElementSelection();
    }

    public void ToggleRemoveTerrain()
    {
        if (removeTerrain)
        {
            removeTerrain = false;
            removeTerrainButtonImage.color = Color.white;
        }
        else
        {
            SetTerrainType(currentTerrainType);
            removeTerrain = true;
            removeTerrainButtonImage.color = Color.yellow;
        }
    }

    public void ToggleRemoveElement()
    {
        if (removeElement)
        {
            isPlacingElements = true;
            removeElement = false;
            removeElementButtonImage.color = Color.white;
        }
        else
        {
            isPlacingElements = false;
            currentElementIdSelected = -1;
            currentConstructIdSelected = -1;
            isPlacingElements = false;
            UpdateFeedbackElementSelection();
            insertPatrolPointButtonImage.color = Color.white;
            removeElement = true;
            removeElementButtonImage.color = Color.yellow;
            removePatrolPoint = false;
            removePatrolButtonImage.color = Color.white;
        }
    }

    public void ToggleRemovePatrol()
    {
        if (removePatrolPoint)
        {
            isPlacingElements = true;
            removePatrolPoint = false;
            removePatrolButtonImage.color = Color.white;
        }
        else
        {
            isPlacingElements = false;
            currentElementIdSelected = -1;
            currentConstructIdSelected = -1;
            isPlacingElements = false;
            UpdateFeedbackElementSelection();
            insertPatrolPointButtonImage.color = Color.white;
            removePatrolPoint = true;
            removePatrolButtonImage.color = Color.yellow;
            removeElement = false;
            removeElementButtonImage.color = Color.white;
        }
    }

    public void ToggleRemoveNPC()
    {
        if (removeNPC)
        {
            isPlacingNPC = true;
            removeNPC = false;

            removeNPCButtonImage.color = Color.white;
        }
        else
        {
            isPlacingNPC = false;
            removeNPC = true;

            removeNPCButtonImage.color = Color.yellow;
        }
    }

    public void ToggleInspectMode()
    {
        if (removeNPC)
        {
            isPlacingNPC = true;
            removeNPC = false;

            removeNPCButtonImage.color = Color.white;
        }
        else
        {
            isPlacingNPC = false;
            removeNPC = true;

            removeNPCButtonImage.color = Color.yellow;
        }
    }
    
    public void SetTerrainType(int type)
    {
        removeTerrain = false;
        removeTerrainButtonImage.color = Color.white;

        if (currentTerrainType == type)
        {
            currentTerrainType = -1;
        }
        else
        {
            currentTerrainType = type;
        }
        this.GetComponent<MouseInputController>().lastTerrainTileClicked = "";
        UpdateFeedbackTerrainSelection();
    }

    public void SaveToTxt()
    {
        Debug.Log(Application.persistentDataPath);
        string path = "";
        string localPath = "";
        if (nameForSave.text == "")
        {
            return;
        }
        else if (nameForSave.text != "")
        {
            path = Application.persistentDataPath + "/" + nameForSave.text + ".txt";
            localPath = "Assets/Resources/" + nameForSave.text + ".txt";
        }

        string mapContent = "";
        bool addedLastComma = false;

        mapContent += this.GetComponent<PlayModeManager>().messageID + "|";

        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                mapContent += tMap[i, j].terrainType + " ";
            }
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
            mapContent += ";";
            addedLastComma = true;
        }
        if (addedLastComma)
        {
            addedLastComma = false;
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        for (int i = 0; i < uMap.GetLength(0); i++)
        {
            for (int j = 0; j < uMap.GetLength(1); j++)
            {
                mapContent += uMap[i, j].terrainType + " ";
            }
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
            mapContent += ";";
            addedLastComma = true;
        }
        if (addedLastComma)
        {
            addedLastComma = false;
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        for (int i = 0; i < elementList.Count; i++)
        {
            Vector3 posOfElm = elementList[i].elementObject.transform.localPosition;
            mapContent += elementList[i].type + " " + elementList[i].elementID + " " + posOfElm.x + " " + posOfElm.z;
            mapContent += ";";
        }
        if (elementList.Count > 0)
        {
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            Vector3 posOfElm = patrolPointsList[i].elementObject.transform.position;
            mapContent += posOfElm.x + " " + posOfElm.z;
            mapContent += ";";
        }
        if (patrolPointsList.Count > 0)
        {
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        List<GameObject> myNPCs = new List<GameObject>();
        foreach (Transform npc in npcHolder.transform)
        {
            myNPCs.Add(npc.gameObject);
        }

        for (int i = 0; i < myNPCs.Count; i++)
        {
            Vector3 npcPos = myNPCs[i].transform.position;
            mapContent += npcPos.x + " " + npcPos.z + ";";

            mapContent += myNPCs[i].GetComponent<NPCData>().npcName + ";";

            mapContent += myNPCs[i].GetComponent<NPCData>().Body.color.r + "," +
                myNPCs[i].GetComponent<NPCData>().Body.color.g + "," +
                myNPCs[i].GetComponent<NPCData>().Body.color.b + ",";
            mapContent += myNPCs[i].GetComponent<NPCData>().Head.color.r + "," +
                myNPCs[i].GetComponent<NPCData>().Head.color.g + "," +
                myNPCs[i].GetComponent<NPCData>().Head.color.b + ",";
            mapContent += myNPCs[i].GetComponent<NPCData>().LeftHand.color.r + "," +
                myNPCs[i].GetComponent<NPCData>().LeftHand.color.g + "," +
                myNPCs[i].GetComponent<NPCData>().LeftHand.color.b + ";";

            foreach (NPCData.Interest interest in myNPCs[i].GetComponent<NPCData>().interests)
            {
                mapContent += interest.name + " " + interest.weight + ",";
            }
            if (myNPCs[i].GetComponent<NPCData>().interests.Count > 0)
            {
                mapContent = mapContent.Substring(0, mapContent.Length - 1);
            }
            mapContent += ";";

            foreach (NPCData.Aquaintance aquaintance in myNPCs[i].GetComponent<NPCData>().aquaintances)
            {
                mapContent += aquaintance.npcName + " " + aquaintance.friendshipLevel + ",";
            }
            if (myNPCs[i].GetComponent<NPCData>().aquaintances.Count > 0)
            {
                mapContent = mapContent.Substring(0, mapContent.Length - 1);
            }
            mapContent += ";";

            foreach (string patrolPointIndex in myNPCs[i].GetComponent<NPCData>().patrolPointIndex)
            {
                mapContent += patrolPointIndex  + ",";
            }
            if (myNPCs[i].GetComponent<NPCData>().patrolPointIndex.Count > 0)
            {
                mapContent = mapContent.Substring(0, mapContent.Length - 1);
            }
            mapContent += ";" + myNPCs[i].GetComponent<NPCData>().assertiveness;
            mapContent += ";" + myNPCs[i].GetComponent<NPCData>().cooperativeness;
            mapContent += ";" + myNPCs[i].GetComponent<NPCData>().NPCType;
            //separates all other stuff from messages
            mapContent += "#";

            foreach (Message message in myNPCs[i].GetComponent<NPCData>().messages)
            {
                mapContent += message.id + " " + message.messageTimeOfLife + "&" + message.description + "&";
                foreach(Message.Tag tag in message.tags)
                {
                    mapContent += tag.name + " " + tag.weight + ",";
                }
                if(message.tags.Count > 0)
                {
                    mapContent = mapContent.Substring(0, mapContent.Length - 1);
                }
                mapContent += ";";
            }
            if (myNPCs[i].GetComponent<NPCData>().messages.Count > 0)
            {
                mapContent = mapContent.Substring(0, mapContent.Length - 1);
            }
            mapContent += "@";
        }
        if (myNPCs.Count > 0)
        {
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        StreamWriter writerLocal = new StreamWriter(localPath, false);
        writer.Write(mapContent);
        writerLocal.Write(mapContent);
        writer.Close();
        writerLocal.Close();

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    public void LoadMap()
    {
        var children = new List<GameObject>();
        foreach (Transform child in terrainHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in undergroundHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in elementHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in patrolPointsHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in npcHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        elementList.Clear();
        patrolPointsList.Clear();

        string myFile = "default";
        if (nameForLoad.text != "")
        {
            myFile = nameForLoad.text;
        }

        //Application.persistentDataPath

        StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + myFile + ".txt");
        string mySaveString = reader.ReadLine();
        //TextAsset asset = Resources.Load(myFile) as TextAsset;

        string[] splitGameData = mySaveString.Split(char.Parse("|"));
        int messageIdCount = System.Int32.Parse(splitGameData[0]);
        string[] splitArrayTerrain = splitGameData[1].Split(char.Parse(";"));
        string[] splitArrayUnderground = splitGameData[2].Split(char.Parse(";"));
        string[] splitArrayElements = splitGameData[3].Split(char.Parse(";"));
        string[] splitArrayPatrolPoints = splitGameData[4].Split(char.Parse(";"));
        string[] splitArrayNPC = splitGameData[5].Split(char.Parse("@"));

        this.GetComponent<PlayModeManager>().messageID = messageIdCount;

        //Debug.Log(splitGameData[0]);
        //Debug.Log(splitGameData[1]);
        //Debug.Log(splitGameData[2]);
        //Debug.Log(splitGameData[3]);

        mapWidth = splitArrayTerrain.Length;
        mapHeight = splitArrayTerrain[0].Split(char.Parse(" ")).Length;

        tMap = new Terrain[mapWidth, mapHeight];
        uMap = new Terrain[mapWidth, mapHeight];
        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                tMap[i, j] = new Terrain();
                uMap[i, j] = new Terrain();
            }
        }

        for (int i = 0; i < mapWidth; i++)
        {
            string[] myLineTerrain = splitArrayTerrain[i].Split(char.Parse(" "));
            string[] myLineUnderground = splitArrayUnderground[i].Split(char.Parse(" "));

            for (int j = 0; j < mapHeight; j++)
            {
                tMap[i, j].terrainType = System.Int32.Parse(myLineTerrain[j]);
                GameObject myTerrain = Instantiate(terrainBasicObject);
                tMap[i, j].terrainObject = myTerrain;
                myTerrain.name = i + " " + j;
                myTerrain.transform.parent = terrainHolder.transform;
                myTerrain.transform.position = new Vector3(i - mapWidth / 2, 0.0f, mapHeight / 2 - j);
                myTerrain.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));

                uMap[i, j].terrainType = System.Int32.Parse(myLineUnderground[j]);
                GameObject myUnderground = Instantiate(undergroundBasicObject);
                uMap[i, j].terrainObject = myUnderground;
                if (uMap[i, j].terrainType != -1)
                {
                    Sprite[] sprites = Resources.LoadAll<Sprite>(texturePacks[uMap[i, j].terrainType].BasicTerrain);
                    uMap[i, j].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[4];

                    
                    if (tMap[i, j].terrainType == -1)
                    {
                        tMap[i, j].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[4];
                    }
                }
                myUnderground.name = i + " " + j;
                myUnderground.transform.parent = undergroundHolder.transform;
                myUnderground.transform.position = new Vector3(i - mapWidth / 2, 0.0f, mapHeight / 2 - j);
                myUnderground.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
            }
        }
        for(int i = 0; i < splitArrayElements.Length; i++)
        {
            string[] myElementData = splitArrayElements[i].Split(char.Parse(" "));

            if (myElementData[0] != "")
            {
                GameObject myElement;
                switch (myElementData[0])
                {
                    case "n":
                        myElement = Instantiate(naturalElementsPrefabs[System.Int32.Parse(myElementData[1])]);

                        myElement.transform.parent = elementHolder.transform;
                        myElement.transform.position = new Vector3(float.Parse(myElementData[2]), 0.0f, float.Parse(myElementData[3]));
                        elementList.Add(new Element(myElement, "n", System.Int32.Parse(myElementData[1])));
                        break;
                    case "c":
                        myElement = Instantiate(constructElementsPrefabs[System.Int32.Parse(myElementData[1])]);

                        myElement.transform.parent = elementHolder.transform;
                        myElement.transform.position = new Vector3(float.Parse(myElementData[2]), 0.0f, float.Parse(myElementData[3]));
                        elementList.Add(new Element(myElement, "c", System.Int32.Parse(myElementData[1])));
                        break;
                }
            }  
        }

        for (int i = 0; i < splitArrayPatrolPoints.Length; i++)
        {
            //Debug.Log(splitArrayPatrolPoints[i]);
            string[] myPatrolData = splitArrayPatrolPoints[i].Split(char.Parse(" "));

            if (myPatrolData[0] != "")
            {
                GameObject myPatrolPoint = Instantiate(patrolPointPrefab);

                myPatrolPoint.transform.parent = patrolPointsHolder.transform;
                myPatrolPoint.transform.position = new Vector3(float.Parse(myPatrolData[0]), 0.0f, float.Parse(myPatrolData[1]));
                myPatrolPoint.GetComponentInChildren<TextMesh>().text = patrolPointsList.Count.ToString();
                patrolPointsList.Add(new Element(myPatrolPoint, "p", -99));
            }
        }
        
        foreach(string s in splitArrayNPC)
        {
            string[] npcData = s.Split('#');

            string[] npcBasicInfo = npcData[0].Split(';');
            string[] npcPos = npcBasicInfo[0].Split(' ');
            string npcName = npcBasicInfo[1];
            string[] myNpcColors = npcBasicInfo[2].Split(',');

            GameObject myNPC = Instantiate(npcTypes[System.Int32.Parse(npcBasicInfo[8])]);

            myNPC.name = npcName;
            myNPC.transform.parent = npcHolder.transform;
            myNPC.transform.localPosition = new Vector3(float.Parse(npcPos[0]), 0.0f, float.Parse(npcPos[1]));


            myNPC.GetComponent<NPCData>().InitializeNPCData(npcName, npcBasicInfo[3], npcBasicInfo[4], npcData[1], 
                npcBasicInfo[5], float.Parse(npcBasicInfo[6]), float.Parse(npcBasicInfo[7]), System.Int32.Parse(npcBasicInfo[8]),
                new Color(float.Parse(myNpcColors[0]), float.Parse(myNpcColors[1]), float.Parse(myNpcColors[2])),
                new Color(float.Parse(myNpcColors[3]), float.Parse(myNpcColors[4]), float.Parse(myNpcColors[5])),
                new Color(float.Parse(myNpcColors[6]), float.Parse(myNpcColors[7]), float.Parse(myNpcColors[8])));
                
            myNPC.GetComponentInChildren<NPCPatrolMovement>().setUpPatrolMovementPoints();
        }
        
        for (int i = 1; i < mapWidth - 1; i++)
        {
            for (int j = 1; j < mapHeight - 1; j++)
            {
                UpdateSprite(i, j);
                //Debug.Log("mapWidth: " + mapWidth + "mapHeight: " + mapHeight);
            }
        }
        foreach (Transform npc in npcHolder.transform)
        {
            npc.gameObject.GetComponentInChildren<NPCPatrolMovement>().UpdatePatrolPoints();
        }
    }
    public void UpdateSprite(int x, int y)
    {
        //Debug.Log("Updating sprite ( " + x + " , " + y + " )");

        string patern = "";
        int myTerrainType = tMap[x, y].terrainType;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if ((x + j < mapWidth && x + j >= 0) && (y + i < mapHeight && y + i >= 0) && tMap[x + j, y + i].terrainType == myTerrainType
                    && myTerrainType != -1)
                {
                    patern += "1";
                }
                else
                {
                    patern += "0";
                }
            }
        }
        Sprite[] sprites;
        switch (patern)
        {
            //Gonna Map BasicTerrain
            case "000011011":
            case "001011011":
            case "100011011":
            case "101011011":
            case "000011111":
            case "001011111":
            case "100011111":
            case "101011111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "011011011":
            case "011011111":
            case "111011011":
            case "111011111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            case "011011000":
            case "011011001":
            case "011011100":
            case "011011101":
            case "111011000":
            case "111011001":
            case "111011100":
            case "111011101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[6];
                break;
            case "000111111":
            case "001111111":
            case "100111111":
            case "101111111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "111111111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[4];
                break;
            case "111111000":
            case "111111001":
            case "111111100":
            case "111111101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[7];
                break;
            case "000110110":
            case "001110110":
            case "100110110":
            case "101110110":
            case "000110111":
            case "001110111":
            case "100110111":
            case "101110111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "110110110":
            case "110110111":
            case "111110110":
            case "111110111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[5];
                break;
            case "110110000":
            case "110110001":
            case "110110100":
            case "110110101":
            case "111110000":
            case "111110001":
            case "111110100":
            case "111110101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BasicTerrain);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[8];
                break;
            //Gonna Map BarPoint_Horizontal
            case "000111110":
            case "001111110":
            case "100111110":
            case "101111110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Horizontal);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "000111011":
            case "001111011":
            case "100111011":
            case "101111011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Horizontal);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "110111000":
            case "110111001":
            case "110111100":
            case "110111101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Horizontal);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "011111000":
            case "011111001":
            case "011111100":
            case "011111101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Horizontal);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map BarPoint_Vertical 
            case "011011010":
            case "011011110":
            case "111011010":
            case "111011110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Vertical);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "110110010":
            case "110110011":
            case "111110010":
            case "111110011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Vertical);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "010011011":
            case "010011111":
            case "110011011":
            case "110011111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Vertical);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "010110110":
            case "010110111":
            case "011110110":
            case "011110111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].BarPoint_Vertical);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map Curve
            case "000011010":
            case "000011110":
            case "001011010":
            case "001011110":
            case "100011010":
            case "100011110":
            case "101011010":
            case "101011110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Curve);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "000110010":
            case "000110011":
            case "001110010":
            case "001110011":
            case "100110010":
            case "100110011":
            case "101110010":
            case "101110011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Curve);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "010011000":
            case "010011001":
            case "010011100":
            case "010011101":
            case "110011000":
            case "110011001":
            case "110011100":
            case "110011101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Curve);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "010110000":
            case "010110001":
            case "010110100":
            case "010110101":
            case "011110000":
            case "011110001":
            case "011110100":
            case "011110101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Curve);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map Extra
            case "000010000":
            case "000010001":
            case "000010100":
            case "000010101":
            case "001010000":
            case "001010001":
            case "001010100":
            case "001010101":
            case "100010000":
            case "100010001":
            case "100010100":
            case "100010101":
            case "101010000":
            case "101010001":
            case "101010100":
            case "101010101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ExtraParts);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "010111010":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ExtraParts);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "000111000":
            case "000111001":
            case "000111100":
            case "000111101":
            case "001111000":
            case "001111001":
            case "001111100":
            case "001111101":
            case "100111000":
            case "100111001":
            case "100111100":
            case "100111101":
            case "101111000":
            case "101111001":
            case "101111100":
            case "101111101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ExtraParts);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "010010010":
            case "010010011":
            case "010010110":
            case "010010111":
            case "011010010":
            case "011010011":
            case "011010110":
            case "011010111":
            case "110010010":
            case "110010011":
            case "110010110":
            case "110010111":
            case "111010010":
            case "111010011":
            case "111010110":
            case "111010111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ExtraParts);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map InnerCurves
            case "111111110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].InnerCurves);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "111111011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].InnerCurves);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "110111111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].InnerCurves);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "011111111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].InnerCurves);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map ThreePoints
            case "010111011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ThreePoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "010111110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ThreePoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "011111010":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ThreePoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "110111010":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].ThreePoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map Tips
            case "000010010":
            case "000010110":
            case "000010011":
            case "000010111":
            case "001010010":
            case "001010110":
            case "001010011":
            case "001010111":
            case "100010010":
            case "100010110":
            case "100010011":
            case "100010111":
            case "101010010":
            case "101010110":
            case "101010011":
            case "101010111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Tips);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "000011000":
            case "000011001":
            case "001011000":
            case "001011001":
            case "000011100":
            case "000011101":
            case "001011100":
            case "001011101":
            case "100011000":
            case "100011001":
            case "101011000":
            case "101011001":
            case "100011100":
            case "100011101":
            case "101011100":
            case "101011101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Tips);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "010010000":
            case "011010000":
            case "110010000":
            case "111010000":
            case "010010001":
            case "011010001":
            case "110010001":
            case "111010001":
            case "010010100":
            case "011010100":
            case "110010100":
            case "111010100":
            case "010010101":
            case "011010101":
            case "110010101":
            case "111010101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Tips);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "000110000":
            case "000110100":
            case "100110000":
            case "100110100":
            case "000110001":
            case "000110101":
            case "100110001":
            case "100110101":
            case "001110000":
            case "001110100":
            case "101110000":
            case "101110100":
            case "001110001":
            case "001110101":
            case "101110001":
            case "101110101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Tips);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map TwoPointsBar
            case "010111000":
            case "010111001":
            case "010111100":
            case "010111101":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPointsBar);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "010110010":
            case "010110011":
            case "011110010":
            case "011110011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPointsBar);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "000111010":
            case "001111010":
            case "100111010":
            case "101111010":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPointsBar);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "010011010":
            case "010011110":
            case "110011010":
            case "110011110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPointsBar);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map TwoPoints
            case "111111010":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "010111111":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case "011111011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case "110111110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].TwoPoints);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
            //Gonna Map Diagonal
            case "011111110":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Diagonal);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case "110111011":
                sprites = Resources.LoadAll<Sprite>(texturePacks[myTerrainType].Diagonal);
                tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            default:
                //tMap[x, y].terrainObject.GetComponent<SpriteRenderer>().sprite = 
                //    terrainBasicObject.GetComponent<SpriteRenderer>().sprite;
                break;
        }
        //Debug.Log("( " + x + " , " + y + " )  -> " + patern);
    }
     
    public void SetUndergroundAtPos(int x, int y)
    {
        if (currentTerrainType != -1)
        {
            int realX = x + mapWidth / 2;
            int realY = mapHeight / 2 - y;
            //Debug.Log("RealX: " + realX + " RealY: " + realY);
            if (realX != 0 && realX != mapWidth - 1 && realY != 0 && realY != mapHeight - 1)
            {
                if (uMap[realX, realY].terrainType != currentTerrainType)
                {
                    uMap[realX, realY].terrainType = currentTerrainType;

                    Sprite[] sprites = Resources.LoadAll<Sprite>(texturePacks[currentTerrainType].BasicTerrain);
                    uMap[realX, realY].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[4];

                    if (tMap[realX, realY].terrainType == -1)
                    {
                        tMap[realX, realY].terrainObject.GetComponent<SpriteRenderer>().sprite = sprites[4];
                    }
                }
            }
        }
    }

    public void removeTerrainAtPos(int x, int y)
    {
        int realX = x + mapWidth / 2;
        int realY = mapHeight / 2 - y;
        if (realX != 0 && realX != mapWidth - 1 && realY != 0 && realY != mapHeight - 1)
        {
            tMap[realX, realY].terrainType = -1;
            tMap[realX, realY].terrainObject.GetComponent<SpriteRenderer>().sprite = uMap[realX, realY].terrainObject.GetComponent<SpriteRenderer>().sprite;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (realX + i > 0 && realX + i < mapWidth - 1 && realY + j > 0 &&
                        realY + j < mapHeight - 1 && tMap[realX + i, realY + j].terrainType != -1)
                    {
                        UpdateSprite(realX + i, realY + j);
                    }
                }
            }
        }
    }

    public void SetTerrainAtPos(int x, int y)
    {
        if (currentTerrainType != -1)
        {
            int realX = x + mapWidth / 2;
            int realY = mapHeight / 2 - y;
            if (realX != 0 && realX != mapWidth - 1 && realY != 0 && realY != mapHeight - 1 &&
                tMap[realX, realY].terrainType != currentTerrainType)
            {
                //Debug.Log("Setting terrain type to: " + currentTerrainType + " at position: ( " + realX + " , " + realY + " )");
                //Debug.Log("mapWidth: " + mapWidth + " mapHeight: " + mapHeight);
                tMap[realX, realY].terrainType = currentTerrainType;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (realX + i > 0 && realX + i < mapWidth - 1 && realY + j > 0 &&
                            realY + j < mapHeight - 1 && tMap[realX + i, realY + j].terrainType != -1)
                        {
                            UpdateSprite(realX + i, realY + j);
                        }
                    }
                }
            }
        }
    }

    public void GenerateMap()
    {
        var children = new List<GameObject>();
        foreach (Transform child in terrainHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in undergroundHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in elementHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        foreach (Transform child in patrolPointsHolder.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        elementList.Clear();
        patrolPointsList.Clear();

        if (widthOfMap.text == "" || heightOfMap.text == "")
        {
            mapWidth = 1;
            mapHeight = 1;
        }
        else {
            mapWidth = int.Parse(widthOfMap.text);
            mapHeight = int.Parse(heightOfMap.text);
        }

        tMap = new Terrain[mapWidth, mapHeight];
        uMap = new Terrain[mapWidth, mapHeight];
        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                tMap[i, j] = new Terrain();
                uMap[i, j] = new Terrain();
            }
        }

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                GameObject myTerrain = Instantiate(terrainBasicObject);
                GameObject myUnderground = Instantiate(undergroundBasicObject);

                tMap[i, j].terrainObject = myTerrain;
                myTerrain.name = i + " " + j;
                myTerrain.transform.parent = terrainHolder.transform;
                myTerrain.transform.position = new Vector3(i - mapWidth / 2, 0.0f, mapHeight / 2 - j);
                myTerrain.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));

                uMap[i, j].terrainObject = myUnderground;
                myUnderground.name = i + " " + j;
                myUnderground.transform.parent = undergroundHolder.transform;
                myUnderground.transform.position = new Vector3(i - mapWidth / 2, 0.0f, mapHeight / 2 - j);
                myUnderground.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
            }
        }
    }
}
