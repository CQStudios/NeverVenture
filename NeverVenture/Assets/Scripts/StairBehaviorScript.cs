using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



// attach to a gameobject. Acts as a proximity sensor does, requesting to move to a target floor when touched.
// must be given GameObject and dungeonManager to begin functioning.


public class StairBehaviorScript : MonoBehaviour {

    public GameObject playerObject;
    public DungeonManager parent;
    public int targetFloor = 0;
    public string option = "";
    public GameObject ProceedButton;
    private bool enablebutton = false;
    
    // Use this for initialization
	void Start () {
        ProceedButton = GameObject.Instantiate(Resources.Load<GameObject>("CanvasButtonObject"), Vector3.zero, Quaternion.identity, GameObject.Find("UICanvas").transform);
        ProceedButton.GetComponent<Button>().onClick.AddListener(OnClickFunction);
        ProceedButton.transform.SetAsFirstSibling();
        //Debug.Log("StairbehaviorScript started.~~~~~~~~~");
        Update2();
    }

    private void OnClickFunction()
    {
        parent.RequestRoomChange(targetFloor, option);
        enablebutton = false;
        ProceedButton.SetActive(false);
    }
    
    
    // Update is called once per frame
    public void Update2 () {
        //shortcircuit without a player to handle, or a dungeon to be part of.
        if (playerObject == null || parent == null )
        {
            Debug.Log("Stair Behavior ShortCicuit.... no parent or no player given.~~~~~~~~~");
            return;
        }
        Vector3 playerdistvect = playerObject.transform.position - gameObject.transform.position;
        if (playerdistvect.magnitude < 2.0f)
        {
            if (ProceedButton != null)
            {
                Vector3 raw = Camera.main.WorldToScreenPoint(transform.position);
                ProceedButton.transform.position = raw + new Vector3(0, -ProceedButton.GetComponentInChildren<Text>().rectTransform.rect.height * 1.0f, 0);
                enablebutton = true;
                //Debug.Log("pushing button position.");
            }
        }
        else
        {
            //Debug.Log("Stair disabled button (update2)");
            enablebutton = false;
        }

        if (ProceedButton != null)
        {
            ProceedButton.SetActive(enablebutton);
        }
    }



    //overload monobehavior functions.
    void OnDestroy()
    {
        parent = null;
        playerObject = null;
        if (ProceedButton != null)
            Destroy(ProceedButton);
    }

    private void OnDisable()
    {
        if (ProceedButton != null)
        {
            enablebutton = false;
            ProceedButton.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (ProceedButton != null)
        {
            ProceedButton.SetActive(enablebutton);
        }
    }

    public void SetPlayer(GameObject plyr)
    {
        playerObject = plyr;
    }

    public void SetDungeonManager( DungeonManager d)
    {
        parent = d;
    }


}
