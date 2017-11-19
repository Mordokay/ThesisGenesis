using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayModeManager : MonoBehaviour {

    int messageID;

	void Start () {
        messageID = 0;		
	}
	
	public int getMessageId()
    {
        return messageID++;
    }
}
