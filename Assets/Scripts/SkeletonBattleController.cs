using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBattleController : MonoBehaviour {

    public UnitBehavior player, MyUnit;
    public int TurnNum = 0;
	// Use this for initialization
	void Start () {
		MyUnit = gameObject.GetComponent<UnitBehavior>();
    }
	
	// Update is called once per frame
	void Update () {
        if (MyUnit.isDead)
        {
            MyUnit.gameObject.SetActive(false);
        }
        if (player == null)
        {
            player = transform.GetComponent<EnemyBehaviour>().Target.GetComponent<UnitBehavior>();
            return;
        }
        
        //executes if we need to give our unit a turn.
        if (MyUnit.GetCombatActionCount() < player.GetCombatActionCount())
        {
            //add a turn.
            if (MyUnit.TurnNum % 2 == 0)
            {
                //move
                Turn X = new Turn();
                X.actiontype = "move";  //ensures the turn knows it has a movement type command
                X.ran = 1.8f; //preserve range for this action
                X.Start(MyUnit);  //ensures the target and targetlocator exist.
                if (!MyUnit.PushCombatAction(X))
                {
                    X.Destroy();
                }
                X.TargetLocator.SetActive(true);
                
                //target the player's next location.
                X.SetTarget((player.GetCombatActionQueue()[0].GetOrigin() - MyUnit.transform.position).normalized * X.GetRange() + X.GetOrigin());
                X.ExternalUpdate = MoveUpdate;

                //prevent player from seeing our targetlocation
                X.TargetLocator.transform.Find("RangeIndicator/Plane").GetComponent<Renderer>().enabled = false;
            }
            else
            {
                //fix the previous move command.
                if (MyUnit.GetCombatActionCount() > 0)
                {
                    Turn Y = MyUnit.GetCombatActionQueue()[MyUnit.GetCombatActionCount() - 1];
                    if (Y.actiontype == "move")
                    Y.SetTarget((player.GetCombatActionQueue()[0].GetOrigin() - MyUnit.transform.position).normalized * Y.GetRange() + Y.GetOrigin());
                }



                // then add an attack
                Turn X = new Turn();
                X.actiontype = "basicattack";  //ensures the turn knows it has a basic attack type command
                X.ran = 1.5f; //preserve range for this action
                X.Start(MyUnit);  //ensures the target and targetlocator exist.
                X.SetProcTime(1.0f);
                MyUnit.AttackDMG = new Vector2(4, 2); //REEEEEEEEE TODO don't do this shit. this is hard coding because framework isnt done to pull damages from weapon+stats
                if (!MyUnit.PushCombatAction(X))
                {
                    X.Destroy();
                }
                X.TargetLocator.SetActive(true);
                
                X.SetTarget((player.GetCombatActionQueue()[0].GetOrigin() - MyUnit.transform.position).normalized * X.GetRange() + X.GetOrigin());
                X.ExternalStart = BasicAttackStart;
                X.ExternalUpdate = BasicAttackUpdate;
                X.ExternalEnd = BasicAttackEnd;

                //prevent player from seeing our targetlocation
                X.TargetLocator.transform.Find("RangeIndicator/Plane").GetComponent<Renderer>().enabled = false;
            }
            Debug.Log("Turn: " + TurnNum);
        }


    }

    //basic attack for "skeleton" like structure.
    public void BasicAttackStart()
    {
        Turn X = MyUnit.GetCombatActionQueue()[0];
        X.self = MyUnit;

        MyUnit.transform.Find("Model").GetComponent<SkeletonAnimationControllerScript>().myanim.SetTrigger("Attack");
        X.model = MyUnit.transform.Find("Model").gameObject;
        //no particle effect here.
        //self.transform.Find("SlashKick").GetComponent<ParticleSystem>().Play();
    }

    //expected to be a turn update
    public void BasicAttackUpdate()
    {
        Turn X = MyUnit.GetCombatActionQueue()[0];
        //model.transform.forward = direction;
        X.direction = (player.transform.position - MyUnit.transform.position).normalized * 1.5f;
        MyUnit.transform.forward = X.direction;
        //todo produce a fireball unitbehavior that moves to TargetLocator's position after say 40 frames
        //Debug.Log("procced =: " + X.HasProcced);
        if (!X.HasProcced)
        {
            //test to see if we dont hit the player.
            if (X.CurrentTime - X.StartTime > X.ProcTime)
            {
                bool hit = true;
                //determine distance/angle
                Vector3 diff = player.transform.position - MyUnit.transform.position;
                // short if distance is too great.
                if (diff.magnitude > 1.5f)
                {
                    X.HasProcced = true;
                    hit = false;
                }
                else
                {
                    float L = MyUnit.transform.GetComponent<CapsuleCollider>().radius;
                    float H = L * 2;
                    Ray r = new Ray(MyUnit.transform.position, X.direction.normalized);
                    float o = Vector3.Cross(r.direction, diff).magnitude;
                    // Hit this unit if inside hitbox influence;
                    if ((o - player.GetComponent<SphereCollider>().radius) > L + (H - L) * (diff.magnitude / X.ran))
                    {
                        hit = false;
                    }
                    Debug.Log("Hit is: " + hit);
                }

                X.HasProcced = true;
                if (hit)
                {
                    player.DamageHealth(MyUnit.GetSkill("basicattack"));
                }
            }
            
        }


    }
    //expected to be a turn end
    public void BasicAttackEnd()
    {
        Turn X = MyUnit.GetCombatActionQueue()[0];
        X.self.transform.forward = X.direction;
        X.model = null;
    }

    public void MoveUpdate()
    {
        //pull this turn
        Turn X = MyUnit.GetCombatActionQueue()[0];
        Vector3 newplace = (player.transform.position - X.origin).normalized * 1.8f + X.origin;
        X.SetTarget(newplace);
        MyUnit.SetOverworldDestination(X.Target.position);
        //X.TargetLocator.transform.position = X.Target.position;
    
    }



}
