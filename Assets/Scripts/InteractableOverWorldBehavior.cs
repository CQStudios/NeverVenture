using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableOverWorldBehavior : MonoBehaviour {
    public float radius = 2;
    private Renderer rend;          //this controls the color of our object
    private Color originalColor;    //this holds the color the bject was originally

    public string target = "Multi-Unit";
    private bool triggered = false;
    private bool triggered_old = false;
    private List<Transform> nearbyObjects; 

	// Use this for initialization
	void Start () {
        rend = transform.GetComponentInChildren<Renderer>();
        originalColor = rend.material.color;
        nearbyObjects = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        //reset state of triggered and nearbyObjects list.
        nearbyObjects.Clear();
        triggered = false;
        //spherecast to determine if there are objects within our radius of effect.
        RaycastHit[] hit = Physics.SphereCastAll(transform.position, radius, Vector3.up, 1);
        foreach (RaycastHit o in hit ) {
            Transform obj = o.collider.transform.root;
            if (target.Contains(obj.name) || obj.name.Contains(target))
            {
                Debug.Log("object in vicinity: " + obj.name);
                triggered = true;
                if (!nearbyObjects.Contains(obj))
                {
                    nearbyObjects.Add(obj);
                }
            }
        }

        //if the triggered status has changed, then call our ProximityChanged method.
        if (triggered != triggered_old)
        {
            if (triggered)
            {
                rend.material.color = Color.red;
            }
            else
            {
                rend.material.color = originalColor;
            }
        }
        triggered_old = triggered;
        foreach ( Transform obj in nearbyObjects)
        {
            Debug.Log("    " + obj.name);
        }
    }
}
