using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolMovement : MonoBehaviour {

    public List<Transform> patrolMovementPoints;
    public NavMeshAgent agent;
    public float waitTime;
    GameObject patrolPointHolder;

    public int patrolIndex;

    public GameObject lineGoalFeedback;
    GameObject myLineGoalFeedback;
    GameObject myTalkLine;

    UIManager uiManager;

    public GameObject thinkingBalloon;
    public GameObject currentGoalObject;

    public bool isWaiting;
    public float remainingDistance;

    public bool stopped;

    GameObject player;

    public void Start () {
        player = GameObject.FindGameObjectWithTag("Player");

        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
        //isWaiting = true;
        stopped = false;

        currentGoalObject = null;

        myLineGoalFeedback = Instantiate(lineGoalFeedback);
        myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().origin = this.transform;

        myTalkLine = Instantiate(lineGoalFeedback);
        myTalkLine.GetComponent<PatrolGoalFeedback>().origin = this.transform;
        myTalkLine.GetComponent<PatrolGoalFeedback>().isTalkArrow = true;

        //agent = GetComponent<NavMeshAgent>();
        //agent.ResetPath();
        waitTime = -1;

        GetNewGoal();
    }
	
    public void setUpPatrolMovementPoints()
    {
        patrolIndex = 0;
        patrolMovementPoints = new List<Transform>();
        patrolPointHolder = GameObject.FindGameObjectWithTag("PatrolPointsHolder");
        foreach (Transform tr in patrolPointHolder.transform) patrolMovementPoints.Add(tr);
    }
    
    void GetNewGoal()
    {
        if (this.GetComponentInParent<NPCData>().patrolPointIndex.Count > 0)
        {
            if (patrolIndex == this.GetComponentInParent<NPCData>().patrolPointIndex.Count)
            {
                patrolIndex = 0;
            }
            if (System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex]) >= patrolMovementPoints.Count)
            {
                this.GetComponentInParent<NPCData>().patrolPointIndex.RemoveAt(patrolIndex);
                GetNewGoal();
                return;
            }
            else if (patrolMovementPoints.Count > 0)
            {
                //Debug.Log(patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])]);
                if (patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])] != null)
                {
                    //agent.destination = patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].position;

                    currentGoalObject = patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].gameObject;
                    this.transform.LookAt(currentGoalObject.transform);
                    //myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().destination =
                    //    patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].position;
                }
                patrolIndex += 1;
            }
        }
        else
        {
            //agent.destination = this.transform.position;
        }
    }

    //When NPC reaches a position he waits for a couple of seconds before starting to move again
    float SetWaitTime()
    {
        if(currentGoalObject == null)
        {
            return 0;
        }

        waitTime = 0;

        foreach (NPCData.Interest interest in GetComponentInParent<NPCData>().interests)
        {
            foreach (Message patrolMessage in currentGoalObject.GetComponent<PatrolPointData>().messages)
            {
                Message.Tag foundTag = patrolMessage.tags.Find(t => t.name == interest.name);
                //If the message from patrol point contains a TAG that is of interest to the player
                if (foundTag != null)
                { 
                    /*
                    Interests are normalized between 0 - 1
                    tags on messages from events are not normalized and values are usually high (Ex: Berrries,50  Wood,40  Gathering,10)
                    If, for example, NPC has an interest of 0.6 in Berries and there is an event on patrol point with Berries,40
                    we have (40 * 0.6) / 10 = 2.6 seconds wait time. 
                    */
                    waitTime += (foundTag.weight * interest.weight) / 10.0f;
                }
            }
        }
        return waitTime;
    }

    public void UpdatePatrolPoints()
    {
        patrolMovementPoints.Clear();
        patrolMovementPoints.RemoveAll(item => item == null);
        foreach (Transform tr in patrolPointHolder.transform) patrolMovementPoints.Add(tr);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.gameObject.tag.Equals("NPC"))
        {
            //Debug.Log("Collided with another NPC!!!");
            this.GetComponentInParent<Social>().NPC_Colision(other.transform.parent.gameObject);
        }
    }

    void Update() {
        if (this.GetComponentInParent<NPCData>().NPCType == 0)
        {
            if (!stopped)
            {
                float step = 1.0f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, currentGoalObject.transform.position, step);

                //transform.Translate(currentGoalObject.transform.position.normalized * Time.deltaTime);
                //this.transform.position += currentGoalObject.transform.position.normalized * Time.deltaTime;
                if (Vector3.Distance(currentGoalObject.transform.position, this.transform.position) < 0.1f)
                {
                    GetNewGoal();
                }
            }
        }
        //The movement of Wizards
        else
        {
            if (!stopped)
            {
                if (this.GetComponent<WizardController>().isFollowingPlayer)
                {
                    float step = (1.9f + (1 - Vector3.Distance(this.transform.position, player.transform.position) / 5.0f)) * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
                    this.transform.LookAt(player.transform);
                }
                else if(currentGoalObject != null)
                {
                    float step = 1.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, currentGoalObject.transform.position, step);

                    //transform.Translate(currentGoalObject.transform.position.normalized * Time.deltaTime);
                    //this.transform.position += currentGoalObject.transform.position.normalized * Time.deltaTime;
                    if (Vector3.Distance(currentGoalObject.transform.position, this.transform.position) < 0.1f)
                    {
                        GetNewGoal();
                    }
                }
            }
        }
        /*
        remainingDistance = Vector3.Distance(currentGoalObject.transform.position, this.transform.position);
        if (uiManager.isGoalFeedbackEnabled)
        {
            if (this.GetComponentInParent<Social>().isTalking)
            {
                myLineGoalFeedback.SetActive(false);
                if (!this.GetComponentInParent<Social>().isReceivingMessage)
                {
                    if (!myTalkLine.activeSelf)
                    {
                        myTalkLine.SetActive(true);
                    }
                    //myTalkLine.GetComponent<PatrolGoalFeedback>().ClearAllArrows();
                    myTalkLine.GetComponent<PatrolGoalFeedback>().destination =
                    this.GetComponentInParent<Social>().talkPartner.transform.GetChild(1).position;
                }
                else
                {
                    myTalkLine.GetComponent<PatrolGoalFeedback>().ClearAllArrows();
                    myTalkLine.SetActive(false);
                }
            }
            else
            {
                if (!myLineGoalFeedback.activeSelf)
                {
                    myLineGoalFeedback.SetActive(true);
                    myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().ClearAllArrows();
                }

                if (myTalkLine.activeSelf)
                {
                    myTalkLine.SetActive(false);
                    myTalkLine.GetComponent<PatrolGoalFeedback>().ClearAllArrows();
                }
            }
        }
        else
        {
            myLineGoalFeedback.SetActive(false);
        }

        if (isWaiting)
        {
            if (!thinkingBalloon.activeSelf)
            {
                //Forces the balloon to move to the NPC Object position. The balloon is updated on NPCFeedbackUpdater script
                thinkingBalloon.transform.localPosition = this.transform.localPosition;

                //Activates thinking ballon 
                thinkingBalloon.SetActive(true);
            }

            waitTime -= Time.deltaTime;
            if(waitTime <= 0)
            {
                waitTime = 0;
                isWaiting = false;
                GetNewGoal();
            }
        }
        else if (Vector3.Distance(currentGoalObject.transform.position, this.transform.position) < 0.5f)
        {
            if (SetWaitTime() != 0)
            {
                isWaiting = true;
                //Forces the balloon to move to the NPC Object position. The balloon is updated on NPCFeedbackUpdater script
                thinkingBalloon.transform.localPosition = this.transform.localPosition;

                //Activates thinking ballon 
                thinkingBalloon.SetActive(true);
            }
            else
            {
                GetNewGoal();
                thinkingBalloon.SetActive(false);
            }
        }
        else
        {
            thinkingBalloon.SetActive(false);
        }
        */
    }
}
