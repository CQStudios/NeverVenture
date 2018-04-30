using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OverWorldControllerScript : MonoBehaviour {

    //needs to know what the main player to follow is, as well as any other players in the party.
    //clientPlayers[0] is the main player for the client.
    private List<UnitBehavior> clientPlayers;
    public UnitBehavior player;
    public int MaxPartySize = 4;
    private Vector3 targetPosition;
    private GameObject ClickEffect;
    public GameObject ClickObject;
    //needs to grab the client's camera (main camera, one per scene essentially)
    public Camera mainCamera;

    private bool mouseHeld;

    public bool MoveOnClick = true;


	// Use this for initialization
	void Start () {
        //player = gameObject.transform.GetComponentInParent<UnitBehavior>(); // added to link the camera to the parent with the unitbehavior script
        clientPlayers = new List<UnitBehavior>();
        clientPlayers.Add( player);
        //add connected players somehow????
        targetPosition = player.transform.position;
        player.SetOverworldDestination(targetPosition);
        ClickEffect = GameObject.Instantiate(ClickObject, targetPosition, Quaternion.identity);
        mouseHeld = false;
        CameraControllerOfflineScript playerCameraScript = mainCamera.GetComponent<CameraControllerOfflineScript>();
        if (playerCameraScript != null)
        {
            playerCameraScript.Target = player.gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {


        //TODO this needs to be on click, and not on press
        //On a click on the overworld we set the destination of the player
        //if (gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        if (true)
        {
            //if the mouse button (left) is held down, we update our target location if needed.
            if (Input.GetMouseButton(0))
            {
                if (!mouseHeld)
                    //on mouse click down, when not in battle, update the clickeffect location.
                    if (!player.inBattle)
                        UpdateTargetLocation();
                //we set this value last so that we can gate access to the above code. 
                //EG, if !mouseHeld, update target location
                mouseHeld = true;

            }
            else
            {
                mouseHeld = false;
            }
        }
    }
    
    
    public CanvasClickBridge MenuInterface;
    //raycasts mouse against the screen, finding the first ground object to target.
    //supports differentiation between non-floor objects, transparent objects, transparent sections of floor, and traversible floor.
    //currently skips anything that isnt traversible floor. guaranteed to find targettable floor, or do nothing at all.
    private void UpdateTargetLocation()
    {
        //if we hit UI elements, do nothing.
        if (MenuInterface.CanvasElementHit()) { return; }

        //make ray for pointer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //raycast from behind the camera. does it catch UI hits?
        ray.origin -= ray.direction.normalized * Camera.main.nearClipPlane * 2;
        //we take the raycast and sort it so that we know element 0 is the closest to the camera.
        //if (UnityEngine.UI.Grap)
        RaycastHit[] hit =  Physics.RaycastAll(ray, Mathf.Infinity);
        SortRHA(hit);
        if (hit != null)
        {
            //Debug.Log("click!");
            //loop over our hits. Ignore hits if the collision point has 0 alpha (full transparency)
            //return if point is opaque and isn't floor
            //set target location if position is floor.
            for (int i = 0; i < hit.Length; i++)
            {
                //catch UI hits.
                if (hit[i].collider.gameObject.layer == 5)
                {
                    Debug.Log("UI hit.         array location: " + (i + 1) + "/" + hit.Length + " ");
                    continue;
                }
                //ignore obstacle and minimap hits.
                if (hit[i].collider.gameObject.layer == 12 )
                {
                    //Debug.Log("Minimap Object found.         array location: " + (i + 1) + "/" + hit.Length + " ");
                    continue;
                }
                //if the hit's renderer is disabled, or it doesn't have one, ignore it
                if (hit[i].transform.GetComponent<Renderer>() == null ) 
                {
                    //Debug.Log("No renderer on this object.   array location: " + (i+1) + "/" + hit.Length + " " + hit[i].transform.name);
                    continue;
                }
                if (!hit[i].transform.GetComponent<Renderer>().isVisible && false)
                {
                    //Debug.Log("Renderer not visible on this object. array location: " + (i + 1) + "/" + hit.Length + " " + hit[i].transform.name);
                    continue;
                }

                //if our hit object has no texturecoord, the object has a basic collider, and is an item or other object. so we ignore it.
                if (hit[i].textureCoord == Vector2.zero)
                {
                    //Debug.Log("Non-Mesh collider found.      array location: " + (i + 1) + "/" + hit.Length + " " + hit[i].transform.name);
                    continue;
                }
                //if the object's texturecoord has too much transparency, ignore it too. 
                //first we must get the pixel information
                Texture2D tex = hit[i].transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
                Vector2 pixelUV = hit[i].textureCoord;
                if (tex == null)
                {
                    continue;
                }
                pixelUV.x *= tex.width;                 //multipied by the width because the example did it.
                pixelUV.y *= tex.height;
                if ( tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a <= 0.05)
                {
                    //Debug.Log("MeshCollider Has transparency Here.  array location: " + (i + 1) + "/" + hit.Length + " " + hit[i].transform.name);
                    continue;
                }

                //if we get here, the object has a mesh collider, is not transparent where we clicked, and is enabled.
                //it's incredibly likely that we've touched a floor or other mesh collider object, so we need to make sure the object is in the floor layer.
                if (hit[i].collider.gameObject.layer != 8)
                {
                    //Debug.Log("Non-floor Object found.              array location: " + (i + 1) + "/" + hit.Length + " " + hit[i].transform.name);
                    //object isn't a floor, ignoring.
                    continue;
                }
                //Debug.Log("Floor Object found.               array location: " + (i + 1) + "/" + hit.Length);
                //if we get here, it definitely is a floor we can click.
                targetPosition = hit[i].point;
                ClickEffect.transform.position = targetPosition;
                //actually call targetposition updates
                if (!player.inBattle && MoveOnClick)
                    player.SetOverworldDestination(targetPosition);
                return;

            }
        }
    }


    /// <summary>
    /// A clone of UpdateTargetLocation, but returns a vector3 from a vector3. available for public use.
    /// </summary>
    /// <param name="mouse"></param>
    /// <returns></returns>
    public Vector3 GetVector3FromRaycast(Vector3 mouse)
    {
        if (MenuInterface.CanvasElementHit()) { return Vector3.zero; }
        Ray ray = Camera.main.ScreenPointToRay(mouse);
        ray.origin -= ray.direction.normalized * Camera.main.nearClipPlane * 2;
        RaycastHit[] hit = Physics.RaycastAll(ray, Mathf.Infinity);
        SortRHA(hit);
        if (hit != null)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].collider.gameObject.layer == 5) {continue; }
                if (hit[i].collider.gameObject.layer == 12){  continue;}
                if (hit[i].transform.GetComponent<Renderer>() == null) { continue;  }
                if (!hit[i].transform.GetComponent<Renderer>().isVisible && false)  { continue;   }
                if (hit[i].textureCoord == Vector2.zero) { continue;  }
                Texture2D tex = hit[i].transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
                Vector2 pixelUV = hit[i].textureCoord;
                if (tex == null)                {                    continue;                }
                pixelUV.x *= tex.width;                 //multipied by the width because the example did it.
                pixelUV.y *= tex.height;
                if (tex.GetPixel((int)pixelUV.x, (int)pixelUV.y).a <= 0.05){ continue;                }

                if (hit[i].collider.gameObject.layer != 8) { continue;  }
               
                return hit[i].point;
            }
        }
        return Vector3.zero;

    }


    //sort the fucking array based on distance
    private void SortRHA(RaycastHit[] R)
    {
        System.Array.Sort(R, (x, y) => x.distance.CompareTo(y.distance));
    }


    public UnitBehavior getPlayer()
    {
        return player;
    }
}
