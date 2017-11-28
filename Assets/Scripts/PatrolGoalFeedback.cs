using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGoalFeedback : MonoBehaviour {

    public List<GameObject> Arrows;
    public Transform origin;
    public Vector3 destination;
    GameObject arrow;

    public float timeSinceLastArrow;
    public float arrowMoveSpeed;

    void Start () {
        arrow = Resources.Load("Arrow") as GameObject;

        Arrows = new List<GameObject>();
        timeSinceLastArrow = Time.timeSinceLevelLoad;
        arrowMoveSpeed = 2.0f;
    }
	
	void Update () {
        timeSinceLastArrow += Time.deltaTime;
        if(timeSinceLastArrow > 0.1f)
        {
            GameObject myArrow = Instantiate(arrow, this.transform);
            Arrows.Add(myArrow);

            foreach (GameObject arrow in Arrows)
            {
                arrow.transform.position += (destination - arrow.transform.position).normalized * arrowMoveSpeed * Time.deltaTime;
            }
            timeSinceLastArrow = Time.timeSinceLevelLoad;
        }
	}
}
