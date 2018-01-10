using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrolMovement : MonoBehaviour {

    public List<Transform> patrolMovementPoints;
    public NavMeshAgent agent;
    public float waitTime;
    public float minimumWaitTime;
    public float maximumWaitTime;
    GameObject patrolPointHolder;

    public int patrolIndex;

    public GameObject lineGoalFeedback;
    GameObject myLineGoalFeedback;
    GameObject myTalkLine;

    UIManager uiManager;

    public void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        myLineGoalFeedback = Instantiate(lineGoalFeedback);
        myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().origin = this.transform;

        myTalkLine = Instantiate(lineGoalFeedback);
        myTalkLine.GetComponent<PatrolGoalFeedback>().origin = this.transform;
        myTalkLine.GetComponent<PatrolGoalFeedback>().isTalkArrow = true;

        agent = GetComponent<NavMeshAgent>();
        agent.ResetPath();
        waitTime = 0;
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
                    agent.destination = patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].position;

                    myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().destination =
                        patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].position;
                }
                patrolIndex += 1;
            }
        }
        else
        {
            agent.destination = this.transform.position;
        }
    }

    //When NPC reaches a position he waits for a couple of seconds before starting to move again
    void SetWaitTime()
    {
        waitTime = Random.Range(minimumWaitTime, maximumWaitTime);
    }

    public void UpdatePatrolPoints()
    {
        patrolMovementPoints.Clear();
        patrolMovementPoints.RemoveAll(item => item == null);
        foreach (Transform tr in patrolPointHolder.transform) patrolMovementPoints.Add(tr);
    }

    void Update () {
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

        if (transform.parent.name.Equals("Villager1"))
        {
            Debug.Log(agent.remainingDistance);
        }
        float dist = agent.remainingDistance;
        if(waitTime != 0)
        {
            waitTime -= Time.deltaTime;
            if(waitTime <= 0)
            {
                waitTime = 0;
                GetNewGoal();
            }
        }
        else if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && 
            agent.remainingDistance < 0.5)
        {
            SetWaitTime();
        }

    }
}
