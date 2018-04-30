using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatText : MonoBehaviour {
    string Stats;
    int statsnumber;

	// Use this for initialization
	void Start () {
        Stats = this.gameObject.GetComponent<Text>().text;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlusOne()
    {
        this.gameObject.GetComponentInParent<PlayerBehaviour>().SendMessage("Add", 1);
        this.gameObject.GetComponentInParent<PlayerBehaviour>().SendMessage("StatIncrease",this.name);

    }
    public void SubOne()
    {
        this.gameObject.GetComponentInParent<PlayerBehaviour>().SendMessage("Sub", -1);
        this.gameObject.GetComponentInParent<PlayerBehaviour>().SendMessage("StatIncrease", this.name);
    }
}
