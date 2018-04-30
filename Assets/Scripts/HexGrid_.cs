using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*      HexGrid_ class provides tools for defining a 2d plane along U and V vectors.
 *  Class that allows the creation of a hexagonal grid. Each Axis follows a U and V vector to establish the grid.
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 *  NOTE: NOT A MONOBEHAVIOR. CLASS DOES NOTHING ON IT'S OWN WITHOUT BEING CALLED.
 */
public class HexGrid_ {

    //declares the "axes" for the grid.
    //cartesian grid can be achieved using (1,0,0) and (0,0,1) for an XZ plane.
    private Vector3 u, v;
    public Vector3 U
    {
        get { return u; }
        set { u = value; }
    }
    public Vector3 V
    {
        get { return v; }
        set { v = value; }
    }

    private GameObject marker;
    private List<GameObject> positions;

    public HexGrid_(Vector3 U_, Vector3 V_,  GameObject prefab, float range_ = 10)
    {
        u = U_.normalized;
        v = V_.normalized;
        marker = prefab;
        positions = new List<GameObject>();
        BuildGrid(range_);
    }
    
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // place markers at integer positions (0,1) (3,3) (-3,2) etc within the range such that the distance from the origin is within the range of the map.
    void BuildGrid(float range)
    {
        //loop over U axis -> loop over V axis.
        GameObject element;
        for (int x = (int)-range; x <= range; x++){
            for (int y = (int)-range; y <= range; y++){
                //if the distance is less than range, build the position.
                Vector3 xy = x * u + y * v;
                if (Mathf.Abs(x - y) <= range)
                {
                    element = GameObject.Instantiate(marker, xy, Quaternion.identity);
                    element.name = "Tile: " + x + ", " + y;
                    positions.Add(element);
                    element.GetComponent<HexTile>().X = x;
                    element.GetComponent<HexTile>().Y = y;
                }

            }
        }
        
        
    }
}
