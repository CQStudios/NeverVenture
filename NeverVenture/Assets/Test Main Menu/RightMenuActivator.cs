using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightMenuActivator : MonoBehaviour {

    private Vector2 StartPos = new Vector2(1500, 0);
    private Vector2 EndPos = new Vector2(-1500, 0);
    public float T;

    public float speed = 100.0F;


    // Use this for initialization
    void Start () {


    }
    void update()
    {
        
    }
    public void Activate()
    {
        gameObject.GetComponent<RectTransform>().localPosition = StartPos;
        gameObject.GetComponent<RectTransform>().localPosition = Vector2.Lerp(EndPos, StartPos, 5000);
        //this.gameObject.GetComponent<RectTransform>().position = StartPos;
        //this.gameObject.GetComponent<RectTransform>().localPosition = StartPos;


    }
}


