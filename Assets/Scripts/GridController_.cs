using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController_ : MonoBehaviour {

    public HexGrid_ MyGrid;
    public int radius;
    public GameObject marker;
	// Use this for initialization
	void Start () {
        //u = x direction, and v = some combination of x and z.
        Vector3 u = new Vector3(1, 0, 0);
        Vector3 v = Quaternion.Euler(0, 120, 0) * u;
        MyGrid = new HexGrid_(u,v,marker, radius);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
