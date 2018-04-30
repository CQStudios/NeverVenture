using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{


    public int MeleeDamage, RangeDamage, Armor, Resist, health, experience;
    //public Loot Table... somehow
    public bool created = false;
    public bool isAlive = false;
    //add specific damages and resistances here          !!!!!!ADD TO THIS LATER!!!!!!!!

    private int TurnNumber = 0;
    
    public GameObject Target;
    //AI for Enemies: 
    //MeleeEasy: Will move to player and attack half as often as possible
    //MeleeNormal: Will move to player and alternate between attack and block
    //MeleeHard: Will move behind the player and block four times and attack twice in any order
    //RangedEasy: Get in view of the player and attack
    //RangedNormal: Get in view of the player, attack and and move in a circle around the player
    //RangedHard: Get in view of the player, attack the player then move away from the player
    public string ai = "MeleeEasy";

    
    
    
    
    private UnitBehavior myUnit;
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (myUnit == null)
        {
            myUnit = gameObject.GetComponent<UnitBehavior>();
            return;
        }

        if (Target != null)
        {
            //move towards target during battle. eventually replace this entirely. 
            //when in battle we shouldn't setoverworlddestination to anything because our turn queue will.
            if (myUnit.inBattle && !myUnit.isDead && isAlive)
            {
                //myUnit.SetOverworldDestination(Target.transform.position);
            }
            else
            {
                myUnit.SetOverworldDestination(Vector3.zero);
            }
        }




        //handle for the frame where we die via unitbehavior and need to trigger death animation.
        if (myUnit.isDead && isAlive)
        {
            isAlive = false;
            gameObject.transform.localScale *= 0.5f;
        }

        
    }

    /// <summary>
    /// request damage to be applied to this enemybehaviour unit and it's underlying unitbehaviour.
    /// </summary>
    /// <param name="dmg">vector2, (wound damage, damage) such that wound damage is applied to wound's value, and damage is applied after. </param>
    public void Damage(Vector2 dmg)
    {
        float rawwounds = dmg[0];
        float rawdamage = dmg[1];
        
        //todo do things here converting raw values into true values
        

    }
    void DoMovement()
    {

    }

    //add to your unit's turn queue
    void DoMoves(string ai)
    {
        if (ai == "MeleeEasy")
        {
            //Attack the second action Enemy is adjacent to player then wait next second, repeat
        }
    }

    //preserved the first n actions.
    public void PreserveNActions(int n)
    {
        TurnNumber -= (myUnit.GetCombatActionCount() - n);
        myUnit.PreserveFirstNActions(n);
    }
    /*
    //put the next turn in our queue.
    public void AddNextTurn()
    {
        if (TurnNumber % 2 == 0)
        {
            //move
        }
        else
        {
            //attack
            Turn X = new Turn();
            X.actiontype = "basicattack";  //ensures the turn knows it has an attack type command
            X.ran = 1.5f;
            X.Start(myUnit);  //ensures the target and targetlocator exist.
            if (!playerObject.PushCombatAction(X))
            {
                X.Destroy();
            }
            X.TargetLocator.SetActive(true);
            X.ExternalUpdate = X.MoveToLocation;
            actionselected = true;
        }
        TurnNumber++;
    }
    */

    void CreateEnemy(int tierOfPlayer, int Enocunter)
    {
        System.Random rnd = new System.Random();
        int aiSelect = rnd.Next(1, 10);
        if (Enocunter == 0)
        {
            if (aiSelect <= 5)
            {
                ai = "MeleeEasy";
                //Use Easy Melee Mesh
                this.MeleeDamage = 3 * tierOfPlayer;
                this.RangeDamage = 1 + tierOfPlayer;
                this.health = 6 * tierOfPlayer;
                this.Armor = 0 + tierOfPlayer;
                this.Resist = 0 + tierOfPlayer;
                this.experience = 50 * tierOfPlayer;
                this.isAlive = true;
            }
            else
            {
                ai = "RangedEasy";
                //Use Easy Ranged Mesh
                this.MeleeDamage = 1 + tierOfPlayer;
                this.RangeDamage = 3 * tierOfPlayer;
                this.health = 6 * tierOfPlayer;
                this.Armor = 0 + tierOfPlayer;
                this.Resist = 0 + tierOfPlayer;
                this.experience = 50 * tierOfPlayer;
                this.isAlive = true;
            }
        }
        else if (Enocunter == 1)
        {
            if (aiSelect <= 3)
            {
                ai = "MeleeEasy";
                //Use Easy Melee Mesh
                this.MeleeDamage = 3 * tierOfPlayer;
                this.RangeDamage = 1 + tierOfPlayer;
                this.health = 6 * tierOfPlayer;
                this.Armor = 0 + tierOfPlayer;
                this.Resist = 0 + tierOfPlayer;
                this.experience = 50 * tierOfPlayer;
                this.isAlive = true;
            }
            else if (aiSelect > 3 && aiSelect <= 5)
            {
                ai = "MeleeNormal";
                //Use Normal Melee Mesh
                this.MeleeDamage = 3 * tierOfPlayer;
                this.RangeDamage = 1 + tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 2 * tierOfPlayer;
                this.Resist = 2 + tierOfPlayer;
                this.experience = 90 * tierOfPlayer;
                this.isAlive = true;
            }
            else if (aiSelect > 5 && aiSelect <= 8)
            {
                ai = "RangedEasy";
                //Use Easy Ranged Mesh
                this.MeleeDamage = 1 + tierOfPlayer;
                this.RangeDamage = 3 * tierOfPlayer;
                this.health = 6 * tierOfPlayer;
                this.Armor = 0 + tierOfPlayer;
                this.Resist = 0 + tierOfPlayer;
                this.experience = 50 * tierOfPlayer;
                this.isAlive = true;
            }
            else
            {
                ai = "RangedNormal";
                //Use Normal Ranged Mesh
                this.MeleeDamage = 1 + tierOfPlayer;
                this.RangeDamage = 3 * tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 2 + tierOfPlayer;
                this.Resist = 2 * tierOfPlayer;
                this.experience = 90 * tierOfPlayer;
                this.isAlive = true;
            }
        }
        else if (Enocunter == 2)
        {
            if (aiSelect <= 3)
            {
                ai = "MeleeNormal";
                //Use Normal Normal Mesh
                this.MeleeDamage = 3 * tierOfPlayer;
                this.RangeDamage = 1 + tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 2 * tierOfPlayer;
                this.Resist = 2 + tierOfPlayer;
                this.experience = 90 * tierOfPlayer;
                this.isAlive = true;
            }
            else if (aiSelect > 3 && aiSelect <= 5)
            {
                ai = "MeleeHard";
                //Use Hard Melee Mesh
                this.MeleeDamage = 4 * tierOfPlayer;
                this.RangeDamage = 1 + tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 4 * tierOfPlayer;
                this.Resist = 4 + tierOfPlayer;
                this.experience = 135 * tierOfPlayer;
                this.isAlive = true;
            }
            else if (aiSelect > 5 && aiSelect <= 8)
            {
                ai = "RangedNormal";
                //Use Normal Ranged Mesh
                this.MeleeDamage = 1 + tierOfPlayer;
                this.RangeDamage = 3 * tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 2 + tierOfPlayer;
                this.Resist = 2 * tierOfPlayer;
                this.experience = 90 * tierOfPlayer;
                this.isAlive = true;
            }
            else
            {
                ai = "RangedHard";
                //Use Hard Ranged Mesh
                this.MeleeDamage = 1 + tierOfPlayer;
                this.RangeDamage = 4 * tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 4 + tierOfPlayer;
                this.Resist = 4 * tierOfPlayer;
                this.experience = 135 * tierOfPlayer;
                this.isAlive = true;
            }
        }
        else if (Enocunter == 3)
        {
            if (aiSelect <= 5)
            {
                ai = "MeleeHard";
                //Use Hard Melee Mesh
                this.MeleeDamage = 4 * tierOfPlayer;
                this.RangeDamage = 1 + tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 4 * tierOfPlayer;
                this.Resist = 4 + tierOfPlayer;
                this.experience = 135 * tierOfPlayer;
                this.isAlive = true;
            }
            else
            {
                ai = "RangedHard";
                //Use Hard Ranged Mesh
                this.MeleeDamage = 4 + tierOfPlayer;
                this.RangeDamage = 4 * tierOfPlayer;
                this.health = 10 * tierOfPlayer;
                this.Armor = 4 + tierOfPlayer;
                this.Resist = 4 * tierOfPlayer;
                this.experience = 135 * tierOfPlayer;
                this.isAlive = true;
            }
        }
        else if (Enocunter == 4)
        {
            if (aiSelect <= 5)
            {
                ai = "MeleeHard";
                //Use Very Hard Melee Mesh
                this.MeleeDamage = 6 * tierOfPlayer;
                this.RangeDamage = 6 + tierOfPlayer;
                this.health = 22 * tierOfPlayer;
                this.Armor = 8 * tierOfPlayer;
                this.Resist = 8 + tierOfPlayer;
                this.experience = 240 * tierOfPlayer;
                this.isAlive = true;
            }
            else
            {
                ai = "RangedHard";
                //Use Very Hard Ranged Mesh
                this.MeleeDamage = 6 + tierOfPlayer;
                this.RangeDamage = 6 * tierOfPlayer;
                this.health = 22 * tierOfPlayer;
                this.Armor = 8 + tierOfPlayer;
                this.Resist = 8 * tierOfPlayer;
                this.experience = 240 * tierOfPlayer;
                this.isAlive = true;
            }
        }

    } // end Create Enemy
}
