using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    float t;
    int x;
    int l;
    public Vector3 StartP;
    public Vector3 StartEnd;
    public GameObject raycastObject;
    RaycastHit objectHit;
    // Use this for initialization
    void Start () {
        StartP = transform.position;
        StartEnd = new Vector3(Random.Range(-300.0f, 300.0f), Random.Range(-300.0f, 300.0f), StartP.z);
    }
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime / 15;
        if (StartP != StartEnd && x == 0)
        {
            transform.position = Vector3.Lerp(StartP, StartEnd, t);
        }
        if(transform.position == StartEnd)
        {
            x = 1;
            l = 1;
            StartP = StartEnd;
            StartEnd.x = Random.Range(-300.0f, 300.0f);
            StartEnd.y = Random.Range(-300.0f, 300.0f);
            print(StartEnd);
            print(StartP);
            print(transform.position);
        }
        if( x == 1 && l == 1)
        {
            x = 0;
            l = 0;
            t = 0;
        }
        //Vector3 fwd = raycastObject.transform.TransformDirection(Vector3.forward);
        //Debug.DrawRay(raycastObject.transform.position, fwd * 300, Color.green);
        //if (Physics.Raycast(raycastObject.transform.position, fwd, out objectHit, 300))
        //{
        //    if (objectHit.transform.tag == "MainMenuMap")
        //    {
                
        //        print(objectHit.transform.position);
        //        print(gameObject.transform.position);
        //        if (gameObject.transform.position.x == objectHit.transform.position.x)
        //        {
        //            print("Retard");
        //        }

        //    }
        //}
    }
    void CheckForHit()
    {
        RaycastHit objectHit;
    }
}
