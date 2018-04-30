using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Handles simple motion to a point, grounding, and reporting health.
/// </summary>
public class UnitBehavior : MonoBehaviour {

    public Vector3 down = Vector3.down;         //determines which direction to check for ground tiles. (for instance this could enable us to have curved spaces, walk on walls/cliffs functionality.
    
    public float health, damage, wounds;
    private Vector3 overWorldDestination, overWorldDestination2, overWorldDifference;      //set this if you wish to walk to this destination in the overworld/dungeon world.
    public float moveSpeed = 6;                                     //how fast to walk to overworlddestination.
    private float radius = 0.1f;                                    //how "wide" our "collider" is.

    //set this bool via other scripts. may not be used by this script, but is definitely used for objects that are players.
    public bool inBattle = false;
    public bool isDead = false;
    public bool isMoving;
    // holds the "Turn" objects that define how the unit is to behave.
    private List<Turn> CombatActionQueue = new List<Turn>();
    private int CombatActionLimit = 6;

    public Vector2 AttackDMG = new Vector2(20, 5);

    // Use this for initialization
    void Start () {
        if (health == 0) { health = 50; }
        if (wounds > 0) { wounds = 0; }
        else { wounds = -wounds; }
        if (damage > 0) {damage= 0; }
        else { damage = -damage; }
	}
	
    //snaps the player model to the unit's transform location.
    private void SnapModel()
    {
        gameObject.transform.Find("Model").transform.localPosition = Vector3.zero;
        gameObject.transform.Find("Model").transform.localRotation = Quaternion.Euler(0,0,0);

    }

    // Update is called once per frame
    void Update () {
        //begin moving towards the target's location.
        OverWorldUpdate();
        //force our transform to remain grounded.
        PlaceOnGround();
        if (gameObject.transform.Find("Model")) {  SnapModel(); }
        //if we die report that.
        if (health - (wounds + damage) <= 0)
        {
            isDead = true;
            if (gameObject.GetComponent<HealthOverTarget>())
            {
                Destroy(gameObject.GetComponent<HealthOverTarget>());
            }
            //Debug.Log(gameObject.name + " has died from lack of health: " + (health - (wounds + damage)) + " / " + health);
        }
        else if (!gameObject.GetComponent<HealthOverTarget>() && inBattle)
        {
            gameObject.AddComponent<HealthOverTarget>();
        }
        if (!inBattle)
        {
            TurnNum = 0;
            if (damage > 0 && !isDead)
            {
                damage -= 1.0f / 60.0f;
            }
            else if (!isDead)
            {
                damage = 0;
            }
        }
        
    }



    public Vector3 GetOverworldDifference()
    {
        return overWorldDifference;
    }

    /// <summary>
    /// return the difference between transform and destination2 point.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetDestDifference()
    {
        return overWorldDestination2 - transform.position;
    }

    //update for overworld motion.
    public float speedmultiplier;
    public float WtR = 6.0f;
    private void OverWorldUpdate()
    {
        //if we should move, our destination isn't 0.
        if (overWorldDestination != Vector3.zero)
        {
            overWorldDifference = (overWorldDestination - transform.position);      //find the difference, this controls our rotation.
            if (overWorldDifference.magnitude <= 0.001f)
            {
                overWorldDestination = Vector3.zero;                                //if magnitude is too low, don't do anything.
                overWorldDestination2 = Vector3.zero;
            }
            else                                                                                                                    //Else do movement code.
            {
                overWorldDestination2 = overWorldDestination;
                //transform.up = -down;
                transform.up = Vector3.up;
                transform.forward = Vector3.ProjectOnPlane(overWorldDifference,down).normalized;
                //raycast from location to the destination location. if it collides with an obstacle, set our destinationlocation to be that far down the raycast - our collider radius * the normal of the object.
                RaycastHit info;
                if (Physics.SphereCast(transform.position, radius, overWorldDifference, out info, overWorldDifference.magnitude, 1 << 10))
                {
                    overWorldDestination2 = transform.position + transform.forward * (info.distance) + Vector3.ProjectOnPlane(info.normal, down).normalized * radius * 2f;
                }
                //slow down when you get near your destination.
                speedmultiplier = (overWorldDifference.magnitude > WtR)? 1 : overWorldDifference.magnitude/(WtR * 1.5f) + 0.33f ;
                speedmultiplier = inBattle ? 1 : speedmultiplier;
                transform.position = Vector3.MoveTowards(transform.position, overWorldDestination2, speedmultiplier * moveSpeed * Time.deltaTime);
                if ((overWorldDestination2 - transform.position).magnitude > 0.001f)
                {
                    isMoving = true;
                }
            }
        }
        else
        {
            isMoving = false;
        }
        Debug.DrawRay(transform.position, transform.forward * radius, Color.green);
        Debug.DrawRay(transform.position, overWorldDifference, Color.red);
        Debug.DrawRay(transform.position, (overWorldDestination2 - transform.position), Color.yellow);
    }

    // sets target position as vector3. 
    public void SetOverworldDestination(Vector3 targetPosition)
    {
        overWorldDestination = targetPosition;
        overWorldDestination2 = overWorldDestination;
    }

    private bool HitObstacle(out RaycastHit hitinfo) {
        // checks for collisions with objects in layer 10
        return Physics.SphereCast(transform.position, radius, transform.forward, out hitinfo, 0, 1 << 10);
    }

    //place the unit such that it sits on top of the surface (surface must be in proper layer) it is over.
    private void PlaceOnGround()
    {
        RaycastHit info;
        //solve for overworldterrain , dungeonworld terrain, and lastly for hextiles.
        if (Physics.Raycast(transform.position - down, down, out info, Mathf.Infinity, 1 << 8))
        {
            transform.position += (info.distance - down.magnitude) * down;
            //down = -info.normal; //uncomment if yu want the transform to follow the terrain slope
        }
        else if (Physics.Raycast(transform.position - down, down, out info, Mathf.Infinity, 1 << 9))
        {
            transform.position += (info.distance - down.magnitude) * down;
            //down = -info.normal;
        }
        else if (Physics.Raycast(transform.position - down, down, out info, Mathf.Infinity, 1 << 11))
        {
            transform.position += (info.distance - down.magnitude) * down;
            //down = -info.normal;
        }
    }

    /// <summary>
    /// returns health as vector3 in the form of (max, wounddamage, damage)
    /// eg. (100,30,20) = 50 health points out of 70 recoverable health points, with 30 points of wound damage.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHealth()
    {
        return new Vector3(health, wounds, damage);
    }

    /// <summary>
    /// damage unit with "dmg" vector2 such that wounddamage and damage are increased. eg. (wounddamage,damage)
    /// </summary>
    /// <param name="dmg"></param>
    public void DamageHealth(Vector2 dmg)
    {
        wounds += dmg[0];
        damage += dmg[1];
    }

    /// <summary>
    /// cleanly handles all "turn" objects and destroys them.
    /// </summary>
    public void ClearCombatActionQueue()
    {
        foreach( Turn o in CombatActionQueue)
        {
            o.Destroy();
        }
        CombatActionQueue.Clear();
    }

    /// <summary>
    /// returns true if the action was succesfully added to the unit's turn queue. False if otherwise.
    /// </summary>
    /// <param name="Action">A full turn object with "ExternalUpdate", "ExternalStart", "ExternalEnd" function callbacks.</param>
    /// <returns></returns>
    public bool PushCombatAction(Turn Action)
    {
        if (CombatActionQueue.Count >= CombatActionLimit)
            return false;
        CombatActionQueue.Add(Action);
        TurnNum++;
        return true;
    }

    public void BeginAction()
    {
        //terminate any movement currently in progress.
        SetOverworldDestination(Vector3.zero);
        if (CombatActionQueue.Count > 0)
        {
            CombatActionQueue[0].TurnStart();
        }
        else
        {
            //Debug.Log("FATAL ERROR: BeginAction called with 0 actions queued: " + name);
        }

    }


    public void ContinueAction()
    {
        if (CombatActionQueue.Count > 0)
        {
            CombatActionQueue[0].TurnUpdate();
        }
        else
        {
            //Debug.Log("FATAL ERROR: ContinueAction called with 0 actions queued: "+ name);
        }
    }

    public void EndAction()
    {
        if (CombatActionQueue.Count > 0)
        {
            
            //call end on the turn.
            CombatActionQueue[0].TurnEnd();
            CombatActionQueue[0].Destroy();
            //pop it from the list.
            CombatActionQueue.Remove(CombatActionQueue[0]);
            //stop movement capacity.
            SetOverworldDestination(Vector3.zero);
        }
        else
        {
            //Debug.Log("FATAL ERROR: EndAction called with 0 actions queued: " + name);
        }
    }

    public int TurnNum = 0;

    /// <summary>
    /// deletes all actions past index N in the CombatActionQueue
    /// </summary>
    /// <param name="n"></param>
    public void PreserveFirstNActions(int n)
    {
        if (n < 0)
        {
            n = 0;
        }
        while(CombatActionQueue.Count > n)
        {
            TurnNum--;
            CombatActionQueue[n].Destroy(); //remove any unity objects we can before going out of scope.
            CombatActionQueue.RemoveAt(n);
        }
    }

    private void ExecuteAction()
    {
        if (CombatActionQueue.Count > 0)
        {
            CombatActionQueue[0].ExternalUpdate();
        }
        else
        {
            //Debug.Log("FATAL ERROR: EXECUTE ACTION called on queue with no turns.");
        }
    }

    /// <summary>
    /// returns this unit's combat action turn list.
    /// </summary>
    /// <returns></returns>
    public List<Turn> GetCombatActionQueue()
    {
        return CombatActionQueue;
    }

    public int GetCombatActionCount()
    {
        return CombatActionQueue.Count;
    }





    public Vector2 GetSkill(string typing)
    {
        if (typing == "cast")
        {
            return new Vector2(10,10);
        }
        if (typing.Contains("basic"))
        {
            return AttackDMG;
        }
        return Vector2.zero;
    }
}
