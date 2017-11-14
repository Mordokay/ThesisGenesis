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

    public GameObject terrainPlacementObject;

    [System.Serializable]
    public class TexturePack
    {
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
    }

    public List<TexturePack> texturePacks;

    public int currentTerrainType;
    int mapWidth;
    int mapHeight;

    public bool isDrawingTerrain = true;
    public bool isPlacingObject = false;

    [System.Serializable]
    public class Terrain
    {
        public int terrainType;
        public string spriteName;
        public int spriteIndex;
        public GameObject terrainObject;

        public Terrain()
        {
            terrainType = -1;
            spriteName = "";
            spriteIndex = 0;
        }
    }

    public GameObject terrainBasicObject;
    public Terrain[,] tMap;

    private void Start()
    {
        currentTerrainType = -1;
    }
    public void SetTerrainType(int type)
    {
        if (currentTerrainType == type)
        {
            currentTerrainType = -1;
        }
        else
        {
            currentTerrainType = type;
        }
    }

    public void SaveToTxt()
    {
        string path = "Assets/Resources/defaultMap.txt";
        if (nameForSave.text != "")
        {
            path = "Assets/Resources/" + nameForSave.text + ".txt";
        }

        string mapContent = "";

        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                mapContent += tMap[i, j].terrainType + " ";
            }
            mapContent = mapContent.Substring(0, mapContent.Length - 1);
            mapContent += ";";
        }
        mapContent = mapContent.Substring(0, mapContent.Length - 1);

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
        foreach (Transform child in terrainPlacementObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        string myFile = "defaultMap";
        if (nameForLoad.text != "")
        {
            myFile = nameForLoad.text;
        }

        TextAsset asset = Resources.Load(myFile) as TextAsset;

        string[] splitArray = asset.text.Split(char.Parse(";"));
        string[] splitLine = splitArray[0].Split(char.Parse(" "));
        mapWidth = splitArray.Length;
        mapHeight = splitLine.Length;

        tMap = new Terrain[mapWidth, mapHeight];
        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                tMap[i, j] = new Terrain();
            }
        }

        for (int i = 0; i < mapWidth; i++)
        {
            string[] myLine = splitArray[i].Split(char.Parse(" "));

            for (int j = 0; j < mapHeight; j++)
            {
                tMap[i, j].terrainType = System.Int32.Parse(myLine[j]);
                GameObject myTerrain = Instantiate(terrainBasicObject);
                tMap[i, j].terrainObject = myTerrain;
                myTerrain.name = i + " " + j;
                myTerrain.transform.parent = terrainPlacementObject.transform;
                myTerrain.transform.position = new Vector3(i - mapWidth / 2, mapHeight / 2 - j, 0.0f);
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
        Debug.Log("( " + x + " , " + y + " )  -> " + patern);
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

    public void printArrayOfTerrain()
    {
        string s = "";
        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            s += "[";
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                s += tMap[i, j].terrainType;
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
        for (int i = 0; i < tMap.GetLength(0); i++)
        {
            for (int j = 0; j < tMap.GetLength(1); j++)
            {
                tMap[i, j] = new Terrain();
            }
        }

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                GameObject myTerrain = Instantiate(terrainBasicObject);
                tMap[i, j].terrainObject = myTerrain;
                myTerrain.name = i + " " + j;
                myTerrain.transform.parent = terrainPlacementObject.transform;
                myTerrain.transform.position = new Vector3(i - mapWidth / 2, mapHeight / 2 - j, 0.0f);
            }
        }
    }
}