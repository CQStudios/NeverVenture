using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleControlScript : MonoBehaviour {

    //list of all units participating in battle that require a turn Queue.
    private List<UnitBehavior> Combatants;
    //list of all units in the combatants that are still alive.
    private List<UnitBehavior> ActiveCombatants;

    //Need a bunch of UI elements, and maybe also list the available actions 
    public Button basicattack, movetobutton, blockbutton, castfirebutton, playbutton, cancelbutton;
    public List<RawImage> I;
    public List<Text> T;
    public GameObject CombatUI;
    public UnitBehavior playerObject;
    public GameObject playerRange;

    private Turn PlayerSetTurn;
    private Vector3 PlayerClick;
    
    
    //activeTime = literal seconds since last turn began.
    //turnTimeCutoff = amount of time to allow before cutting off animations (maybe not necessary right now)
    //turnTimeBuffer = amount of extra time to allow for projectiles to finish (just add to turntimeCutoff for now, since we don't have realitme yet.)
    //turnWaitTime = time in between triggering next turns. gives time for death anims.
    private float activeTime = 0, turnTimeCutoff=1.5f, turnTimeBuffer=0.25f, turnWaitTime = 0.25f;

    //determines whether or not the "play" is in progress." playActive terminates if there are no units, a dead player, or when no more turns are Queued from the player.
    //remaining bools are used to make sure we only process one-off actions on the frame we pas the cutoff/buffer times. 
    private bool playActive = false, turnTimeCutoffHit = false, turnTimeBufferHit = false, turnWaitTimeHit = false;

    // Use this for initialization
    void Start () {
        playerObject = transform.GetComponent<OverWorldControllerScript>().getPlayer();
        playerRange = playerObject.transform.Find("RangeIndicator").gameObject;
        cancelbutton.onClick.AddListener(ClickCancel);
        basicattack.onClick.AddListener(ClickAttack);
        castfirebutton.onClick.AddListener(ClickCast);
        movetobutton.onClick.AddListener(ClickMove);
        blockbutton.onClick.AddListener(ClickBlock);
        playbutton.onClick.AddListener(ClickPlay);
    }

    // Update is called once per frame
    private int ActiveCount = -1;
    void Update () {

        UpdateQueueImages();
        //if we have a combatants list, we should remove dead ones from the active list.\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //depends on UnitBehavior.GetHealth() being accurate\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        if (Combatants != null)
        {
            Vector3 UnitHP;
            foreach (UnitBehavior unit in Combatants)
            {
                UnitHP = unit.GetHealth();
                // if wounds plus damage exceed maximum health, remove from activecombatants, and remove UI tags tracking it.
                if ((UnitHP[1] + UnitHP[2] > UnitHP[0]))
                {
                    Kill(unit);
                }
                
            }
        }
        //likely that we have an active battle if the combatants list exists.
        if (ActiveCombatants != null)
        {
            ActiveCount = 0;
            //test to see if any are dead. If any are dead, they can be removed from our list.
            foreach (UnitBehavior o in ActiveCombatants)
            {
                o.inBattle = true;  //make sure live units understand they are in battle.
                if (o.gameObject.GetComponent<EnemyBehaviour>() != null)
                {
                    ActiveCount += 1;
                    //slowly kill active enemies. Kills enemies earliest in list fastest.
                    //DEV AOE
                    //o.DamageHealth(new Vector2(Random.value / (float)ActiveCount / 2.0f, 0.0f));
                }
                /*
                // make sure it has an active tag.
                if (!o.GetComponent<HealthOverTarget>() && !o.isDead)
                {
                    o.gameObject.AddComponent<HealthOverTarget>();
                }
                */
            }
            if (ActiveCount == 0)
            {
                foreach(UnitBehavior o in Combatants)
                {
                    o.inBattle = false; //END COMBAT BECAUSE ALL ENEMY SCRIPTS REPORTED DEATH.
                }
            }
            GameObject.Find("UICanvas/enemycount").GetComponent<Text>().text = "EnemyCount: " + ActiveCount.ToString();
            
        }
        if (ActiveCount <= 0)
        {
            playerRange.SetActive(false);
            ActivateQueueImages(false);
            ActivateButtons(false); //make sure the combat buttons are invisible.
            return; //short if there are no active combatants
        }
        //ActivateQueueImages(true);
        //if we get here, there was an activecombatants list, and there are enemies around.
        //test if there is no turn queue for the player. If so, then unset playActive, as there are no turns left to play.
        if (playerObject.GetCombatActionCount() == 0)
        {
            playActive = false; //there are no turns left to account for or plays to produce
            activeTime = 0;
            //TODO END PLAY 
        }
        //make sure combat buttons are available during combat.
        ActivateButtons(!playActive);

        //ONLY HAPPENS when someone clicked "play" and there are turns available in the player's turn Queue.
        //retains active state until the player's combat action count is 0.
        //in this block we need to keep track of time, 
        if (playActive)
        {

            //IDEALLY, 0=start. 0-cutoff=update. cutoff-buffer=nothing. buffer=end. buffer-wait=nothing. wait=beginNextturn
            //if activetime is 0, play was just pressed or the last turn just ended, so we can start a turn.
            if (activeTime == 0) {
                activeTime = Time.time;
                BeginNextTurn(); //ask the units to begin their next turn.
            }
            //when we get here, we expect a turn to have been started if it needed to be started. if this goes through, it indicates the time for which the turns need their update scripts called.
            if (!turnTimeBufferHit)
            {
                //turnTimeCutoff=1.5f, turnTimeBuffer=0.5f, turnWaitTime = 1f;
                //executes during the time between turn start and 
                if ((Time.time - (turnTimeBuffer + turnTimeCutoff)) > activeTime)
                {
                    turnTimeCutoffHit = true;
                    turnTimeBufferHit = true;
                }
                //Debug.Log("update time frame");
                UpdateCurrentTurn(); //ask the unit's to execute their turn's update func.
            }
            //this is the wait time in between turns after the previous turn has "ended" and before the next turn "begins"
            else if (!turnWaitTimeHit)
            {
                //Debug.Log("active time: " + activeTime.ToString() + "         now: " + (Time.time - (turnTimeBuffer + turnTimeCutoff + turnWaitTime)).ToString());
                if ((Time.time - (turnTimeBuffer + turnTimeCutoff + turnWaitTime)) > activeTime)
                {
                    turnWaitTimeHit = true;

                }
            }

            //previous turn has ended. reset activeTime and end this current turn that has been updating.
            if (turnWaitTimeHit)
            {
                //Debug.Log("wait time proc");
                EndCurrentTurn();
                activeTime = 0;
                //on the next frame, all flags will be reset. if playActive is true, then the next turn will begin.
            }


        }
        else
        {
            //if the mouse button (left) is held down, we update our target location for the last action stacked on the player's queue.
            if (Input.GetMouseButton(0))
            {
                //vector3 of the mouse.
                Vector3 dif = gameObject.GetComponent<OverWorldControllerScript>().GetVector3FromRaycast(Input.mousePosition);
                if (dif == Vector3.zero){return;} //break off if we get shit data from vector3raycast
                                                  //dif = dif - playerObject.transform.position;                
                //if the mouse is held, change the location of the action's direction locator.
                if (playerObject.GetCombatActionCount() > 0)
                {
                    dif = dif - playerObject.GetCombatActionQueue()[playerObject.GetCombatActionCount() - 1].GetOrigin();
                    float range = playerRange.GetComponent<RangeIndicator>().radius;
                    if (dif.magnitude > range)
                    {
                        dif = dif.normalized * range;
                    }
                    PlayerSetTurn = playerObject.GetCombatActionQueue()[playerObject.GetCombatActionCount() - 1];
                    PlayerSetTurn.SetTarget(playerObject.GetCombatActionQueue()[playerObject.GetCombatActionCount() - 1].GetOrigin() + dif);

                }
                else
                {

                }
            }




            //if we get here, playActive is false. this is where we care to handle button presses.
            //buttons ARE active when we get here.

            bool actionselected = false;

            //whenever a button procs, try to add it to the list (handle move).
            if (MoveButtonProc)
            {
                MoveButtonProc = false; //consume the flag and push a move turn to the player queue.
                Turn X = new Turn();
                X.actiontype = "move";  //ensures the turn knows it has a movement type command
                X.ran = playerRange.GetComponent<RangeIndicator>().radius; //preserve range for this action
                X.Start(playerObject);  //ensures the target and targetlocator exist.
                if (!playerObject.PushCombatAction(X))
                {
                    X.Destroy();
                }
                X.TargetLocator.SetActive(true);
                X.ExternalUpdate = X.MoveToLocation;
                actionselected = true;
            }
            //whenever a button procs, try to add it to the list (handle move).
            if (CastButtonProc)
            {
                CastButtonProc = false; //consume the flag and push a casting turn to the player queue.
                Turn X = new Turn();
                X.actiontype = "cast";  //ensures the turn knows it has a cast type command
                X.ran = playerRange.GetComponent<RangeIndicator>().radius; //preserve range for this action
                X.Start(playerObject);  //ensures the target and targetlocator exist.
                X.SetProcTime(0.75f);
                if (!playerObject.PushCombatAction(X))
                {
                    X.Destroy();
                }
                X.TargetLocator.SetActive(true);
                X.ExternalStart = X.CastFireStart;
                X.ExternalUpdate = X.CastFireUpdate;
                X.ExternalEnd = X.CastFireEnd;
                actionselected = true;
            }
            //whenever a button procs, try to add it to the list (handle move).
            if (AttackButtonProc)
            {
                AttackButtonProc = false; //consume the flag and push an attack turn to the player queue.
                Turn X = new Turn();
                X.actiontype = "basicattack";  //ensures the turn knows it has a basic attack type command
                X.ran = playerRange.GetComponent<RangeIndicator>().radius; //preserve range for this action
                X.Start(playerObject);  //ensures the target and targetlocator exist.
                X.SetProcTime(0.6f);
                if (!playerObject.PushCombatAction(X))
                {
                    X.Destroy();
                }
                X.TargetLocator.SetActive(true);
                X.ExternalStart = X.BasicAttackStart;
                X.ExternalUpdate = X.BasicAttackUpdate;
                X.ExternalEnd = X.BasicAttackEnd;
                actionselected = true;
            }
            //whenever a button procs, try to add it to the list (handle move).
            if (BlockButtonProc)
            {
                BlockButtonProc = false; //consume the flag and push a defend turn to the player queue.
                Turn X = new Turn();
                X.actiontype = "block";  //ensures the turn knows it has a block type command
                X.ran = playerRange.GetComponent<RangeIndicator>().radius; //preserve range for this action
                X.Start(playerObject);  //ensures the target and targetlocator exist.
                if (!playerObject.PushCombatAction(X))
                {
                    X.Destroy();
                }
                X.TargetLocator.SetActive(true);
                X.ExternalStart = X.BlockStart;
                X.ExternalUpdate = X.CastFireUpdate; //face that direction.
                X.ExternalEnd = X.BlockEnd;
                actionselected = true;
            }
            //one of the above actions has been chosen this frame. adjust playerRange.
            if (actionselected)
            {
                playerRange.transform.position = playerObject.GetCombatActionQueue()[0].GetOrigin();
                Renderer r1 = playerRange.transform.Find("Plane").GetComponent<Renderer>();
                Renderer r2 = playerObject.GetCombatActionQueue()[playerObject.GetCombatActionCount() - 1].TargetLocator.transform.Find("RangeIndicator/Plane").GetComponent<Renderer>();
                Color col = r2.material.GetColor("_MainColor"); //col.a = 0.25f;
                r1.material.SetColor("_MainColor", col);
            }
        }

	}






    /// <summary>
    /// Removes unit's Combat-given UI tags.
    /// </summary>
    /// <param name="unit">the unit within Combatants which acts as an indexer.</param>
    private void Kill (UnitBehavior unit)
    {
        //remove from fighters list.
        //Debug.Log("Kill_ started");
        ActiveCombatants.Remove(unit);
        //clean up actions
        unit.ClearCombatActionQueue();
        /*
        //remove it's health over target entity.
        if (unit.gameObject.GetComponent<HealthOverTarget>())
        {
            Destroy(unit.gameObject.GetComponent<HealthOverTarget>());
        }
        */
        //Debug.Log("Kill_ Ended");
    }

    //////////////////////  GETTERS AND SETTERS \\\\\\\\\\\\\\\\\\\\\\\\\
    public void SetCombatants(List<UnitBehavior> newcombatants)
    {
        if (Combatants != null)
        {
            foreach (UnitBehavior o in Combatants)
            {
                if (!newcombatants.Contains(o))
                {
                    Kill(o);    //remove battle tags from entities that aren't in the new set.
                }
            }
        }
        Combatants = newcombatants;
        if (ActiveCombatants != null)
        {
            ActiveCombatants.Clear();
        }
        ActiveCombatants = new List<UnitBehavior>();
        foreach (UnitBehavior o in Combatants)
        {
            ActiveCombatants.Add(o);
        }
    }
    public List<UnitBehavior> GetCombatants()
    {
        return Combatants;
    }
    public List<UnitBehavior> GetAliveCombatants()
    {
        return ActiveCombatants;
    }



    /// <summary>
    /// tells all active combatants to begin their next turn sequence.
    /// </summary>
    public void BeginNextTurn()
    {
        foreach  (UnitBehavior unit in ActiveCombatants)
        {
            unit.BeginAction();
        }
        //reset timer hit flags 
        turnTimeBufferHit = false;
        turnTimeCutoffHit = false;
        turnWaitTimeHit = false;
    }

    /// <summary>
    /// executes the continueAction function for each active turn taking unit.
    /// </summary>
    public void UpdateCurrentTurn()
    {
        foreach (UnitBehavior unit in ActiveCombatants)
        {
            unit.ContinueAction();
        }
    }

    
    public void EndCurrentTurn()
    {
        foreach(UnitBehavior unit in ActiveCombatants)
        {
            unit.EndAction();
        }
    }

    /// <summary>
    /// disables/enables all the combat related buttons.
    /// </summary>
    public void ActivateButtons(bool b)
    {
        basicattack.gameObject.SetActive(b);
        movetobutton.gameObject.SetActive(b);
        blockbutton.gameObject.SetActive(b);
        castfirebutton.gameObject.SetActive(b);
        playbutton.gameObject.SetActive(b);
        cancelbutton.gameObject.SetActive(b);
    }

    /// <summary>
    /// disables/enables all the combat related images/text
    /// </summary>
    /// <param name="b"></param>
    public void ActivateQueueImages(bool b)
    {
        foreach(RawImage img in I){img.gameObject.SetActive(b);}
    }

    private bool AttackButtonProc, MoveButtonProc, BlockButtonProc, CastButtonProc;

    /// <summary>
    /// Make the MoveTo Button do this onclick.
    /// Sets the proc bool for move to location..... possibly does cleanup code for it?
    /// </summary>
    public void ClickBlock()
    {
        ClearProcBools();
        playerRange.SetActive(true);
        BlockButtonProc = true;
        //REEEEEEEEEE - todo, change hard coded float to query the ranges from the player.
        playerRange.GetComponent<RangeIndicator>().radius = 1.5f;
    }

    /// <summary>
    /// Make the MoveTo Button do this onclick.
    /// Sets the proc bool for move to location..... possibly does cleanup code for it?
    /// </summary>
    public void ClickCast()
    {
        ClearProcBools();
        playerRange.SetActive(true);
        CastButtonProc = true;
        //REEEEEEEEEE - todo, change hard coded float to query the ranges from the player.
        playerRange.GetComponent<RangeIndicator>().radius = 7f;
    }

    /// <summary>
    /// Make the MoveTo Button do this onclick.
    /// Sets the proc bool for move to location..... possibly does cleanup code for it?
    /// </summary>
    public void ClickMove()
    {
        ClearProcBools();
        playerRange.SetActive(true);
        MoveButtonProc = true;
        //REEEEEEEEEE - todo, change hard coded float to query the ranges from the player.
        playerRange.GetComponent<RangeIndicator>().radius = 2.5f;
    }

    /// <summary>
    /// Make the attack Button do this onclick.
    /// Sets the proc bool for attack things..... possibly does cleanup code for it?
    /// </summary>
    public void ClickAttack()
    {
        ClearProcBools();
        playerRange.SetActive(true);
        AttackButtonProc = true;
        //REEEEEEEEEE - todo, change hard coded float to query the ranges from the player.
        playerRange.GetComponent<RangeIndicator>().radius = 1.75f;
    }


    /// <summary>
    /// Give as an onclick to the cancel button.
    /// </summary>
    public void ClickCancel()
    {
        int Num = playerObject.GetCombatActionCount() - 1;
        foreach(UnitBehavior unit in ActiveCombatants)
        {
            unit.PreserveFirstNActions(Num);
        }
        if (playerObject.GetCombatActionCount() > 0)
        {
            playerRange.transform.position = playerObject.GetCombatActionQueue()[0].GetOrigin();
            playerRange.GetComponent<RangeIndicator>().radius = playerObject.GetCombatActionQueue()[playerObject.GetCombatActionCount() -1].GetRange();
            playerObject.GetCombatActionQueue()[playerObject.GetCombatActionCount() - 1].SetRangeColor(playerObject.gameObject);

        }
        else
        {
            playerRange.transform.position = playerObject.transform.position;
            playerRange.SetActive(false);
        }
        ClearProcBools();
    }

    /// <summary>
    /// add to onclick of play button.
    /// would attempt to begin turns.
    /// </summary>
    public void ClickPlay()
    {
        ClearProcBools();
        playerRange.SetActive(false);
        playActive = true;
    }


    /// <summary>
    /// clears flags before setting them.
    /// </summary>
    private void ClearProcBools()
    {
        BlockButtonProc = false;
        AttackButtonProc = false;
        MoveButtonProc = false;
        CastButtonProc = false;
        //playerRange.SetActive(false);
    }









    private void UpdateQueueImages()
    {

    }

}
