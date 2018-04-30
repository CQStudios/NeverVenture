using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBehavior : MonoBehaviour {
    
    private Camera minimapcam;
    private CameraControllerOfflineScript minimapcamcontroller;
    private WFloor floor;
    private GameObject TargetDisplay;

    private bool hasGivenColors = false;


    private int second = 0;
    public Color _Unrevealed, _Revealed, _Visited, _NotCleared;
    public Material mat;
    // Use this for initialization
    void Start () {  
        TargetDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
        TargetDisplay.transform.forward = -Vector3.up;
        TargetDisplay.name = "MinimapTargetDisplay";
        TargetDisplay.layer = 12;   //only for minimap display
        TargetDisplay.GetComponent<Renderer>().material = mat;
        TargetDisplay.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

    }

    // Update is called once per frame
    void Update()
    {
        if (minimapcamcontroller != null)
        {
            //set minimap's target to the main camera's player object.
            minimapcamcontroller.Target = Camera.main.GetComponent<CameraControllerOfflineScript>().Target;
            TargetDisplay.transform.position = Camera.main.GetComponent<CameraControllerOfflineScript>().Target.transform.position + Vector3.up*(1.1f);
            //set minimap's target2 to the main camera's 2nd target.
            minimapcamcontroller.Target2 = Camera.main.GetComponent<CameraControllerOfflineScript>().Target2;
            minimapcam.backgroundColor = _Unrevealed;
        }
        else
        {
            //get the minimap camera
            minimapcam = GameObject.Find("MinimapCamera").GetComponent<Camera>();
            //get that camera's control script
            minimapcamcontroller = minimapcam.GetComponent<CameraControllerOfflineScript>();
        }
        /*
        //when the floor is finished creating itself, apply color changes
        if (floor != null) {
            if (!hasGivenColors & floor.FloorSize == floor.PathSize )
            {
                //set the scale of our target display
                TargetDisplay.transform.localScale = new Vector3(floor.roomWidth/4, floor.roomWidth/4, floor.roomWidth/4);
                hasGivenColors = true;
                foreach (WRoom o in floor.map)
                {
                    o._Unrevealed = _Unrevealed;
                    o._Revealed = _Revealed;
                    o._NotCleared = _NotCleared;
                    o._Visited = _Visited;
                    o.minirend.material = mat; 
                }
            }
        }
        else
        {
            //get the floor.
            floor = GameObject.Find("DungeonController").GetComponent<DungeonManager>().myfloor;
        }
        */
    }
}
