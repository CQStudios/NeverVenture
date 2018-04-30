using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraControllerOfflineScript : MonoBehaviour
{
    public float offsetRotation;    //set by parent gameobject.
    public float currentRotation;   //set by mousex input.
    private float trueRotation;     //addition of the above two within the range 0-360
    public GameObject Target, Target2;       //LP: the object the camera "follows" // -JO Changed to Transform to get Parent of current gameobject allowing for camera to be linked to current unit rather than manually set.
    public float PivotOffsetMultiplier = 1.0f;
    public float RotationSensitivity;
    public float BattleDistMultiplier = 1.5f;
    private bool inBattle;
    //using a pivot point (neck) alongside a pivot offset (head or fly-away point) lets us control the
    //camera so that looking down will look down their chest instead of their spine...
    //like how the eyes naturally moving forward when tilting your head to look down
    public Vector3 PivotPoint = new Vector3(0, 1.5f, 0);
    public Vector3 PivotOffset = new Vector3(0, 0.15f, -4);
    public Vector3 TruePivotOffset = Vector3.zero;
    public int MaxTilt, MinTilt;
    public float TargetSpeed = 0.2f; //how fast we transition our pivotpoint to match the new pivotpoint location.

    public float TiltSensitivity = 0.5f;
    private float currentTilt = 50;


    private float lastMouseX, lastMouseY;
	// Use this for initialization
	void Start ()
    {
        //TODO have a case where if the parent object's root has a network identity, work off of the parent, otherwise do nothing.
        //used when script is on a camera attached to a specific object.    //GameObject ParentUnit = transform.gameObject.GetComponentInParent<UnitBehavior>().gameObject;
        //print(ParentUnit);
        //again only used in online version // Target = ParentUnit;// Sets Target to Parent Game Object
	}

    
    // Update is called once per frame
    void LateUpdate ()
    {
        //make sure targets exist. 
        if (Target2 == null)
            Target2 = Target;
        if (Target == null)
            return;
        Vector3 dist = Target.transform.position - Target2.transform.position;


        //LP: find offset from previous location and current location (speed)

        //TargetSpeed = (PivotPoint - Target.transform.position).magnitude;
        //LP: Now move our pivot point to our target gameobject's location
        PivotPoint = Vector3.MoveTowards(PivotPoint,(Target.transform.position - dist/2), TargetSpeed);

        //LP: Could bring the camera farther away the faster we go (feature?)
        //TruePivotOffset.Set(PivotOffset.x,PivotOffset.y,PivotOffset.z - TargetSpeed);
        //Debug.Log(TargetSpeed);

        //LP: essentially, The forward vector of the camera *must not* equal the normal vector of the horizontal plane.
		
        

        if (Input.GetMouseButton(1))
        {
            currentRotation += (Input.mousePosition.x - lastMouseX) * RotationSensitivity;
            currentTilt -= (Input.mousePosition.y - lastMouseY) * TiltSensitivity;
        }
        //currentTilt -= InputScript.MouseY() * TiltSensitivity;
        if (currentTilt > MaxTilt) currentTilt = MaxTilt;     //LP: if you let it tile by 90, it fucks with shit.
        if (currentTilt < MinTilt) currentTilt = MinTilt;   //LP: if you let it tile by -90, it also fucks with shit.
                                                    //currentRotation += InputScript.MouseX() * RotationSensitivity;                  //change. the player rotation is based on movement not camera direction.
        if (currentRotation > 360) currentRotation -= 360;                                  //logic for changing rotations.
        if (currentRotation < 0) currentRotation += 360;

        trueRotation = currentRotation + offsetRotation;
        if (trueRotation > 360) trueRotation -= 360;                                  //logic for changing rotations.
        if (trueRotation < 0) trueRotation += 360;
        if (inBattle) //control distance away from targets the camera is.
        {
            PivotOffsetMultiplier = BattleDistMultiplier;
        }
        else
        {
            PivotOffsetMultiplier = 1.0f;
        }
        transform.position = PivotPoint + Quaternion.Euler(currentTilt, trueRotation, 0) * PivotOffset * PivotOffsetMultiplier;
        
        transform.rotation = Quaternion.Euler(currentTilt, trueRotation, 0);
        //transform.Rotate(Vector3.up, offsetRotation);
        //transform.GetChild(0).Rotate(Vector3.up, -offsetRotation);              //move camera opposite of character.

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }

    public void SetBattle(bool l)
    {
        inBattle = l;
    }

    public bool GetBattle()
    {
        return inBattle;
    }
}
