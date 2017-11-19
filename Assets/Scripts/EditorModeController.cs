using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorModeController : MonoBehaviour {

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
    public GameObject patrolPointSelectedImage;
    public GameObject patrolPointPrefab;

    public List<string> textureNames;

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

    public List<GameObject> selectedTextureFeedbackList;
    public List<GameObject> selectedNaturalFeedbackList;
    public List<GameObject> selectedConstructFeedbackList;

    public GameObject removeTerrainFeedback;
    public GameObject removeElementFeedback;
    public GameObject removePatrolFeedback;

    List<TexturePack> texturePacks;
    public List<Element> elementList;
    public List<Element> patrolPointsList;
    public List<GameObject> npcList;

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
    public bool removeTerrain = false;   
    public bool removePatrolPoint = false;
    public bool removeElement = false;

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

    private void Start()
    {
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
        removeElementFeedback.SetActive(false);
        removePatrolFeedback.SetActive(false);
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
        foreach (GameObject SelectedTexture in selectedTextureFeedbackList)
        {
            SelectedTexture.SetActive(false);
        }
        if (currentTerrainType != -1)
        {
            selectedTextureFeedbackList[currentTerrainType].SetActive(true);
        }
    }

    public void UpdateFeedbackElementSelection()
    {
        foreach (GameObject SelectedTexture in selectedNaturalFeedbackList)
        {
            SelectedTexture.SetActive(false);
        }
        foreach (GameObject SelectedTexture in selectedConstructFeedbackList)
        {
            SelectedTexture.SetActive(false);
        }

        if (patrolPointEnabled)
        {
            patrolPointSelectedImage.SetActive(true);
        }
        else
        {
            patrolPointSelectedImage.SetActive(false);
            
            if (currentElementIdSelected != -1)
            {
                selectedNaturalFeedbackList[currentElementIdSelected].SetActive(true);
            }
            if (currentConstructIdSelected != -1)
            {
                selectedConstructFeedbackList[currentConstructIdSelected].SetActive(true);
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
            npc.gameObject.GetComponent<NPCPatrolMovement>().UpdatePatrolPoints();
        }
        refreshPatrolPointNumber();
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
                npc.gameObject.GetComponent<NPCPatrolMovement>().UpdatePatrolPoints();
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
        
        removeElementFeedback.SetActive(false);
        removePatrolFeedback.SetActive(false);
        removeElement = false;
        removePatrolPoint = false;
        UpdateFeedbackElementSelection();
    }

    public void ToggleRemoveTerrain()
    {
        if (removeTerrain)
        {
            removeTerrain = false;
            removeTerrainFeedback.SetActive(false);
        }
        else
        {
            SetTerrainType(currentTerrainType);
            removeTerrain = true;
            removeTerrainFeedback.SetActive(true);
        }
    }

    public void ToggleRemoveElement()
    {
        if (removeElement)
        {
            isPlacingElements = true;
            removeElement = false;
            removeElementFeedback.SetActive(false);
        }
        else
        {
            isPlacingElements = false;
            currentElementIdSelected = -1;
            currentConstructIdSelected = -1;
            isPlacingElements = false;
            UpdateFeedbackElementSelection();
            patrolPointSelectedImage.SetActive(false);
            removeElement = true;
            removeElementFeedback.SetActive(true);
            removePatrolPoint = false;
            removePatrolFeedback.SetActive(false);
        }
    }

    public void ToggleRemovePatrol()
    {
        if (removePatrolPoint)
        {
            isPlacingElements = true;
            removePatrolPoint = false;
            removePatrolFeedback.SetActive(false);
        }
        else
        {
            isPlacingElements = false;
            currentElementIdSelected = -1;
            currentConstructIdSelected = -1;
            isPlacingElements = false;
            UpdateFeedbackElementSelection();
            patrolPointSelectedImage.SetActive(false);
            removePatrolPoint = true;
            removePatrolFeedback.SetActive(true);
            removeElement = false;
            removeElementFeedback.SetActive(false);
        }
    }

    public void SetTerrainType(int type)
    {
        removeTerrain = false;
        removeTerrainFeedback.SetActive(false);

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
        string path = "Assets/Resources/defaultMap.txt";
        if (nameForSave.text != "")
        {
            path = "Assets/Resources/" + nameForSave.text + ".txt";
        }

        string mapContent = "";
        bool addedLastComma = false;

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
            addedLastComma = true;
        }
        if (addedLastComma)
        {
            addedLastComma = false;
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        for (int i = 0; i < patrolPointsList.Count; i++)
        {
            Vector3 posOfElm = patrolPointsList[i].elementObject.transform.position;
            mapContent += posOfElm.x + " " + posOfElm.z;
            mapContent += ";";
            addedLastComma = true;
        }
        if (addedLastComma)
        {
            addedLastComma = false;
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
        }
        mapContent += "|";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(mapContent);
        writer.Close();

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

        elementList.Clear();
        patrolPointsList.Clear();

        string myFile = "defaultMap";
        if (nameForLoad.text != "")
        {
            myFile = nameForLoad.text;
        }

        TextAsset asset = Resources.Load(myFile) as TextAsset;

        string[] splitGameData = asset.text.Split(char.Parse("|"));
        string[] splitArrayTerrain = splitGameData[0].Split(char.Parse(";"));
        string[] splitArrayUnderground = splitGameData[1].Split(char.Parse(";"));
        string[] splitArrayElements = splitGameData[2].Split(char.Parse(";"));
        string[] splitArrayPatrolPoints = splitGameData[3].Split(char.Parse(";"));

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
            npc.gameObject.GetComponent<NPCPatrolMovement>().UpdatePatrolPoints();
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
                    Debug.Log("Setting terrain type to: " + currentTerrainType + " at position: ( " + realX + " , " + realY + " )");
                    //Debug.Log("mapWidth: " + mapWidth + " mapHeight: " + mapHeight);
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
