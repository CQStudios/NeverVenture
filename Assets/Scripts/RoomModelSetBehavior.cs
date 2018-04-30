using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModelSetBehavior : MonoBehaviour {

    public GameObject Single, Corner, Hallway, Triple, Quad;
    public WFloor CurrentFloor;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (CurrentFloor == null)
        {
            GetFloorFromSibling();
        }
        else
        {
            foreach (WRoom rm in CurrentFloor.map)
            {
                Debug.DrawRay(rm.Location, Vector3.forward, Color.red);
            }
        }
	}

    void SetFloor( WFloor obj)
    {
        CurrentFloor = obj;
    }


    /// <summary>
    /// gets the Wfloor object from the Walker object attached to our parent.
    /// </summary>
    void GetFloorFromSibling()
    {
        CurrentFloor = GetComponent<DungeonManager>().myfloor;
    }

}
