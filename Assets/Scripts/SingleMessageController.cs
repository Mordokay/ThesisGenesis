using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMessageController : MonoBehaviour {

    public int messageId;
    UIManager uiManager;

    public void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
    }

    public void RemoveThisMessage()
    {
        uiManager.removeMessageWithId(messageId);
        Destroy(this.gameObject);
    }
}
