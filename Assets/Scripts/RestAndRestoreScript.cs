using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestAndRestoreScript : MonoBehaviour {
    UnitBehavior player;
    public float radius;
    public float incAmount;
	// Use this for initialization
	void Start () {
        incAmount = 5.0f / 60.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null)
        {
            player = GameObject.Find("OverWorldController").GetComponent<OverWorldControllerScript>().player;
            return;
        }

        //heal the player as long as they're here.
        if ((gameObject.transform.position - player.transform.position).magnitude  < radius)
        {
            if (player.damage > incAmount)
                player.damage -= incAmount;
            else
                player.damage = 0;
            if (player.wounds > incAmount)
                player.wounds -= incAmount;
            else
                player.wounds = 0;

        }
	}
}
