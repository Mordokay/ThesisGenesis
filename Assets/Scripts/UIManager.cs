using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject SidePanel;
    public GameObject mainPanel;
    public GameObject addPlayerPanel;
    public GameObject drawTerrainPanel;
    public GameObject addElementPanel;
    public GameObject addNPC_Panel;
    public GameObject canvasBackroundBox;

    public GameObject listNatural;
    public GameObject listConstruct;

    public GameObject interestNPCList;
    public GameObject friendsNPCList;
    public GameObject interestNameInputField;
    public GameObject interestWeightInputField;
    public GameObject friendNameInputField;
    public GameObject friendLevelInputField;
    public GameObject separator;

    GameObject gm;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager");
    }

    public void ShowNaturals()
    {
        listNatural.SetActive(true);
        listConstruct.SetActive(false);
    }

    public void ShowConstructs()
    {
        listNatural.SetActive(false);
        listConstruct.SetActive(true);
    }

    public void ShowPlayerPanel()
    {
        mainPanel.SetActive(false);
        addPlayerPanel.SetActive(true);
    }

    /*
     * public GameObject interestNameInputField;
    public GameObject interestWeightInputField;
    public GameObject friendNameInputField;
    public GameObject friendLevelInputField;
    public GameObject separator;
     */
    public void addInterestToNPC()
    {
        GameObject interestName = Instantiate(interestNameInputField);
        interestName.transform.parent = interestNPCList.transform;
        GameObject interestWeight = Instantiate(interestWeightInputField);
        interestWeight.transform.parent = interestNPCList.transform;
        GameObject mySeparator = Instantiate(separator);
        mySeparator.transform.parent = interestNPCList.transform;
    }
    public void addFriendToNPC()
    {
        GameObject friendName = Instantiate(friendNameInputField);
        friendName.transform.parent = friendsNPCList.transform;
        GameObject friendLevel = Instantiate(friendLevelInputField);
        friendLevel.transform.parent = friendsNPCList.transform;
        GameObject mySeparator = Instantiate(separator);
        mySeparator.transform.parent = friendsNPCList.transform;
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

        ShowNaturals();

        gm.GetComponent<EditorModeController>().isPlacingElements = true;
        gm.GetComponent<EditorModeController>().removeElement = false;
        gm.GetComponent<EditorModeController>().removePatrolPoint = false;
        gm.GetComponent<EditorModeController>().removeElementFeedback.SetActive(false);
        gm.GetComponent<EditorModeController>().removePatrolFeedback.SetActive(false);
    }

    public void ShowAddNPCPanel()
    {
        gm.GetComponent<EditorModeController>().isPlacingNPC = true;
        mainPanel.SetActive(false);
        addNPC_Panel.SetActive(true);
    }

    public void ToggleRemoveTerrain()
    {
        gm.GetComponent<EditorModeController>().ToggleRemoveTerrain();
    }

    public void ToggleRemoveElement()
    {
        gm.GetComponent<EditorModeController>().ToggleRemoveElement();
    }

    public void ToggleRemovePatrol()
    {
        gm.GetComponent<EditorModeController>().ToggleRemovePatrol();
    }

    public void Play()
    {
        Time.timeScale = 1.0f;
        SidePanel.SetActive(false);
        canvasBackroundBox.SetActive(false);
        gm.GetComponent<EditorModeController>().isEditorMode = false;
    }

    public void Pause()
    {
        SidePanel.SetActive(true);
        canvasBackroundBox.SetActive(true);
        Time.timeScale = 0.0f;
        gm.GetComponent<EditorModeController>().isEditorMode = true;
    }

    public void ReturnToMainPanel()
    {
        mainPanel.SetActive(true);
        addPlayerPanel.SetActive(false);
        drawTerrainPanel.SetActive(false);
        addElementPanel.SetActive(false);
        addNPC_Panel.SetActive(false);

        gm.GetComponent<EditorModeController>().isDrawingTerrain = false;
        gm.GetComponent<EditorModeController>().isPlacingElements = false;
        gm.GetComponent<EditorModeController>().isPlacingNPC = false;
        gm.GetComponent<EditorModeController>().isPlacingPlayer = false;
    }
}