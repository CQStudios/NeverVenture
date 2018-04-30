using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// turn class has two required functions. start and end.
/// It could also utilize an update function which executes whatever directive is given to it.
/// it is expected that when you Queue a turn to
/// </summary>
public class Turn {

    //declare type of "Callback" which can be used.
    public delegate void Callback();
    public string actiontype = "";
    public Callback ExternalUpdate, ExternalStart, ExternalEnd;
    public Transform Target;
    public GameObject TargetLocator;
    public UnitBehavior self;
    public Vector3 direction;
    public Vector3 origin;
    public Vector3 displacement;
    private bool started = false, ended = false;
    public float ran;
    public Color col;

	// call this to create a turn.
	public void Start(UnitBehavior s) {
        TargetLocator = GameObject.Instantiate(Resources.Load<GameObject>("ActionLocator"));
        SetRangeColor(TargetLocator);
        self = s;
        //properly offset this turn's origin point based on previous turns.
        origin = GetOrigin();
        SetTarget( origin + self.transform.Find("Model").forward);
	}
    public void SetRangeColor(GameObject T)
    {
        Material x = T.transform.Find("RangeIndicator/Plane").GetComponent<Renderer>().material;
        if (actiontype == "cast")
        {
            x.SetColor("_MainColor", new Color(1, 0, 0, 1));
        }
        if (actiontype == "move")
        {
            x.SetColor("_MainColor", new Color(0, 1, 0, 1));
        }
        if (actiontype == "basicattack")
        {
            x.SetColor("_MainColor", new Color(1, 0.7f, 0.2f, 1));
        }
    }

    public void SetDisplacementToTarget()
    {
        displacement = Target.position - origin;
    }
    public void SetTarget(Vector3 t)
    {
        Target = TargetLocator.transform;
        Target.position = t;
        //uses direction to establish the firing angle for anythin requiring a firing angle. 
        //REEEEEEEEEE - todo, make sure it's the target minus the last predicted player location.
        //      since moveto would guaranteed move a player, we'd want the last moveto location, not the player's current location.
        direction = Target.transform.position - origin;
        if (actiontype == "move")
        {
            SetDisplacementToTarget();
        }
    }

    /// <summary>
    /// returns a vector3 for the "effective" starting location for this turn.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetOrigin()
    {
        //loop over turns in our unit's combat queue excepting this ons.
        Vector3 og = self.transform.position;
        List<Turn> copy = self.GetCombatActionQueue();
        int count = self.GetCombatActionCount();
        if (copy == null)
        {
            return og;
        }
        if (copy.Contains(this))
        {
            count -= 1;
        }
        for (int i = 0; i < count; i++)
        {
            og += copy[i].displacement; 
        }
        return og;

    }





    /// <summary>
    /// calls external start. 
    /// </summary>
    public void TurnStart()
    {
        started = true;
        CurrentTime = Time.time;
        StartTime = Time.time;
        if (ExternalStart != null)
        ExternalStart();
    }

    public void TurnUpdate()
    {
        if (started && !ended)
        {
            CurrentTime = Time.time;
            if (ExternalUpdate != null)
                ExternalUpdate();
        }
    }

    public void TurnEnd()
    {
        ended = true;
        StartTime = 0;
        CurrentTime = 0;
        if (ExternalEnd != null)
            ExternalEnd();
    }


























    //all posible turns capable of being executed.


    //expected to be a turn update
    public void MoveToLocation()
    {
        self.SetOverworldDestination(Target.position);
        TargetLocator.transform.position = Target.position;
    }

    
    
    
    //expected to be a turn update
    public void MoveToUnit()
    {
        self.SetOverworldDestination(Target.position);
        TargetLocator.transform.position = Target.position;
    }

    //only for "Neven" like units.
    private PlayerAnimationController selfanim;
    public GameObject model;
    private List<UnitBehavior> DamageTheseUnits;
    public bool HasProcced = false;
    public float CurrentTime, StartTime, ProcTime;
    public void SetProcTime(float tim) { ProcTime = tim; }
    private BattleControlScript BCS = GameObject.Find("OverWorldController").GetComponent<BattleControlScript>();
    //expected to be a turn start
    public void CastFireStart()
    {
        selfanim = self.transform.Find("Model").GetComponent<PlayerAnimationController>();
        selfanim.myanim.SetTrigger("Attack3Trigger");
        self.transform.Find("FireStrike").GetComponent<ParticleSystem>().startSpeed = self.transform.Find("RangeIndicator").GetComponent<RangeIndicator>().radius;
        self.transform.Find("FireStrike").GetComponent<ParticleSystem>().Play();
        //model = self.transform.Find("Model").gameObject;
    }
    //expected to be a turn update
    public void CastFireUpdate()
    {
        //model.transform.forward = direction;
        self.transform.forward = direction;
        //todo produce a fireball unitbehavior that moves to TargetLocator's position after say 40 frames
        //Debug.Log("procced =: " + HasProcced);
        if (!HasProcced)
        {
            if (CurrentTime - StartTime > ProcTime)
            {
                //CAST FIRE BALL
                //start firestrike animation
                

                List<UnitBehavior> EligibleUnits = new List<UnitBehavior>() , pulled = GetSusceptibleUnits();
                //Debug.Log("GetSusceptibleUnits gave " + pulled.Count + " units.");
                foreach (UnitBehavior unit in pulled)
                {

                    //determine distance/angle
                    Vector3 diff = unit.transform.position - origin;
                    // short if distance is too great.
                    if (diff.magnitude - unit.GetComponent<CapsuleCollider>().radius > direction.magnitude) { continue; }


                    float L = self.transform.GetComponent<SphereCollider>().radius;
                    float H = L * 2;
                    Ray r = new Ray(origin, direction.normalized);
                    float o = Vector3.Cross(r.direction, diff).magnitude;
                    // skip this unit if outside hitbox influence;
                    if ( o - unit.GetComponent<CapsuleCollider>().radius > L + (H - L) * (diff.magnitude / direction.magnitude)) { continue; }
                    //add the eligible enemies to the list.
                    EligibleUnits.Add(unit);
                }
                if (EligibleUnits.Count < 1) { Debug.Log("no units hit."); HasProcced = true; return; }

                //hit the closest enemy code below
                /*
                UnitBehavior hit = EligibleUnits[0];
                foreach(UnitBehavior unit in EligibleUnits)
                {
                    if ((unit.transform.position - origin).magnitude < (hit.transform.position - origin).magnitude)
                    {
                        hit = unit;
                    }
                }
                hit.DamageHealth(self.GetSkill(actiontype));
                */

                //hit all enemies able to be hit code below
                foreach (UnitBehavior unit in EligibleUnits)
                {
                    unit.DamageHealth(self.GetSkill(actiontype));
                }
                HasProcced = true;
            }
        }

        CurrentTime = Time.time;
    }
    //expected to be a turn end
    public void CastFireEnd()
    {
        selfanim = null;
        //model.transform.forward = direction;
        self.transform.forward = direction;
        model = null;
    }


    //expected to be a turn start for a "Neven"-like unit
    public void BasicAttackStart()
    {
        selfanim = self.transform.Find("Model").GetComponent<PlayerAnimationController>();
        selfanim.myanim.SetTrigger("AttackKick2Trigger");
        model = self.transform.Find("Model").gameObject;
        self.transform.Find("SlashKick").GetComponent<ParticleSystem>().Play();
    }
    //expected to be a turn update
    public void BasicAttackUpdate()
    {
        //model.transform.forward = direction;
        self.transform.forward = direction;
        //todo produce a fireball unitbehavior that moves to TargetLocator's position after say 40 frames
        //Debug.Log("procced =: " + HasProcced);
        if (!HasProcced)
        {
            if (CurrentTime - StartTime > ProcTime)
            {
                //CAST FIRE BALL
                List<UnitBehavior> EligibleUnits = new List<UnitBehavior>(), pulled = GetSusceptibleUnits();
                //Debug.Log("GetSusceptibleUnits gave " + pulled.Count + " units.");
                foreach (UnitBehavior unit in pulled)
                {

                    //determine distance/angle
                    Vector3 diff = unit.transform.position - origin;
                    // short if distance is too great.
                    if (diff.magnitude > ran) { continue; }


                    float L = self.transform.GetComponent<SphereCollider>().radius;
                    float H = L * 2;
                    Ray r = new Ray(origin, direction.normalized);
                    float o = Vector3.Cross(r.direction, diff).magnitude;
                    // skip this unit if outside hitbox influence;
                    if (o - unit.GetComponent<CapsuleCollider>().radius > L + (H - L) * (diff.magnitude / ran)) { continue; }
                    //add the eligible enemies to the list.
                    EligibleUnits.Add(unit);
                }
                if (EligibleUnits.Count < 1) { Debug.Log("no units hit."); HasProcced = true; return; }

                //hit the closest enemy code below
                
                UnitBehavior hit = EligibleUnits[0];
                foreach(UnitBehavior unit in EligibleUnits)
                {
                    if ((unit.transform.position - origin).magnitude < (hit.transform.position - origin).magnitude)
                    {
                        hit = unit;
                    }
                }
                hit.DamageHealth(self.GetSkill(actiontype));


                //hit all enemies able to be hit code below
                /*
                foreach (UnitBehavior unit in EligibleUnits)
                {
                    unit.DamageHealth(self.GetSkill(actiontype));
                }
                */            
                HasProcced = true;
                
            }
        }


    }
    //expected to be a turn end
    public void BasicAttackEnd()
    {
        selfanim = null;
        //model.transform.forward = direction;
        self.transform.forward = direction;
        model = null;
    }

    public void BlockStart()
    {
        self.transform.Find("BlockEffect").GetComponent<ParticleSystem>().Play();
    }

    public void BlockEnd()
    {
        self.transform.Find("BlockEffect").GetComponent<ParticleSystem>().Stop();
    }






    private List<UnitBehavior> GetSusceptibleUnits()
    {
        List<UnitBehavior> ret = new List<UnitBehavior>();
        bool isplayer = (self.GetComponent<PlayerBehaviour>() != null);
        bool isenemy = (self.GetComponent<EnemyBehaviour>() != null);
        foreach (UnitBehavior o in BCS.GetAliveCombatants())
        {

            if (o == self) { continue; }
            if (isplayer)
            {
                if (o.transform.GetComponent<PlayerBehaviour>() == null)
                {
                    ret.Add(o);
                }
            }
            else if (isenemy)
            {
                if (o.transform.GetComponent<EnemyBehaviour>() == null)
                {
                    ret.Add(o);
                }
            }
        }
        //Debug.Log("Giving " + ret.Count + " combatants back from GetSusceptibleUnits");
        return ret;
    }

    public float GetRange()
    {
        return ran;
    }
    public Color GetColor()
    {
        return col;
    }














    public void Destroy()
    {
        Target = null;
        GameObject.Destroy(TargetLocator,0.0f);
    }








}
