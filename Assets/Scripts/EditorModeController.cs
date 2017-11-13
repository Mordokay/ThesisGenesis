using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EditorModeController : MonoBehaviour {

    public InputField widthOfMap;
    public InputField heightOfMap;
    public InputField nameForSave;
    public InputField nameForLoad;

    public GameObject terrainPlacementObject;

    public int currentTerrainType = 0;
    int mapWidth;
    int mapHeight;

    public bool isDrawingTerrain = true;
    public bool isPlacingObject = false;

    [System.Serializable]
    public class Terrain
    {
        public int terrainType;

        public GameObject topLeft;
        public GameObject top;
        public GameObject topRight;
        public GameObject middleLeft;
        public GameObject middle;
        public GameObject middleRight;
        public GameObject bottonLeft;
        public GameObject botton;
        public GameObject bottonRight;
    }
    
    public List<Terrain> myTerrains = new List<Terrain>();
    public Terrain[,] terrainMapping;

    public void SetTerrainType(int type)
    {
        currentTerrainType = type;
    }

    public void SaveToTxt()
    {
        string path = "Assets/Resources/defaultMap.txt";
        if (nameForSave.text != "")
        {
            path = "Assets/Resources/" + nameForSave.text + ".txt";
        }

        string mapContent = "";

        for (int i = 0; i < terrainMapping.GetLength(0); i++)
        {
            for (int j = 0; j < terrainMapping.GetLength(1); j++)
            {
                mapContent += terrainMapping[i, j].terrainType + " ";
            }
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
            mapContent += ";";
        }
        mapContent = mapContent.Substring(0, mapContent.Length - 1);

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(mapContent);
        writer.Close();
    }

    public void LoadMap()
    {
        string myFile = "defaultMap";
        if (nameForLoad.text != "")
        {
            myFile = nameForLoad.text;
        }

        TextAsset asset = Resources.Load(myFile) as TextAsset;

        //Print the map from the file
        Debug.Log(asset.text);
    }

    public void SetTerrainAtPos(int x, int y)
    {
        int realX = x + mapWidth / 2;
        int realY = mapHeight / 2 - y;
        Debug.Log("Setting terrain type to: " + currentTerrainType + " at position: ( " + realX + " , " + realY + " )");
        if (terrainMapping[realX, realY].terrainType != currentTerrainType)
        {
            Destroy(terrainPlacementObject.transform.Find(realX + " " + realY).gameObject);
            terrainMapping[realX, realY] = myTerrains[currentTerrainType];
            GameObject myTerrain = Instantiate(terrainMapping[realX, realY].middle);
            myTerrain.name = realX + " " + realY;
            myTerrain.transform.parent = terrainPlacementObject.transform;
            myTerrain.transform.position = new Vector3(x, y, 0.0f);
        }
    }

    public void printArrayOfTerrain()
    {
        string s = "";
        for (int i = 0; i < terrainMapping.GetLength(0); i++)
        {
            s += "[";
            for (int j = 0; j < terrainMapping.GetLength(1); j++)
            {
                s += terrainMapping[i, j].terrainType;
            }
            s += "]" + System.Environment.NewLine;
        }
        print(s);
    }

    public void GenerateMap()
    {
        var children = new List<GameObject>();
        foreach (Transform child in terrainPlacementObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        mapWidth = int.Parse(widthOfMap.text);
        mapHeight = int.Parse(heightOfMap.text);

        terrainMapping = new Terrain[mapWidth, mapHeight];
        for (int i = 0; i < terrainMapping.GetLength(0); i++)
        {
            for (int j = 0; j < terrainMapping.GetLength(1); j++)
            {
                terrainMapping[i, j] = new Terrain();
            }
        }

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                terrainMapping[i, j] = myTerrains[0];
                GameObject myTerrain = Instantiate(terrainMapping[i, j].middle);
                myTerrain.name = i + " " + j;
                myTerrain.transform.parent = terrainPlacementObject.transform;
                myTerrain.transform.position = new Vector3(i - mapWidth / 2, mapHeight / 2 - j, 0.0f);
            }
        }
    }
}