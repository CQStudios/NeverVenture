using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour {
    public float radius = 5.0f;
    private float rad_old = 1f;
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //shortcircuit
        if (radius != rad_old)
        {
            transform.localScale = new Vector3(radius, 1, radius);
            rad_old = radius;
        }
    }
}
