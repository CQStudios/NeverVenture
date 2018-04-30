using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOverWorldFixer : MonoBehaviour {
    UnitBehavior player;
    private bool set = false;

    public bool DisablesOverworld = false;
	// Use this for initialization
	void Start () {
        set = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null && !set)
        {
            player = GameObject.Find("OverWorldController").GetComponent<OverWorldControllerScript>().getPlayer();
            return;
        }
        else if (!set)
        {
            set = true;
            gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClickFunction);
        }
	}

    private void OnClickFunction()
    {
        player.SetOverworldDestination(Vector3.zero);
        GameObject.Find("OverWorldController").GetComponent<OverWorldControllerScript>().MoveOnClick = !DisablesOverworld;
    }
}
