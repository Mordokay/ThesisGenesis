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

    public GameObject npcHolder;

    GameObject gm;

    public bool isFeedbackEnabled;

    public GameObject npcUpdaterPanel;
    public GameObject npcUpdaterName;
    public GameObject npcUpdaterAssertiveness;
    public GameObject npcUpdaterCooperativeness;
    public GameObject npcUpdaterInterestHolder;
    public GameObject npcUpdaterMessagesHolder;
    public GameObject messageElement;
    public GameObject NPCBeingUpdated;
    public List<int> messageIdsToRemoveNPCUpdater;

    void Start()
    {
        messageIdsToRemoveNPCUpdater = new List<int>();
        isFeedbackEnabled = false;
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

    public void ToggleFeedback()
    {
        if (isFeedbackEnabled)
        {
            foreach (Transform npc in npcHolder.transform)
            {
                npc.gameObject.GetComponent<NPCFeedbackUpdater>().feedbackCanvas.SetActive(false);
            }
            isFeedbackEnabled = false;
        }
        else
        {
            foreach (Transform npc in npcHolder.transform)
            {
                npc.gameObject.GetComponent<NPCFeedbackUpdater>().feedbackCanvas.SetActive(true);
                npc.gameObject.GetComponent<NPCFeedbackUpdater>().refreshFeedbackCanvas();
            }
            isFeedbackEnabled = true;
        }
    }

    public void RefreshNPCUpdater(NPCData data)
    {
        messageIdsToRemoveNPCUpdater = new List<int>();
        npcUpdaterPanel.SetActive(true);
        NPCBeingUpdated = data.gameObject;

        npcUpdaterName.GetComponent<InputField>().text = data.npcName;
        npcUpdaterAssertiveness.GetComponent<Slider>().value = data.assertiveness;
        npcUpdaterCooperativeness.GetComponent<Slider>().value = data.cooperativeness;

        foreach (Transform child in npcUpdaterInterestHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in npcUpdaterMessagesHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (NPCData.Interest i in data.interests)
        {
            GameObject myInterestChangeTagButton = Instantiate(interestChangeTagButton, npcUpdaterInterestHolder.transform);
            GameObject myInterestChangeTagList = Instantiate(interestChangeTagList, npcUpdaterInterestHolder.transform);
            myInterestChangeTagButton.GetComponent<TagListSelectorController>().listOfTags = myInterestChangeTagList;

            foreach (SingleTagController stc in myInterestChangeTagList.GetComponentsInChildren<SingleTagController>())
            {
                stc.tagButton = myInterestChangeTagButton;
            }

            Instantiate(interestWeightInputField, npcUpdaterInterestHolder.transform);
            Instantiate(separator, npcUpdaterInterestHolder.transform);

            myInterestChangeTagButton.GetComponentInChildren<Text>().text = i.name;
            interestWeightInputField.GetComponent<InputField>().text = i.weight.ToString();
        }

        foreach (Message message in data.messages)
        {
            GameObject myMessageElement = Instantiate(messageElement, npcUpdaterMessagesHolder.transform);
            string messageText = "";
            messageText += "ID: " + message.id + "Transmission Time: " + message.messageTimeOfLife + System.Environment.NewLine;
            foreach(Message.Tag t in message.tags)
            {
                messageText += "(" + t.name + "," + t.weight + "),";
            }
            if(message.tags.Count > 0)
            {
                messageText = messageText.Substring(0, messageText.Length - 1);
            }
            myMessageElement.GetComponentInChildren<Text>().text = messageText;

            myMessageElement.GetComponent<SingleMessageController>().messageId = message.id;
        }
    }

    public void UpdateNPC()
    {
        NPCBeingUpdated.GetComponent<NPCData>().npcName = npcUpdaterName.GetComponent<InputField>().text;
        NPCBeingUpdated.GetComponent<NPCData>().assertiveness = npcUpdaterAssertiveness.GetComponent<Slider>().value;
        NPCBeingUpdated.GetComponent<NPCData>().cooperativeness = npcUpdaterCooperativeness.GetComponent<Slider>().value;

        List<GameObject> Interests = new List<GameObject>();
        foreach (Transform npc in npcUpdaterInterestHolder.transform)
        {
            Interests.Add(npc.gameObject);
        }

        NPCBeingUpdated.GetComponent<NPCData>().interests = new List<NPCData.Interest>();

        if (Interests.Count > 0)
        {
            float interestTotalWeight = 0;
            for (int i = 0; i < Interests.Count; i = i + 4)
            {
                if (!Interests[i].GetComponentInChildren<Text>().text.Equals("Interest Name"))
                {
                    interestTotalWeight += float.Parse(Interests[i + 2].GetComponent<InputField>().text);
                }
            }

            for (int i = 0; i < Interests.Count; i = i + 4)
            {
                if (!Interests[i].GetComponentInChildren<Text>().text.Equals("Interest Name"))
                {
                    NPCBeingUpdated.GetComponent<NPCData>().interests.Add(
                        new NPCData.Interest(Interests[i].GetComponentInChildren<Text>().text,
                        float.Parse(Interests[i + 2].GetComponent<InputField>().text) / interestTotalWeight));
                    Interests[i + 2].GetComponent<InputField>().text = 
                        (float.Parse(Interests[i + 2].GetComponent<InputField>().text) / interestTotalWeight).ToString();
                }
            }
        }

        foreach (int id in messageIdsToRemoveNPCUpdater)
        {
            Message m = NPCBeingUpdated.GetComponent<NPCData>().messages.Find(x => x.id == id);
            if (m != null)
            {
                NPCBeingUpdated.GetComponent<NPCData>().messages.Remove(m);
            }
        }
    }

    public void addUpdatedInterestToNPC()
    {
        GameObject myInterestChangeTagButton = Instantiate(interestChangeTagButton, npcUpdaterInterestHolder.transform);
        GameObject myInterestChangeTagList = Instantiate(interestChangeTagList, npcUpdaterInterestHolder.transform);
        myInterestChangeTagButton.GetComponent<TagListSelectorController>().listOfTags = myInterestChangeTagList;

        foreach (SingleTagController stc in myInterestChangeTagList.GetComponentsInChildren<SingleTagController>())
        {
            stc.tagButton = myInterestChangeTagButton;
        }

        Instantiate(interestWeightInputField, npcUpdaterInterestHolder.transform);
        Instantiate(separator, npcUpdaterInterestHolder.transform);
    }

    public void removeMessageWithId(int id)
    {
        messageIdsToRemoveNPCUpdater.Add(id);
        /*
        Message m = NPCBeingUpdated.GetComponent<NPCData>().messages.Find(x => x.id == id);
        if(m != null)
        {
            NPCBeingUpdated.GetComponent<NPCData>().messages.Remove(m);
        }
        */
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
        else if (type == 2)
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
        //Removes last Interest from NPC Updater
        else if (type == 3)
        {
            List<GameObject> Interests = new List<GameObject>();
            foreach (Transform npc in npcUpdaterInterestHolder.transform)
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

        Time.timeScale = this.GetComponent<TimeSpeedController>().currentTime;

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
        npcUpdaterPanel.SetActive(false);
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

        NPCBeingUpdated = null;
    }
}