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

    UIManager uiManager;

    public void Start () {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        myLineGoalFeedback = Instantiate(lineGoalFeedback);
        myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().origin = this.transform;

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
            }
            else if (patrolMovementPoints.Count > 0)
            {
                agent.destination = patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].position;

                myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().destination =
                    patrolMovementPoints[System.Int32.Parse(this.GetComponentInParent<NPCData>().patrolPointIndex[patrolIndex])].position;
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
        if (uiManager.isFeedbackEnabled)
        {
            if (!myLineGoalFeedback.activeSelf)
            {
                myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().ClearAllArrows();
                myLineGoalFeedback.SetActive(true);
            }
        }
        else
        {
            myLineGoalFeedback.SetActive(false);
        }

        if(myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().destination != null &&
            Vector3.Distance(myLineGoalFeedback.GetComponent<PatrolGoalFeedback>().destination, this.transform.position) > 0.5f)
        {
            myLineGoalFeedback.transform.position = this.transform.position + ((agent.destination - this.transform.position) / 2.0f);
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
