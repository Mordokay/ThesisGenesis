using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageLayerController : MonoBehaviour {

    public InputField messageID;
    bool hasUpdatedNPCs;
    public GameObject npcHolder;

    void Start () {
        hasUpdatedNPCs = false;
    }
	
	void Update () {
        if (!hasUpdatedNPCs)
        {
            UpdateNPCCanvas();
            hasUpdatedNPCs = true;
        }
	}

    private void OnDisable()
    {
        Debug.Log("I was disabled and I ran this function!!! I was watching message with ID: " + messageID.text);
    }

    public void UpdateNPCCanvas()
    {
        if (this.GetComponentInParent<UIManager>().isMessageLayerEnabled)
        {
            Debug.Log("Updating new messageID: " + messageID.text);
            foreach (Transform npc in npcHolder.transform)
            {
                Debug.Log(npc.gameObject.name);
                foreach (Message m in npc.gameObject.GetComponent<NPCData>().messages)
                {
                    if (m.id.Equals(System.Int32.Parse(messageID.text)))
                    {
                        //setsActivethe messageFeedback
                    }
                }
            }
        }
    }
}
