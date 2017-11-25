using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject SidePanel;
    public GameObject mainPanel;
    public GameObject addPlayerPanel;
    public GameObject drawTerrainPanel;
    public GameObject addElementPanel;
    public GameObject addNPC_Panel;
    public GameObject inspectorPanel;
    public GameObject canvasBackroundBox;

    public GameObject listNatural;
    public GameObject listConstruct;

    public GameObject interestNPCList;
    public GameObject friendsNPCList;
    public GameObject interestChangeTagButton;
    public GameObject interestChangeTagList;
    public GameObject interestWeightInputField;
    public GameObject friendNameInputField;
    public GameObject friendLevelInputField;
    public GameObject separator;

    public GameObject patrolPointNumber;
    public GameObject listPatrolPoints;

    public GameObject npcTypeList;
    public GameObject npcTypeButton;

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

    public void addInterestToNPC()
    {
        GameObject myInterestChangeTagButton = Instantiate(interestChangeTagButton, interestNPCList.transform);
        GameObject myInterestChangeTagList = Instantiate(interestChangeTagList, interestNPCList.transform);
        myInterestChangeTagButton.GetComponent<TagListSelectorController>().listOfTags = myInterestChangeTagList;

        foreach(SingleTagController stc in myInterestChangeTagList.GetComponentsInChildren<SingleTagController>())
        {
            stc.tagButton = myInterestChangeTagButton;
        }

        Instantiate(interestWeightInputField, interestNPCList.transform);
        Instantiate(separator, interestNPCList.transform);
    }

    public void addFriendToNPC()
    {
        Instantiate(friendNameInputField, friendsNPCList.transform);
        Instantiate(friendLevelInputField, friendsNPCList.transform);
        Instantiate(separator, friendsNPCList.transform);
    }

    public void addNumberToPatrolNPC()
    {
        Instantiate(patrolPointNumber, listPatrolPoints.transform);
        
    }

    public void RemoveLast(int type)
    {
        //TAGS
        if (type == 0)
        {
            List<GameObject> Interests = new List<GameObject>();
            foreach (Transform npc in interestNPCList.transform)
            {
                Interests.Add(npc.gameObject);
            }
            if (Interests.Count > 0)
            {
                for (int i = Interests.Count - 1; i >= Interests.Count - 4; i--)
                {
                    Destroy(Interests[i]);
                }
            }
        }
        //FRIENDS
        else if(type == 1)
        {
            List<GameObject> Friends = new List<GameObject>();
            foreach (Transform npc in friendsNPCList.transform)
            {
                Friends.Add(npc.gameObject);
            }
            if (Friends.Count > 0)
            {
                for (int i = Friends.Count - 1; i >= Friends.Count - 3; i--)
                {
                    Destroy(Friends[i]);
                }
            }
        }
        else
        {
            List<GameObject> PatrolPointNumber = new List<GameObject>();
            foreach (Transform patrol in listPatrolPoints.transform)
            {
                PatrolPointNumber.Add(patrol.gameObject);
            }
            if (PatrolPointNumber.Count > 0)
            {
                Destroy(PatrolPointNumber[PatrolPointNumber.Count - 1]);
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

    public void ShowInspectorPanel()
    {
        mainPanel.SetActive(false);
        inspectorPanel.SetActive(true);

        gm.GetComponent<EditorModeController>().isInspectingElement = true;
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

    public void SetNPCType(int x)
    {
        gm.GetComponent<EditorModeController>().selectedNPCType = x;
        npcTypeList.SetActive(false);
        switch (x)
        {
            case 0:
                npcTypeButton.GetComponentInChildren<Text>().text = "Fat NPC";
                break;
            case 1:
                npcTypeButton.GetComponentInChildren<Text>().text = "Wizard NPC";
                break;
        }
    }

    public void ToggleNPCTypeList()
    {
        if (npcTypeList.activeSelf)
        {
            npcTypeList.SetActive(false);
        }
        else
        {
            npcTypeList.SetActive(true);
        }
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
        inspectorPanel.SetActive(false);

        gm.GetComponent<EditorModeController>().isDrawingTerrain = false;
        gm.GetComponent<EditorModeController>().isPlacingElements = false;
        gm.GetComponent<EditorModeController>().isPlacingNPC = false;
        gm.GetComponent<EditorModeController>().isPlacingPlayer = false;
        gm.GetComponent<EditorModeController>().removeElement = false;
        gm.GetComponent<EditorModeController>().removeTerrain = false;

        gm.GetComponent<EditorModeController>().isInspectingElement = false;

        gm.GetComponent<EditorModeController>().isPlacingNPC = false;
        gm.GetComponent<EditorModeController>().removeNPC = false;
        gm.GetComponent<EditorModeController>().removeNPCButtonImage.color = Color.white;
    }
}