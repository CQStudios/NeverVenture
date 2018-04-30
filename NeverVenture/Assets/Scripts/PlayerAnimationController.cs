using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {
    public UnitBehavior myunit;
    public Animator myanim;
	// Use this for initialization
	void Start () {
        //obtain unitbehavior from parent object if it is not already set.
        if (myunit == null)
        {
            myunit = gameObject.transform.parent.GetComponent<UnitBehavior>();
        }
        //obtain the model's animator if not already set.
        if (myanim == null)
        {
            myanim = gameObject.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    private Vector3 dist;
	void Update () {
        //change the animation state's parameters based on unitbehavior information.
        dist = myunit.GetDestDifference();
        if (dist.magnitude > myunit.WtR && myunit.isMoving)  //running in this case, as nothing blocks our path.
        {
            myanim.SetBool("Moving", true);
            myanim.SetFloat("Velocity Z", 1);
            //myanim.speed = myunit.moveSpeed * myunit.speedmultiplier / 2.0f;
        }
        else if (dist.magnitude > 0.01f && myunit.isMoving)    //walking in this case, as we are approaching an obstacle or about to hit our destination.
        {
            myanim.SetBool("Moving", true);
            myanim.SetFloat("Velocity Z", myunit.speedmultiplier );
            //myanim.speed = myunit.moveSpeed * myunit.speedmultiplier /2.0f;
        }
        else
        {
            myanim.SetBool("Moving", false);
            myanim.SetFloat("Velocity Z", 0);

            //myanim.speed = 1;
        }
	}
}
