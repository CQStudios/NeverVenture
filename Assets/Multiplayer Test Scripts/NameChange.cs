using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameChange : MonoBehaviour {
   

    // Use this for initialization
    void Awake () {
        string CurName = name; //set Curname to current name of object. 
        if (CurName != "Unit") //Unit will always be a clone so will attempt to rename on player joining.
        {
            print(CurName); // find out name of current object
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
