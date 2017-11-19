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

    void Start () {
        patrolMovementPoints = new List<Transform>();
        patrolPointHolder = GameObject.FindGameObjectWithTag("PatrolPointsHolder");
        foreach (Transform tr in patrolPointHolder.transform) patrolMovementPoints.Add(tr);
        agent = GetComponent<NavMeshAgent>();
        waitTime = 0;
        //this.transform.position = patrolMovementPoints[Random.Range(0, patrolMovementPoints.Length)].position;
    }
	
    void GetNewGoal()
    {
        if (patrolMovementPoints.Count > 0)
        {
            int index = Random.Range(0, patrolMovementPoints.Count);
            if (patrolMovementPoints[index] == null)
            {
                UpdatePatrolPoints();
            }
            else {
                agent.destination = patrolMovementPoints[index].position;
            }
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
            agent.remainingDistance == 0)
        {
            SetWaitTime();
        }

    }
}
