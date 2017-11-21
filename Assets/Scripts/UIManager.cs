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
        Instantiate(interestNameInputField, interestNPCList.transform);
        Instantiate(interestWeightInputField, interestNPCList.transform);
        Instantiate(separator, interestNPCList.transform);
    }
    public void addFriendToNPC()
    {
        Instantiate(friendNameInputField, friendsNPCList.transform);
        Instantiate(friendLevelInputField, friendsNPCList.transform);
        Instantiate(separator, friendsNPCList.transform);
    }

    public void RemoveLast(int type)
    {
        if (type == 0)
        {
            List<GameObject> Interests = new List<GameObject>();
            foreach (Transform npc in interestNPCList.transform)
            {
                Interests.Add(npc.gameObject);
            }
            for (int i = Interests.Count - 1; i >= Interests.Count - 3; i--)
            {
                Destroy(Interests[i]);
            }
        }
        else
        {
            List<GameObject> Friends = new List<GameObject>();
            foreach (Transform npc in friendsNPCList.transform)
            {
                Friends.Add(npc.gameObject);
            }
            for (int i = Friends.Count - 1; i >= Friends.Count - 3; i--)
            {
                Destroy(Friends[i]);
            }
        }
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
        gm.GetComponent<EditorModeController>().removeElementButtonImage.color = Color.white;
        gm.GetComponent<EditorModeController>().removePatrolButtonImage.color = Color.white;
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

    public void ToggleRemoveNPC()
    {
        gm.GetComponent<EditorModeController>().ToggleRemoveNPC();
    }

    public void Play()
    {
        Time.timeScale = 1.0f;
        SidePanel.SetActive(false);
        canvasBackroundBox.SetActive(false);
        gm.GetComponent<EditorModeController>().isEditorMode = false;
        gm.GetComponent<Zoom>().zoomToPlayMode = true;
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
        gm.GetComponent<EditorModeController>().removeElement = false;
        gm.GetComponent<EditorModeController>().removeTerrain = false;

        gm.GetComponent<EditorModeController>().isPlacingNPC = false;
        gm.GetComponent<EditorModeController>().removeNPC = false;
        gm.GetComponent<EditorModeController>().removeNPCButtonImage.color = Color.white;
    }
}