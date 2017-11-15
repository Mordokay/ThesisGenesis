using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject mainPanel;
    //public GameObject loadMapPanel;
    //public GameObject saveMapPanel;
    public GameObject drawTerrainPanel;
    public GameObject addElementPanel;
    public GameObject addNPC_Panel;

    GameObject gm;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager");
    }

    public void ShowTerrainPanel()
    {
        mainPanel.SetActive(false);
        drawTerrainPanel.SetActive(true);
        gm.GetComponent<EditorModeController>().isDrawingTerrain = true;
    }

    public void ShowElementPanel()
    {
        mainPanel.SetActive(false);
        addElementPanel.SetActive(true);

        gm.GetComponent<EditorModeController>().isPlacingElements = true;
    }

    public void ShowAddNPCPanel()
    {
        mainPanel.SetActive(false);
        addNPC_Panel.SetActive(true);
    }

    public void ReturnToMainPanel()
    {
        mainPanel.SetActive(true);
        //loadMapPanel.SetActive(false);
        //saveMapPanel.SetActive(false);
        drawTerrainPanel.SetActive(false);
        addElementPanel.SetActive(false);
        addNPC_Panel.SetActive(false);

        gm.GetComponent<EditorModeController>().isDrawingTerrain = false;
        gm.GetComponent<EditorModeController>().isPlacingElements = false;
    }
}