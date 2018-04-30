using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraControllerScript : NetworkBehaviour
{
    public float offsetRotation;    //set by parent gameobject.
    public float currentRotation;   //set by mousex input.
    private float trueRotation;     //addition of the above two within the range 0-360
    public GameObject Target;       //LP: the object the camera "follows" // -JO Changed to Transform to get Parent of current gameobject allowing for camera to be linked to current unit rather than manually set.
    public float PivotOffsetMultiplier = 1.0f;
    public float RotationSensitivity;
    //using a pivot point (neck) alongside a pivot offset (head or fly-away point) lets us control the
    //camera so that looking down will look down their chest instead of their spine...
    //like how the eyes naturally moving forward when tilting your head to look down
    public Vector3 PivotPoint = new Vector3(0, 1.5f, 0);
    public Vector3 PivotOffset = new Vector3(0, 0.15f, -4);
    public Vector3 TruePivotOffset = Vector3.zero;
    private float TargetSpeed = 0;
    public Camera cam; // for cam enabling in multiplayer
    public Camera SpawnCam; // for spawncam disabling.

    public float TiltSensitivity = 0.5f;
    private float currentTilt = 50;


    private float lastMouseX, lastMouseY;
	// Use this for initialization
	void Start ()
    {
        SpawnCam = GameObject.FindGameObjectWithTag("OverworldCam").GetComponent<Camera>(); // to locate "spawn cam" thing
        GameObject ParentUnit = transform.gameObject.GetComponentInParent<UnitBehavior>().gameObject;
        //print(ParentUnit);
        Target = ParentUnit;// Sets Target to Parent Game Object
	}

    
    // Update is called once per frame
    void Update ()
    {
        if (!gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            cam.enabled = false; //disabling cam if not local player 
            //return;
        }
        else
        {
            SpawnCam.enabled = false; //disable spawn cam if local player 
            //return;
        }


        //LP: find offset from previous location and current location (speed)

        //TargetSpeed = (PivotPoint - Target.transform.position).magnitude;
        //LP: Now move our pivot point to our target gameobject's location
        PivotPoint = Target.transform.position;

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
        if (currentTilt > 80) currentTilt = 80;     //LP: if you let it tile by 90, it fucks with shit.
        if (currentTilt < 30) currentTilt = 30;   //LP: if you let it tile by -90, it also fucks with shit.
                                                    //currentRotation += InputScript.MouseX() * RotationSensitivity;                  //change. the player rotation is based on movement not camera direction.
        if (currentRotation > 360) currentRotation -= 360;                                  //logic for changing rotations.
        if (currentRotation < 0) currentRotation += 360;

        trueRotation = currentRotation + offsetRotation;
        if (trueRotation > 360) trueRotation -= 360;                                  //logic for changing rotations.
        if (trueRotation < 0) trueRotation += 360;
        transform.position = PivotPoint + Quaternion.Euler(currentTilt, trueRotation, 0) * PivotOffset * PivotOffsetMultiplier;

        transform.rotation = Quaternion.Euler(currentTilt, trueRotation, 0);
        //transform.Rotate(Vector3.up, offsetRotation);
        //transform.GetChild(0).Rotate(Vector3.up, -offsetRotation);              //move camera opposite of character.

        lastMouseX = Input.mousePosition.x;
        lastMouseY = Input.mousePosition.y;
    }
}
