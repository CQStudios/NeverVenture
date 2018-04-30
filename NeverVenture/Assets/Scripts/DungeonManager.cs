using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

public class DungeonManager: MonoBehaviour
{
    public int second;

    [Header("Floor Arguments")]
    public int TotalFloors, CurrentFloor;
    public float RoomWidth = 6;
    public int MinRooms = 3;
    public int MaxRooms = 6;
    public float PerFloorRoomScaling = 1.1f; //for every floor, the min and max sizes scale by 10%   
    public string ThemePath;
    public GameObject roomprefab;
    public GameObject playerObject;

    private Vector3 playeroverworldspawnpoint;
    public float VisionLength;
    public Texture2D roomTexture;
    public WFloor myfloor;
    public WRoom target = null;
    public List<Vector2> floorSizes;
    private List<WFloor> floors = new List<WFloor>();
    private bool started = false, playerHasBeenPlaced = false;
    public Color CurrentRoomColor, ConnectedRoomColor, UnvisibleColor;
    public bool[] Completed = { false, false, false };

    private int requestedroom;
    private GameObject ProceedButton;
    private bool isNewFloor = false;
    public BattleControlScript globalbattlecontrol;

    //called when the scene loads.
    // instantiates the dungeon's first floor, no rooms, no models.
    void Start(){
        //GetStatsFromSave();
        CurrentFloor = -1;
        second = (int)Time.time;
        myfloor = new WFloor();
        target = null;
        myfloor.Yoffset = -20 * (CurrentFloor + 1);
        myfloor.roomWidth = RoomWidth;
        myfloor.minSize = MinRooms;
        myfloor.maxSize = MaxRooms;
        myfloor.ThemePath = ThemePath;
        myfloor.Start(roomprefab);
        //
        //generate the floor's first room, without it's model.
        GenerateNext();
        started = false;
        playerHasBeenPlaced = false;
        ProceedButton = GameObject.Instantiate(Resources.Load<GameObject>("CanvasButtonObject"), Vector3.zero, Quaternion.identity, GameObject.Find("UICanvas").transform);
        //(GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/p10refabs/CanvasButtonObject.prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity, GameObject.Find("UICanvas").transform);
        ProceedButton.GetComponent<Button>().onClick.AddListener(OnClickEnterFloor);
        ProceedButton.transform.SetAsFirstSibling();
    }

    private void OnClickEnterFloor()
    {
        playeroverworldspawnpoint = playerObject.transform.position;
        EnterFloor(0);
    }

    /// <summary>
    /// Generates the specified floor level by walking the floor, then assigning models.
    /// Assumes that the floor exists in our "floors" object to load from, otherwise generates a new one.
    /// </summary>
    /// <param name="floorlevel"></param>
    public void EnterFloor(int floorlevel)
    {

        playerObject.GetComponent<UnitBehavior>().SetOverworldDestination(Vector3.zero);
        Debug.Log("enterring floor " + floorlevel + " of " + TotalFloors);
        started = true;
        myfloor.RandomAssignDoorModelsShort = false;
        GenerateNextShortCircuit = false;
        //if we have previous floors to deal with, save them, then destroy their models.
        if (CurrentFloor >= 0)
        {
            //save the floor we are on (put it inside the floors list) 
            //TODO we do this when we generate the floor. do we really need to do it again?
            //if (CurrentFloor < floors.count)
            //use currentfloor for deleting old floor models.
            Debug.Log("DESTROYING A FLOOR~~~~~~~~~~~~~~~");
            foreach (WRoom o in floors[CurrentFloor].map)
            {
                o.DestroyModels();
                //also completely remove 
                o.DisposeDeadContents();                
            }   
        }
        //otherwise we are new and we can assume we had no previous floors
        else
        {
            //if we don't have a label on the player, add one.
            if (playerObject.GetComponent<HealthOverTarget>() == null)
            {
                playerObject.AddComponent<HealthOverTarget>();
            }
        }

        CurrentFloor = floorlevel;
        //if we request a floor greater than our total number of floors, reset our character in the overworld, undo our started aspect, and delete our floors list. 
        //if the requested floor exists, load it. (there are no models, it needs to reassign models.)
        //else begin generating a new floor.
        if (CurrentFloor >= TotalFloors || CurrentFloor <0)
        {
            /*
            //if we have this tag element, remove it.
            if (playerObject.GetComponent<HealthOverTarget>() != null)
            {
                HealthOverTarget n = playerObject.GetComponent<HealthOverTarget>();
                Destroy(n);
            }
            */
            playerObject.transform.position = playeroverworldspawnpoint;
            playerObject.GetComponent<UnitBehavior>().SetOverworldDestination(playerObject.transform.position);
            started = false;
            foreach(WFloor f in floors)
            {
                f.DestroyFloor();
            }
            floors.Clear();
            //Destroy(floors);
            floors = new List<WFloor>();
            CurrentFloor = 0;
            Start();
            playerHasBeenPlaced = true;
            GameObject.Find("Camera").GetComponent<CameraControllerOfflineScript>().Target2 = null;
            Debug.Log("returning to overworld");
            System.GC.Collect();
            //return player to entrance?
            //return;
        }
        // requested floor exists and is not < 0.
        else if (CurrentFloor < floors.Count)
        {
            //GenerateNextShortCircuit = true;
            myfloor = floors[CurrentFloor];
            myfloor.Yoffset = -20 * (CurrentFloor + 1);
            myfloor.CreateRoomModels(myfloor.ThemePath);
            myfloor.Update();
            playerHasBeenPlaced = false;
        }
        // floor does not exist, must be made.
        else
        {
            myfloor = new WFloor();
            myfloor.roomWidth = RoomWidth;
            myfloor.minSize = (int)(MinRooms * Mathf.Pow(PerFloorRoomScaling , CurrentFloor));
            myfloor.maxSize = (int)(MaxRooms * Mathf.Pow(PerFloorRoomScaling , CurrentFloor));
            myfloor.ThemePath = ThemePath;
            myfloor.Yoffset = -20 * (CurrentFloor + 1);
            myfloor.Start(roomprefab);
            isNewFloor = true;
            loadtest = 0; //counts number of times createpath gets called
            GenerateNext();
            floors.Add(myfloor);
            playerHasBeenPlaced = false;
        }
        //Debug.Log("setting player's location to the first room in the map.");
        //playerObject.transform.position= myfloor.map[0].model.transform.position;
    }


    //populates the map array and instantiates the floor terrain/room divisions.
    /// <summary>
    /// returns true if it generated a room, returns false after all rooms are made and the models are made.
    /// </summary>
    /// <returns></returns>

    private bool GenerateNextShortCircuit = false;
    private int loadtest = 0;
    public bool GenerateNext()
    {
        if (myfloor.FloorSize < myfloor.PathSize)
        {
            myfloor.CreatePath();
            //Debug.Log("GenerateNext did createpath(): " + loadtest);
            loadtest++;
            return true;
        }
        else
        {
            if (!GenerateNextShortCircuit)
            {
                if (isNewFloor)
                {
                    //last room was created last frame, so add the next stair case
                    PlaceStairs();
                    //after stairs are created, craft enemies at 30% chance of having enemies per room, with mobs between min and max.
                    PlaceEnemies(rate: 0.9f, min: 1, max: 5);
                }
                //let the floor produce the room models.
                //if webgl use this
                //myfloor.CreateRoomModelsWebGL(roomTexture);
                //for pc use this
                myfloor.CreateRoomModels(myfloor.ThemePath);
                //let the floor do it's first update() run (fixes locations of rooms)
                myfloor.Update();
                //Debug.Log("RandomAssignCalled~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                myfloor.RandomAssignDoorModels();
                GenerateNextShortCircuit = true;
                isNewFloor = false;
            }
            //let the floor do it's first update() run (fixes locations of rooms)
            //myfloor.Update();
            //playerObject.GetComponent<UnitBehavior>().SetOverworldDestination(Vector3.zero);
            return false;
        }
    }
    
    
    /// <summary>
    /// Places two stairs in the center of the start room and the end room.
    /// ASSUMES stairs must be dropped in only the first and last rooms.
    /// </summary>
    private void PlaceStairs()
    {
        //don't build stairs if we already built stairs for this floor in the first or last rooms.
        foreach(GameObject potential in myfloor.map[0].Contents)
        {
            if (potential.GetComponent<StairBehaviorScript>() != null)
                return;
        }
        foreach (GameObject potential in myfloor.map[myfloor.map.Count -1].Contents)
        {
            if (potential.GetComponent<StairBehaviorScript>() != null)
                return;
        }

        //generate the first stair.
        //GameObject stairball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject stairball;
        if (CurrentFloor == 0)
        {
            stairball = GameObject.Instantiate(Resources.Load<GameObject>("RopeStair"));
        }
        else
        {
            stairball = GameObject.Instantiate(Resources.Load<GameObject>("StairsUp"));
        }
        stairball.gameObject.AddComponent<StairBehaviorScript>();
        StairBehaviorScript stair = stairball.gameObject.GetComponent<StairBehaviorScript>();
        stair.SetPlayer(playerObject);
        stair.targetFloor = CurrentFloor - 1;
        stair.option = "last";
        stair.SetDungeonManager(this);
        stair.transform.position = myfloor.map[0].Location + new Vector3 (3,0,2);
        myfloor.map[0].Contents.Add(stairball);

        //generate the second stair
        //GameObject stairball2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject stairball2 = GameObject.Instantiate(Resources.Load<GameObject>("StairsDown"));
        stairball2.gameObject.AddComponent<StairBehaviorScript>();
        StairBehaviorScript stair2 = stairball2.gameObject.GetComponent<StairBehaviorScript>();
        stair2.SetPlayer(playerObject);
        stair2.targetFloor = CurrentFloor + 1;
        stair2.SetDungeonManager(this);
        stair2.transform.position = myfloor.map[myfloor.map.Count - 1].Location + new Vector3(3,0,2);
        //stairball2.GetComponent<Renderer>().material.color = new Color(0.0f, 0.9f, 0.6f);
        myfloor.map[myfloor.map.Count - 1].Contents.Add(stairball2);

    }
    

    private void PlaceEnemies(float rate, float min, float max, float doublerate = 0.0f)
    {
        //loop over the middle rooms in the floor.
        for (int i = 1; i < myfloor.map.Count - 1; i++)
        {
            //if rng value is inside the rate, put some enemies down.
            if (Random.value < rate)
            {
                //choose how many enemies to place, optionally having a percent chance to increase mobsize by 1.5.
                int mobsize = (int)(Random.value * (max - min) + min);
                if (Random.value < doublerate) { mobsize += (int)(mobsize * 1.5); }
                for (int enemynumber = 0; enemynumber < mobsize; enemynumber++) {
                    //add gameobjects to room contents, randomly place them.
                    myfloor.map[i].Contents.Add(CreateEnemy());
                    RandomPlace(myfloor.map[i].Contents[myfloor.map[i].Contents.Count - 1], myfloor.map[i]);
                }
            }
        }
    }

    private GameObject CreateEnemy()
    {
        GameObject t = GameObject.Instantiate(Resources.Load<GameObject>("SkeletonSword"));
        //random rotations
        t.transform.forward = new Vector3(Random.value - 0.5f, 0, Random.value -0.5f).normalized;
        t.name = "Spoopy Skeltal";
        //give obstacle layer temporarily.
        t.layer = 10;
        t.AddComponent<UnitBehavior>();
        t.AddComponent<EnemyBehaviour>();
        t.GetComponent<UnitBehavior>().moveSpeed = 2.0f;
        //t.GetComponent<UnitBehavior>().name = "lol";
        t.GetComponent<EnemyBehaviour>().Target = playerObject.gameObject;
        return t;
    }

    /// <summary>
    /// randomly place an object "o"s position within the constraints of the room "rm" given
    /// </summary>
    /// <param name="o"></param>
    /// <param name="rm"></param>
    private void RandomPlace(GameObject o, WRoom rm)
    {
        float width = 0.8f;
        Vector3 cen = rm.Location, right = Vector3.right, forward = Vector3.forward;
        float dx = (Random.value* width - width / 2.0f); //-0.4 to 0.4
        float dz = (Random.value* width - width / 2.0f); //-0.4 to 0.4
        o.transform.position = cen + RoomWidth * (dx * right + dz * forward);
    }

    /// <summary>
    /// Sets the target room object the player is currently in. sets null if the player is not in a room in the floor.
    /// </summary>
    /// <returns></returns>
    private void GetRoomPlayerIsIn()
    {
        target = null;
        //find the room the player is over.
        RaycastHit hit;
        if (Physics.Raycast(playerObject.transform.position + playerObject.transform.up / 2.0f, -playerObject.transform.up, out hit, Mathf.Infinity, 1 << 8))
        {
            //make a target room and make it the same room the player is in.
            target = myfloor.map[0];
            foreach (WRoom o in myfloor.map)
            {
                //o.SetVisionColor(UnvisibleColor);
                //o.SetActive(false);
                if (o.model == null)
                    continue;
                if (hit.transform.position == o.model.transform.position)
                {
                    //this is the room the player is in.
                    target = o;
                }
            }
        }
    }
    
    /// <summary>
    /// pushes contents of "target" room with unitbehaviors to the battle controller
    /// </summary>
    private void SendCombatantsToBattleController()
    {
        //create list to send away to battlecontroller
        List<UnitBehavior> units = new List<UnitBehavior>();
        if (target == null || target.Contents == null) {
            globalbattlecontrol.SetCombatants(units);
            return;
        }

        

        foreach ( GameObject o in target.Contents)
        {
            UnitBehavior c = o.GetComponent<UnitBehavior>();
            //only publish alive units from the room's contents.
            if (c != null && !c.isDead)
            {
                units.Add(c);
            }
        }
        if (playerObject.GetComponent<UnitBehavior>() != null )
        {
            if (units.Count > 0) //if there are combatants, add the player before sending it out.
            {
                units.Add(playerObject.GetComponent<UnitBehavior>());
            }
            else //otherwise make sure the playerobject is not stuck in battle.
            {
                playerObject.GetComponent<UnitBehavior>().inBattle = false;
            }
        }

        globalbattlecontrol.SetCombatants(units);
    
    }

    /// <summary>
    /// changes color of the room's roof object based on relative position.
    /// also changes the floor flags for the minimap class to use
    /// </summary>
    void UpdateRoofing()
    {
        if (target == null || target.model == null)
        {
            //Debug.Log("target or target model is null, so no roof to update.");
            return;
        }
        foreach (WRoom o in myfloor.map)
        {
            o.SetVisionColor(UnvisibleColor);
            o.SetActive(false);
        }

        //now we have a target, make the target room's roof clear, and the connection rooms semi-clear
        target.SetActive(true);
        //target.model.transform.Find("Roof").GetComponent<Renderer>().material.SetColor("_MainColor", CurrentRoomColor);
        target.SetVisionColor(CurrentRoomColor);
        //set hasbeenrevealed for this object which the player is now in.
        target.hasBeenRevealed = true;
        //set hasbeenvisited for this object which the player is now in.
        target.hasBeenVisited = true;
        CameraControllerOfflineScript cam = GameObject.Find("Camera").GetComponent<CameraControllerOfflineScript>();
        cam.Target2 = target.model;
        UnitBehavior unit = playerObject.GetComponent<UnitBehavior>();
        if (unit != null)
        {
            cam.SetBattle(unit.inBattle); //lets the camera know whether to zoom out or not.
        }
        for (int i = 0; i < 4; i++)
        {
            if (target.connections[i] != null)
            {
                //target.connections[i].model.transform.Find("Roof").GetComponent<Renderer>().material.SetColor("_MainColor", ConnectedRoomColor);
                target.connections[i].SetActive(true);
                target.connections[i].SetVisionColor(ConnectedRoomColor);
                //since this connected room is partially revealed, reflect this in the map.
                target.connections[i].hasBeenRevealed = true;
            }
        }
        
    }
    
    /// <summary>
    /// calls Update2() on the objects in the the room's contents list.
    /// </summary>
    void UpdateRoomContents()
    {
        if (target == null)
        {
            //Debug.Log("target is null, thus no contents in room to update.");
            return;
        }
        foreach( WRoom other in target.connections)
        {
            foreach (GameObject c in target.Contents)
            {
                if (c.GetComponent<StairBehaviorScript>())
                {
                    c.GetComponent<StairBehaviorScript>().Update2();
                }
            }
        }
        GameObject cd;
        for (int i = 0; i < target.Contents.Count; i++)
        {
            cd = target.Contents[i];
            if (cd.GetComponent<StairBehaviorScript>())
            {
                cd.GetComponent<StairBehaviorScript>().Update2();
            }
        }
    }

    public void RequestRoomChange(int rmnm, string option = "")
    {
        //if different from the current floor, we'll have to move.
        if (rmnm != CurrentFloor)
        {
            EnterFloor(rmnm);
            requestedroom = 0;
        }
        if (option.ToLower().Contains("last") && CurrentFloor >= 0)
        {
            //dDebug.Log("Request asked to be put in last room on list.");
            playerObject.transform.position = myfloor.map[myfloor.map.Count - 1].Location + Vector3.right;
            requestedroom = myfloor.map.Count - 1;
        }
    }

    void Update()
    {
        bool movePlayerToFirstFloor = false;
        //start the dungeon if the player gets to the dungeon entrance.
        Vector3 playerdistvect = playerObject.transform.position - gameObject.transform.position;
        
        if (playerdistvect.magnitude < 9)
        {
            if (ProceedButton != null) {
                ProceedButton.SetActive(true);
                Vector3 raw = Camera.main.WorldToScreenPoint(transform.position);
                ProceedButton.transform.position = raw + new Vector3(0, -ProceedButton.GetComponentInChildren<Text>().rectTransform.rect.height * 1.0f, 0);
            }
        }
        else
        {
            if (ProceedButton != null )
                ProceedButton.SetActive(false);
        }
        //we only want to begin generating floors if this dungeon has been started, and only if this floor reference isnt full.
        if (myfloor.FloorSize <= myfloor.PathSize && started)
        {
            //will build one room per frame, and then set their models and locations.
            //when this returns false, then the rooms are ready to use.
            if (!GenerateNext() && !playerHasBeenPlaced)
                movePlayerToFirstFloor = true;
            //return;
        }
        else if (started && movePlayerToFirstFloor) //room is fully built and it is started
        {   //if the room is inside our list of floors, update that list, otherwise add it
            if (CurrentFloor < floors.Count)
                floors[CurrentFloor] = myfloor;
            else
                floors.Add(myfloor);
        }
        
        //call update roofing a few times every second along with the retargetting method (raycast caller)
        //call other non-immediate functions a couple times every second too.
        if (Time.time * 4 > second && started)
        {
            second += 1;
            GetRoomPlayerIsIn();
            SendCombatantsToBattleController();
            UpdateRoofing();
        }
        
        //let rooms adjust their position/shape.
        myfloor.Update();
        //draw those connections between rooms.
        myfloor.DrawConnections();
        //for each object/script in the contents list, execute their update method.
        UpdateRoomContents();


        if (movePlayerToFirstFloor)
        {
            //Debug.Log("moving player to room "+ requestedroom +" of floor " + CurrentFloor + " out of " + TotalFloors);
            //Debug.Log("room is: " + myfloor.map[0].model.transform.position);
            playerObject.transform.position = myfloor.map[requestedroom].model.transform.position;
            playerHasBeenPlaced = true;
            playerObject.GetComponent<UnitBehavior>().SetOverworldDestination(playerObject.transform.position);
        }
        
        
        //traversal condition. once all rooms have been seen, we go one more down
        /*
        bool goToNextFloor = true;
        if (myfloor.map.Count < 1)
            goToNextFloor = false;
        foreach(WRoom o in myfloor.map)
        {
            if (!o.hasBeenVisited)
                goToNextFloor = false;
        }
        if (started && goToNextFloor)
        {
            EnterFloor(CurrentFloor + 1);
            goToNextFloor = false;
        }
        */



    }
}



public enum Dir { north, east, south, west };       //defines the cardinal directions.
public enum CXN { single, corner, hall, t, quad }; //defines connection types for rooms.
/*
     Floor Object

Map as array of room objects
Size of Floor as integer
Path Size as integer
Minimum Size as integer
Maximum Size as integer
Center as Coordinate
Current Position(Middle) as coordinate (Location)
Current Direction(North) as string or integer 0 through 3(0 as North, 2 as South)
*/

public class WFloor{
    
    public List<WRoom> map;                                 //holds references to our rooms.
    public Texture2D miniMap = new Texture2D(600, 420);     //our rooms will be drawn on this to provide an idea of how big it is.
    private WRoom lastVisitedRoom;
    public List<Vector3> roomLocations;                     //holds references to our room locations.
    public int FloorSize = 0, PathSize, minSize = 10, maxSize = 15;
    public Dir direction;    //direction compares with Dir
    public Vector3 Center, Location;
    public string ThemePath;
    private GameObject RoomPrefab;
    public float roomWidth = 6, Yoffset = 0;

    // we might want to change out the start function with the constructor, as this is not a monobehavior
    // Start will generate the origin room, but will not create the models for it's rooms.
    // This requires that minSize and maxSize are defined.
    public void Start(GameObject prefab) {
        PathSize = minSize + (int)(Random.value * (maxSize - minSize));

        map = new List<WRoom>();
        roomLocations = new List<Vector3>();
        Center = Vector3.zero;
        Location = Center;
        direction = Dir.north;
        RoomPrefab = prefab;
        //Debug.Log(RoomPrefab);
        RoomPrefab.transform.localScale = new Vector3(roomWidth / 2.0f, 0.05f, roomWidth / 2.0f);
        //generate room 1
        WRoom origin = new WRoom(RoomPrefab) {
            Location = Location
        };
        //To force it to load it's model into memory, we need to call WRoom.Start(). Only need to use this if we require the floor to be used.
        //origin.Start();
        //add the location of the room to our list of locations.
        roomLocations.Add(origin.Location);
        //add the room to our map, which is just a list of rooms.
        map.Add(origin);
        //make sure the walker functionality knows where it came from.
        lastVisitedRoom = origin;
	}
	
    //TODO make  rooms instantiate models via InstantiateModels function TODO

    /// <summary>
    /// for each room object, load the models that need to be loaded. This at base level means generating the floor tile, walls/doors, and open areas.
    /// later on this would also mean placing obstacles where they were (we'll have a list for this.) and enemies.
    /// </summary>
    public void InstantiateModels()
    {
        foreach (WRoom o in map)
        {
            o.Start();
        }
    }

    /// <summary>
    /// At base level, for each room object delete the model associated with the room.
    /// </summary>
    public void DeleteModels()
    {
        foreach (WRoom o in map)
        {
            //destroy the model.
            o.DestroyModels();
            //GameObject.Destroy(o.model);
            //GameObject.Destroy(o.minimapmodel);
            o.model = null;
            //o.minimapmodel = null;
            o.DisposeAllContents();
            //o.DisposeDeadContents();
        }
    }

    public void DestroyFloor()
    {
        DeleteModels();
    }
    // Update is called once per frame
    private long[] timeper = new long[100]; //{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private int timeperindex = 0;
    private long lasttime = 0;
	public void Update () {
        timeperindex = (timeperindex +1 ) % timeper.Length;
        timeper[timeperindex] = System.DateTime.Now.Ticks - lasttime;
        lasttime = System.DateTime.Now.Ticks;
        long sum = 0;
        foreach (long num in timeper)
        {
            sum += num;
        }
        float T = (float)(10000000.0f / (sum / timeper.Length));
        GameObject.Find("UICanvas/fpstext").GetComponent<Text>().text = T.ToString();
        foreach (WRoom o in map)
        {
            //update all the rooms (eg let them move their models to their data locations)
            o.Location.y = Yoffset;
            o.Update();
        }
    }


    public void MoveForward(Dir orientation)
    {

        switch (orientation)
        {
            case Dir.north:
                Location.z += roomWidth;
                break;
            case Dir.south:
                Location.z -= roomWidth;
                break;
            case Dir.west:
                Location.x -= roomWidth;
                break;
            case Dir.east:
                Location.x += roomWidth;
                break;
            default:
                Debug.Log("MoveForward failure. orientation mis-set");
                break;

        }

    }

    /*
     * Function for Path Creation(Floor Object)
 If Floor Object’s Size of Floor is less than Path Size
    From Current Position
        Choose One
        -Go Right
            Change Direction right by 90 degrees
            Move Forward
        -Go Left
            Change Direction left by 90 degrees
            Move Forward
        -Go Forward
            Move Forward
        Add 1 to Size of Floor
        Add Room at Current Position with Orientation and connection opposite of orientation
*/

        /// <summary>
        /// Call to create room objects in our map. The map doesn't necessitate that we need a model yet.
        /// Moves the walker one step in a random direction, generating a room if necessary
        /// </summary>
    public void CreatePath()
    {
        if (FloorSize < PathSize)
        {
            //obtain a new direction that can be anything but the direction we came from
            direction = (Dir)( ((int)direction + ( (int)(Random.value * 3) % 3) +3 )% 4); // takes a random 0.0 - 1.0 value, multiplies by 3, then makes sure it lies within 0-2, and drops it by 1 (then adds 4 to prevent negatives.).
            //make sure that the lastvisitedroom reflects the room we were on before moving away.
            lastVisitedRoom = map[roomLocations.IndexOf(Location)];
            MoveForward(direction);
            // if there is no room here, make one, else establish a connection if there is not one.
            if (roomLocations.IndexOf(Location) == -1)
            {
                //make the room
                WRoom obj = new WRoom(RoomPrefab);
                obj.ThemePath = ThemePath;
                //make the room exist at this location
                obj.Location = Location;
                //add the room to our list, and add the location to our list.
                roomLocations.Add(obj.Location);
                map.Add(obj);

                //add a connection from this room to the previous room and vice-versa
                //Debug.Log("1.   lastvisited adds other");
                lastVisitedRoom.CreateConnection(obj, direction);
                //Debug.Log("1.   other adds lastvisited");
                obj.CreateConnection(lastVisitedRoom, (Dir)(((int)direction + 2) % 4) );
                //Debug.Log("1. done, " + dit);
                FloorSize += 1;
            }
            else  // there is a room at this location, give the room a connection to the previous room (if there wasn't one before. wroom handles for that)
            {
                WRoom obj = map[roomLocations.IndexOf(Location)];
                //add a connection from this room to the previous room and vice-versa
                //Debug.Log("2.   lastvisited adds other");
                lastVisitedRoom.CreateConnection(obj, direction);
                //Debug.Log("2.   other adds lastvisited");
                obj.CreateConnection(lastVisitedRoom, (Dir)(((int)direction + 2) % 4));
                //Debug.Log("2. done, " +dit);
            }
        }
        else
        {
            //Debug.Log("CreatePath: requested to create a path when floorsize equals or exceeds PathSize. No more rooms to make.");
            return;
        }

    }

    //draw out the connections for each room in this floor.
    public void DrawConnections()
    {
        foreach (WRoom o in map)
        {
            o.DrawConnections();
            o.DrawLocation();
        }
    }

    //otherwise use this
    public void CreateRoomModels(string theme)
    {
        //force the models to exist.
        InstantiateModels();
        foreach (WRoom o in map)
        {
            o.FindCXNType();
            o.ToggleWallsByCXN();
            o.model.transform.up = Vector3.up;
            o.ApplyTheme(theme);
        }
    }

    //randomly assign the door models for the rooms.
    // for each floor in the map, makes sure each connection only has one door object
    public bool RandomAssignDoorModelsShort = false;
    public void RandomAssignDoorModels() {
        if (RandomAssignDoorModelsShort)
        {
            //Debug.Log("randomassignroommodels shorted out.");
            return;
        }
        foreach (WRoom o in map){
            bool changeself = Random.value > 0.5f;
            for (int i = 0; i < 4; i++) {
                if (o.connections[i] != null)
                {
                    string d = ((Dir)i).ToString();
                    d = d[0].ToString().ToUpper() + d.Substring(1);
                    string t = ((Dir)((i+2)%4)).ToString();
                    t = t[0].ToString().ToUpper() + t.Substring(1);
                    o.model.transform.Find(d + "Door").gameObject.SetActive(changeself);
                    o.connections[i].model.transform.Find(t + "Door").gameObject.SetActive(!changeself);
                }
            }
        }
        RandomAssignDoorModelsShort = true;
    }
}




/*
    Room Object

Number of Connections
Orientation
Model
Location.
*/

    public class WRoom{
    
    public CXN connectionType;  //compare to CXN vals. generate this after we know our connection directions.
    public Dir orientation;     //compare to Dir vals. generate this after when determining CXN type
    public WRoom[] connections = new WRoom[4];  //rooms may only have 4 connections to other rooms the index of the connection is also a Dir enum
    // for example element 0 is the  north connection. element  2 is the south connection.

    public GameObject modelprefab, model, minimapmodel;    // the model that the room "is", and the quad that is shown on a minimap
    public Vector3 Location;
    public string ThemePath = "null"; // the string that identifies the particular theme to apply when the room is created
    private float ccb = 0.02f;
    private Renderer rend;
    public Renderer minirend;
    public List<GameObject> Contents;   //list of everything in the room that has any purpose. enemies, loot, stairs, keys, interactables, etc. if an object is in the list, it's state is controlled by the room.

    //the below is also accomplished in the updateroofing method.
    public bool hasBeenVisited, hasBeenRevealed, hasBeenCleared, hasObjective;   //bools determine the colors we use when drawing our minimap representation.
    public Color _Unrevealed = new Color(0,0,0,0), _Revealed = new Color(0.3f, 0.3f, 0.3f, 1), _Visited = new Color(0, 1, 0, 1), _NotCleared = new Color(0.6f, 0.6f, 0, 1);
    //if hasBeenVisited, the player has enterred the room.
    //if hasBeenRevealed, the player enterred a room that connects to this one.
    //if hasBeenCleared, the player has cleared the room of enemies, loot, and objectives.
    //if hasObjective, the room displays in a different color if it is to be displayed (depends on hasBeenVisited).


    public WRoom(GameObject prefab)
    {
        Location = new Vector3();
        modelprefab = prefab;
        Contents = new List<GameObject>();
    }

    /// <summary>
    ///creates the models and handles placing them in proper layers. Assigns the renderers.
    /// </summary>

    public void Start()
    {
        if (model == null) {
            model = Object.Instantiate(modelprefab);
            rend = model.transform.Find("Roof").GetComponent<Renderer>();
        }
        if (minimapmodel == null)
        {
            minimapmodel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            minimapmodel.transform.localScale = new Vector3(model.transform.localScale.x * 2, model.transform.localScale.x * 2, model.transform.localScale.z * 2);
            minimapmodel.transform.forward = -Vector3.up;
            minimapmodel.layer = 12; //minimaponly layer
            minimapmodel.name = model.name + " _ minimap block";
            minirend = minimapmodel.GetComponent<Renderer>();
            AlterMinimapModelByCXNType();
            //minirend.material.shader = Shader.Find("Unlit/Transparent");
        }       
    }

    /// <summary>
    /// destroys the memory intensive components.
    /// </summary>
    public void DestroyModels()
    {
        if (model != null)
        {
            model.SetActive(true);
            GameObject.Destroy(model);
        }
        if (minimapmodel != null)
        {
            minimapmodel.SetActive(true);
            GameObject.Destroy(minimapmodel);
        }
        if (minirend != null)
            GameObject.Destroy(minirend);
        if (rend != null)
            GameObject.Destroy(rend);
    }

    /// <summary>
    /// Any contents that are dead as far as their unitbehavior states will be permanently destroyed.
    /// </summary>
    public void DisposeDeadContents()
    {
        GameObject item;
        UnitBehavior unit;
        for (int i =0; i< Contents.Count; i++)
        {
            item = Contents[i];
            unit = item.GetComponent<UnitBehavior>();
            if (unit != null && unit.isDead)
            {
                //object is dead, dispose of it completely.
                unit = null;
                Contents.Remove(item);
                GameObject.Destroy(item);
                i--;
            }
        }
    }

    //literally call destroy on everything.
    public void DisposeAllContents()
    {
        GameObject c;
        for (int i =0; i < Contents.Count; i++)
        {
            c = Contents[i];
            
            if (c != null)
            {
                Contents.Remove(c);
                GameObject.Destroy(c);
                i--;
            }
            else
            {
                Contents.RemoveAt(i);
                i--;
            }
        }
    }

    //updates room location, room minimap location, and vision of the room/ color on the minimap
    public void Update()
    {
        if (model != null)
            model.transform.position = Location;
        if (minimapmodel != null)
            minimapmodel.transform.position = Location + Vector3.up;
        if (rend != null && minirend != null)
            FadeToVisionColor();
    }

    public void CreateConnection(WRoom other, Dir connection)
    {
        // if the requested connection direction is empty, fill it
        if (connections[(int)connection] == null)
        {
            connections[(int)connection] = other;
        }
        else
        {
            //Debug.Log("WRoom.CreateConnection was given a location with a previous connection, ignoring.");
        }
    }



    //static private Color[] col = { new Color(1,0,0,0.3f), new Color(0, 1, 0, 0.3f), new Color(0, 0, 1, 0.3f), new Color(0.6f, 0.6f, 0.6f, 0.3f) };
    /// <summary>
    /// draws raycasts from it's location to the locations of it's connected rooms.
    /// </summary>
    public void DrawConnections()
    {
        for (int  i = 0; i < 4; i++)
        {
            if (connections[i] != null)
            {
                Vector3 offset =  Vector3.up;// new Vector2((float)i / 10f, (float)i / 10f);
                Debug.DrawRay(Location + offset , (connections[i].Location - Location), Color.red);
            }
        }
    }

    public void DrawLocation()
    {
        Vector3 tl = new Vector3(-0.2f, 0, -0.2f);
        Vector3 tr = new Vector3(0.2f, 0, -0.2f);
        Vector3 ll = new Vector3(-0.2f, 0, 0.2f);
        Vector3 lr = new Vector3(0.2f, 0, 0.2f);
        Debug.DrawRay(Location + tl,tr -tl,Color.blue);
        Debug.DrawRay(Location + tr, lr - tr, Color.blue);
        Debug.DrawRay(Location + lr, ll - lr, Color.blue);
        Debug.DrawRay(Location + ll, tl - ll, Color.blue);
    }


    /// <summary>
    /// given the path to the 5 prefabs, finds the appropriate model, and stores the reference in the room's model.
    /// It instantiates the model.
    /// </summary>
    /// <param name="theme"> the path to the desired prefab set</param>
    
    public void ApplyTheme(string theme)
    {
        //material changes the texture only when changing themes.
        string texture = "Rooms/" + connectionType.ToString() + "/" + orientation.ToString() + theme;
        model.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>(texture);
        Debug.Log(texture);
        ThemePath = theme;
        
    }
    

    public void ApplyThemeWebGL(Texture2D theme)
    {
        model.GetComponent<Renderer>().material.mainTexture = theme;
    }

   

    /// <summary>
    /// Finds the CXNType using the connections array.
    /// </summary>
    public void FindCXNType()
    {
        int numCXN = 0;
        for (int i = 0; i < 4; i++)
        {
            if (connections[i] != null)
                numCXN += 1;
        }
        if (numCXN == 2)
        {
            //if north and south are filled, set hallway north.
            if (connections[0] != null && connections[2] != null)
            {
                orientation = Dir.north;
                connectionType = CXN.hall;
            }
            else if (connections[1] != null && connections[3] != null)
            {
                orientation = Dir.west;
                connectionType = CXN.hall;
            }
            else
            {
                connectionType = CXN.corner;
                if (connections[0] != null)
                {
                    if (connections[3] != null)
                        orientation = Dir.west;
                    else
                        orientation = Dir.north;
                }
                else if (connections[1] != null)
                    orientation = Dir.east;
                else if (connections[2] != null)
                    orientation = Dir.south;
            }
        }
        else if (numCXN == 4)
        {
            orientation = Dir.north;
            connectionType = CXN.quad;
        }
        else if (numCXN == 3)
        {
            connectionType = CXN.t;
            for (int i =0; i < 4; i++)
            {
                if (connections[i] == null)
                    orientation = (Dir)((i + 2)% 4);
            }
        }
        else
        {
            connectionType = CXN.single;
            for (int i = 0; i < 4; i++)
            {
                if (connections[i] != null)
                    orientation = (Dir)(i);
            }
        }
    }

    public void ToggleWallsByCXN()
    {
        for (int i = 0; i < 4; i++)
        {
            string d = ((Dir)i).ToString();
            d = d[0].ToString().ToUpper() + d.Substring(1);
            if (connections[i] != null)
            {
                model.transform.Find(d + "Wall").gameObject.SetActive(false);
                minimapmodel.transform.Find(d + "Wallminimap").gameObject.SetActive(false);
                model.transform.Find(d + "Door").gameObject.SetActive(true);
            }
            else
            {
                model.transform.Find(d + "Wall").gameObject.SetActive(true);
                minimapmodel.transform.Find(d + "Wallminimap").gameObject.SetActive(true);
                model.transform.Find(d + "Door").gameObject.SetActive(false);
            }
        }
    }

    private Vector4 roofColor = Vector4.zero;
    //Sets the color of the Roof component in the model.
    public void SetVisionColor(Color o)
    {
        roofColor.Set(o.r, o.g, o.b, o.a);
    }

    /// <summary>
    /// sets the model active/inactive, as well as any contents in the room.
    /// </summary>
    /// <param name="a"></param>
    public void SetActive(bool a)
    {
        if (model != null)
            model.SetActive(a);
        foreach (GameObject o in Contents)
        {
            if (o.transform.Find("Model/UD_light_infantry") != null && o.transform.Find("Model/UD_light_infantry").GetComponent<Renderer>() != null)
            {
                o.transform.Find("Model/UD_light_infantry").GetComponent<Renderer>().enabled = a;
            }
            else
            { 
                o.SetActive(a);
            }
        }
    }



    void FadeToVisionColor()
    {
        //Debug.Log("FadeToVisionColor called.");
        Color orig = rend.material.GetColor("_MainColor");
        float r = orig.r; float g = orig.g; float b = orig.b; float a = orig.a;
        if (r > roofColor.x + ccb) { r = r - ccb; } else if (r < roofColor.x - ccb) { r = r + ccb; } else { r = roofColor.x; }
        if (g > roofColor.y + ccb) { g = g - ccb; } else if (g < roofColor.y - ccb) { g = g + ccb; } else { g = roofColor.y; }
        if (b > roofColor.z + ccb) { b = b - ccb; } else if (b < roofColor.z - ccb) { b = b + ccb; } else { b = roofColor.z; }
        if (a > roofColor.w + ccb) { a = a - ccb; } else if (a < roofColor.w - ccb) { a = a + ccb; } else { a = roofColor.w; }
        Color newcol = new Color(r, g, b, a);
        rend.material.SetColor("_MainColor", newcol);

        Vector4 minimapgoal = new Vector4();
        if (hasObjective)
        {
            minimapgoal.Set(_NotCleared.r, _NotCleared.g, _NotCleared.b, _NotCleared.a);
            minimapmodel.SetActive(true);
        }
        else if (hasBeenVisited)
        {
            minimapgoal.Set(_Visited.r, _Visited.g, _Visited.b, _Visited.a);
            minimapmodel.SetActive(true);
        }
        else if (hasBeenRevealed)
        {
            minimapgoal.Set(_Revealed.r, _Revealed.g, _Revealed.b, _Revealed.a);
            minimapmodel.SetActive(true);
        }
        else
        {
            minimapgoal.Set(_Unrevealed.r, _Unrevealed.g, _Unrevealed.b, _Unrevealed.a);
            minimapmodel.SetActive(false);
        }
        orig = minirend.material.GetColor("_Color");
        r = orig.r; g = orig.g; b = orig.b; a = orig.a;
        if (r > minimapgoal.x + ccb) { r = r - ccb; } else if (r < minimapgoal.x - ccb) { r = r + ccb; } else { r = minimapgoal.x; }
        if (g > minimapgoal.y + ccb) { g = g - ccb; } else if (g < minimapgoal.y - ccb) { g = g + ccb; } else { g = minimapgoal.y; }
        if (b > minimapgoal.z + ccb) { b = b - ccb; } else if (b < minimapgoal.z - ccb) { b = b + ccb; } else { b = minimapgoal.z; }
        if (a > minimapgoal.w + ccb) { a = a - ccb; } else if (a < minimapgoal.w - ccb) { a = a + ccb; } else { a = minimapgoal.w; }
        newcol = new Color(r, g, b, a);
        minirend.material.SetColor("_Color", newcol);
    }

    //sets the minimap walls to be children of the minimap
    //only call once per room otherwise the minimap will have scaling issues.
    public void AlterMinimapModelByCXNType()
    {
        Transform W;
        bool north = false, west = false, south = false, east = false;
        if (model.transform.Find("NorthWall").gameObject.activeSelf)
        {
            W = model.transform.Find("NorthWall/NorthWallminimap");
            W.SetParent(minimapmodel.transform, true);
            W.position = new Vector3(W.position.x, 0.0f, W.position.z);
        }
        if (model.transform.Find("WestWall").gameObject.activeSelf)
        {
            W = model.transform.Find("WestWall/WestWallminimap");
            W.SetParent(minimapmodel.transform, true);
            W.position = new Vector3(W.position.x, 0.0f, W.position.z);
        }
        if (model.transform.Find("SouthWall").gameObject.activeSelf)
        {
            W = model.transform.Find("SouthWall/SouthWallminimap");
            W.SetParent(minimapmodel.transform, true);
            W.position = new Vector3(W.position.x, 0.0f, W.position.z);
        }
        if (model.transform.Find("EastWall").gameObject.activeSelf)
        {
            W = model.transform.Find("EastWall/EastWallminimap");
            W.SetParent(minimapmodel.transform, true);
            W.position = new Vector3(W.position.x, 0.0f, W.position.z);
        }
    }


}
