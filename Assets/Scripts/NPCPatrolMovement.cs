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

    public void Start () {
        patrolIndex = 0;
        patrolMovementPoints = new List<Transform>();
        patrolPointHolder = GameObject.FindGameObjectWithTag("PatrolPointsHolder");
        foreach (Transform tr in patrolPointHolder.transform) patrolMovementPoints.Add(tr);
        agent = GetComponent<NavMeshAgent>();
        agent.ResetPath();
        waitTime = 0;
    }
	
    void GetNewGoal()
    {
        if(patrolIndex == this.GetComponent<NPCData>().patrolPointIndex.Count){
            patrolIndex = 0;
        }
        if(System.Int32.Parse(this.GetComponent<NPCData>().patrolPointIndex[patrolIndex]) > patrolMovementPoints.Count)
        {
            this.GetComponent<NPCData>().patrolPointIndex.RemoveAt(patrolIndex);
            GetNewGoal();
            return;
        }
        if (patrolMovementPoints.Count > 0)
        {
            agent.destination = patrolMovementPoints[System.Int32.Parse(this.GetComponent<NPCData>().patrolPointIndex[patrolIndex])].position;
            patrolIndex += 1;
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
