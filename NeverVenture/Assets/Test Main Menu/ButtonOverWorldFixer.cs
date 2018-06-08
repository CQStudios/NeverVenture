using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOverWorldFixer : MonoBehaviour {
    UnitBehavior player;
    private OverWorldControllerScript overcontroller;
    private bool set = false;
    private bool hovered = false;
    private bool isdown = false;
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
        if (overcontroller == null)
        {
            overcontroller = GameObject.Find("OverWorldController").GetComponent<OverWorldControllerScript>();
        }
        else if (hovered && isdown)
        {
            Debug.Log("should be affecting the motion.");
            player.SetOverworldDestination(Vector3.zero);
            //overcontroller.MoveOnClick = !DisablesOverworld;
        }
    }

    private void OnClickFunction()
    {
        player.SetOverworldDestination(Vector3.zero);
        overcontroller.MoveOnClick = !DisablesOverworld;
    }

    public void OnMouseEnter()
    {
        this.hovered = true;
    }

    public void OnMouseDown()
    {
        this.isdown = true;
    }
    public void OnMouseUp()
    {
        this.isdown = false;
    }
    public void OnMouseExit()
    {
        this.hovered = false;
        this.isdown = false;
    }


}
