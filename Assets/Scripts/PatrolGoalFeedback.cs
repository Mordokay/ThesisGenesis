using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGoalFeedback : MonoBehaviour {

    public List<GameObject> Arrows;
    public Transform origin;
    public Vector3 destination;
    GameObject arrow;

    public float timeSinceLastArrow;
    public float arrowForceSpeed;
    Color currentColor;

    void Start () {
        arrow = Resources.Load("Arrow") as GameObject;

        Arrows = new List<GameObject>();
        timeSinceLastArrow = Time.timeSinceLevelLoad;
        arrowForceSpeed = 200.0f;
        destination = Vector3.one;

        currentColor = Color.green;
    }
	
    public void ClearAllArrows()
    {
        //Debug.Log("Clearing line with " + Arrows.Count + " arrows");
        for (int i = Arrows.Count - 1; i >= 0; i--)
        {
            Destroy(Arrows[i]);
            Arrows.RemoveAt(i);
        }
    }

    void ChangeLineColor()
    {
        for (int i = Arrows.Count - 1; i >= 0; i--)
        {
            if (Arrows[i].GetComponentInChildren<SpriteRenderer>().color.Equals(Color.green))
            {
                Arrows[i].GetComponentInChildren<SpriteRenderer>().color = Color.red;
                currentColor = Color.red;
            }
            else
            {
                Arrows[i].GetComponentInChildren<SpriteRenderer>().color = Color.green;
                currentColor = Color.green;
            }
        }
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ChangeLineColor();
        }

        timeSinceLastArrow += Time.deltaTime;
        if(!destination.Equals(Vector3.one) && timeSinceLastArrow > 0.1f)
        {
            GameObject myArrow = Instantiate(arrow, this.transform);
            myArrow.transform.position = origin.transform.position;
            myArrow.GetComponent<Rigidbody>().AddForce((destination - myArrow.transform.position).normalized * arrowForceSpeed);
            myArrow.GetComponentInChildren<SpriteRenderer>().color = currentColor;
            myArrow.transform.LookAt(destination);

            Arrows.Add(myArrow);

            for(int i = Arrows.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(Arrows[i].transform.position, destination) < 0.5f)
                {
                    Destroy(Arrows[i]);
                    Arrows.RemoveAt(i);
                }
            }
            timeSinceLastArrow = 0;
        }
	}
}