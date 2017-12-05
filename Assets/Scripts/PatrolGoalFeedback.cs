using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGoalFeedback : MonoBehaviour {

    public List<GameObject> Arrows;
    public Transform origin;
    public Vector3 destination;

    GameObject arrow;

    public float timeSinceLastArrow;
    public float timeSinceLastGoalArrow;

    public float arrowForceSpeed;
    Color currentColor;

    public bool isTalkArrow;
    Color talkColor;

    void Start () {
        talkColor = Color.blue;
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

    void ChangeLineColor(Color newColor)
    {
        for (int i = Arrows.Count - 1; i >= 0; i--)
        {
            Arrows[i].GetComponentInChildren<SpriteRenderer>().color = newColor;
        }
    }

	void Update () {
        /*
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (Arrows.Count > 0)
            {
                if (Arrows[0].GetComponentInChildren<SpriteRenderer>().color.Equals(Color.green))
                {
                    currentColor = Color.red;
                }
                else
                {
                    currentColor = Color.green;
                }
            }
            ChangeLineColor(currentColor);
        }
        */

        if (isTalkArrow)
        {
            timeSinceLastGoalArrow += Time.deltaTime;
            if (!destination.Equals(Vector3.one) && timeSinceLastGoalArrow > 0.4f)
            {
                GameObject myArrow = Instantiate(arrow, this.transform);
                myArrow.transform.position = origin.transform.position;
                myArrow.GetComponent<Rigidbody>().AddForce((destination - myArrow.transform.position).normalized * (arrowForceSpeed));
                myArrow.GetComponentInChildren<SpriteRenderer>().color = talkColor;
                //Debug.Log("destination " + destination + " myArrow.transform.position " + myArrow.transform.position);
                myArrow.transform.LookAt(destination);
                Arrows.Add(myArrow);
                timeSinceLastGoalArrow = 0;
            }
        }
        else
        {
            timeSinceLastArrow += Time.deltaTime;
            if (!destination.Equals(Vector3.one) && timeSinceLastArrow > 0.1f)
            {
                GameObject myArrow = Instantiate(arrow, this.transform);
                myArrow.transform.position = origin.transform.position;
                myArrow.GetComponent<Rigidbody>().AddForce((destination - myArrow.transform.position).normalized * arrowForceSpeed);
                myArrow.GetComponentInChildren<SpriteRenderer>().color =  origin.transform.GetComponentInChildren<SpriteRenderer>().color;//currentColor;

                myArrow.transform.LookAt(destination);
                Arrows.Add(myArrow);
                timeSinceLastArrow = 0;
            }
        }

        for (int i = Arrows.Count - 1; i >= 0; i--)
        {
            if (isTalkArrow)
            {
                //Debug.Log("arrowPos: " + Arrows[i].transform.position + " destination  " + destination);
                if (Vector3.Distance(Arrows[i].transform.position, destination) < 0.1f)
                {
                    //Debug.Log("arrowPos: " + Arrows[i].transform.position + " destination  " + destination);
                    //Debug.Log("Destroy arrow!!!");
                    Destroy(Arrows[i]);
                    Arrows.RemoveAt(i);
                }
            }
            else
            {
                if (Vector3.Distance(Arrows[i].transform.position, destination) < 0.5f)
                {
                    Debug.Log("Fuck me!!!!!!");
                    Destroy(Arrows[i]);
                    Arrows.RemoveAt(i);
                }
            }
        }
	}
}