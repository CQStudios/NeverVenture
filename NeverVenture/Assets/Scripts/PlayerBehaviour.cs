using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour {

    

    //Player Stats
    public int health;
    public int wounds = 0;
    public int maxHealth;
    private float healthRegen;
    private int effectiveHealth;

    private float energy;
    private float energyRegen;
    private int maxEnergy;
    private float exaughstion;

    private int level = 1;
    private int tier;
    private float experience = 10;
    private int nextLevel = 1000;
    private bool hasLevelUp;
    private int levelsGained;
    private int pointsToDistribute;

    //UI
    public Sprite playerImage;
   // public Slider playerHealth;
    //public Slider playerEnergy;
    public Text playerName;
    public Text textLevel;
    public Text textTier;
    public Text textHelath;
    public Text textEnergy;
    public Text textExperience;
    public Text textBrawn;
    public Text textAgility;
    public Text textVitality;
    public Text textIntelligence;
    public Text textWisdom;
    public Text textWillpower;
    public Text textInventory;
    public Text textDefense;
    public Text textMelee;
    public Text textRanged;
    public Text textStatPoints;
    public Text textWeaponName;
    public Text textArmorName;
    public Text textRingName;
    public Text textFocusName;
    public Text textTrinketName;
    public Text textAmuletName;


    //Display Weapon Info
    public Text textWeaponDamage;
    public Text textBluntValue;
    public Text textSlashingValue;
    public Text textPiercingValue;
    public Text textAcidValue;
    public Text textNarcoticValue;
    public Text textPoisonValue;

    //Display Armor Info
    public Text textArmorDamageReduction;
    public Text textArmorWoundResist;
    public Text textArmorMundane;
    public Text textArmorAlc;
    public Text textArmorRecovery;
    public Text textArmorSpeed;




    public int statinorout;
    public float BlockValue;
    public float physDamage;
    public float magicDamage;



    //Damage Types   -   Modified by Equipped Items
    public float blunt;
    public float slash;
    public float pierce;
    public float acid;
    public float narco;
    public float sonic;
    public float arcane;
    public float necro;
    public float radiant;
    public float fire;
    public float cold;
    public float shock;


    //Resist Types  -   Modified by Equipped Items
    public float mundane;    //Blunt, Piercing, Slashing
    public float alchemical; //Acid, Narcotic, Sonic
    public float mystical;   //Arcane, Necromatic, Radiant
    public float elemental;

    //Limits till Debuffs
    ///Physical
    ////Mundane
    public float bluntDamageLimit = 100;
    public float bluntDamageDebuff = 20;
    public float bluntDamage;
    public float slashDamageLimit = 100;
    public float slashDamageDebuff = 20;
    public float slashDamage;
    public float piercingDamageLimit = 100;
    public float piercingDamageDebuff = 20;
    public float piercingDamage;
    ////Alchemical
    public float acidDamageLimit = 100;
    public float acidDamageDebuff = 20;
    public float acidDamage;
    public float narcoticDamageLimit = 100;
    public float narcoticDamageDebuff = 20;
    public float narcoticDamage;
    public float sonicDamageLimit = 100;
    public float sonicDamageDebuff = 20;
    public float sonicDamage;
    ////Mystical
    public float arcaneDamageLimit = 100;
    public float arcaneDamageDebuff = 20;
    public float arcaneDamage;
    public float necroticDamageLimit = 100;
    public float necroticDamageDebuff = 20;
    public float necroticDamage;
    public float radiantDamageLimit = 100;
    public float radiantDamageDebuff = 20;
    public float radiantDamage;
    //Elemental
    public float fireDamageLimit = 100;
    public float fireDamageDebuff = 20;
    public float fireDamage;
    public float coldDamageLimit = 100;
    public float coldDamageDebuff = 20;
    public float coldDamage;
    public float shockDamageLimit = 100;
    public float shockDamageDebuff = 20;
    public float shockDamage;


    //Inventories for the player. 
    //Equipment: 6 Full Items on this list 
    //0: Brawn (Physical Weapon ID 1000s)
    //1: Agility (Armor ID 2000s)
    //3: Vitallity (Ring ID 3000s)
    //3: Intelligence (Focus ID 4000s)
    //4: Wisdom (Trinket ID 5000s)
    //5: Willpower (Amulet ID 6000s)
    
    public List<FullItem> Equipment = new List<FullItem>();
    public List<FullItem> Stash = new List<FullItem>();
    


    public int brawn, agility, vitality, intellegience, wisdom, willpower;
    private int brawnBonus, agilityBonus, vitalityBonus, intelligenceBonus, wisomBonus, willpowerBonus;


    //Stats Modified by Equipment
    private int armorValue = 2;
    private int regenValue, recoveryValue;
    private int weaponValue = 4;
    private int rangeValue = 4;

    public int Defense;
    public int Melee;
    public int Range;


    //Menu Stats for Leveling Up
    private int startPoints;
    private int brawnstart, astart, vstart, istart, domstart, powstart; 

    //public List<FullItem> Inventory = new List<FullItem>();



    // Use this for initialization
    void Start() {
        CreateTestStats();
        statinorout = 0;
        pointsToDistribute = 10;
        startPoints = pointsToDistribute; // REMOVE
        brawnstart = brawn;
        astart = agility;
        vstart = vitality;
        istart = intellegience;
        domstart = wisdom;
        powstart = willpower;
        setPlayerUI();

    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update() {
        
    }


    void calculateStats()
    {
        //Set Stat bonuses
        brawnBonus = (int)(brawn / 4);
        agilityBonus = (int)(agility / 4);
        vitalityBonus = (int)(vitality / 4);
        intelligenceBonus = (int)(intellegience / 4);
        wisomBonus = (int)(wisdom / 4);
        willpowerBonus = (int)(willpower / 4);

        //Set Max Values
        maxHealth = brawn + agility + (vitality * tier) + (10 * tier) + (level + vitalityBonus);
        health = maxHealth - wounds;
        healthRegen = vitalityBonus * tier + willpowerBonus + regenValue;

        maxEnergy = intellegience + wisdom + (willpower * tier) + (level * willpowerBonus);
        energy = maxEnergy - exaughstion;
        energyRegen = willpowerBonus * tier + vitalityBonus + recoveryValue;

        //Find Armor and Hit Values

        //Raw Damage
        physDamage = brawnBonus + Equipment[0].baseDamage + tier;
        magicDamage = intelligenceBonus + Equipment[3].baseDamage + tier;





        //Basic Damages
        Defense = Equipment[1].damageReduction + Equipment[4].damageReduction + agilityBonus + wisomBonus;

        //Mundane
        blunt = Equipment[0].bludgeoning;
        slash = Equipment[0].slashing;
        pierce = Equipment[0].piercing;
        //Mystical
        arcane = Equipment[3].arcane;
        necro = Equipment[3].necrotic;
        radiant = Equipment[3].radiant;

        //Enchant Damages
        //Alchemical
        acid = Equipment[0].acid;
        sonic = Equipment[0].sonic;
        narco = Equipment[0].narcotic;
        //Elemental
        fire = Equipment[3].fire;
        cold = Equipment[3].cold;
        shock = Equipment[3].shock;


        //Armor Resistances
        mundane = Equipment[1].mundane + Equipment[2].mundane + Equipment[4].mundane + Equipment[5].mundane;
        alchemical = Equipment[1].alchemical + Equipment[2].alchemical + Equipment[4].alchemical + Equipment[5].alchemical;
        mystical = Equipment[1].mystical + Equipment[2].mystical + Equipment[4].mystical + Equipment[5].mystical;
        elemental = Equipment[1].elemental + Equipment[2].elemental + Equipment[4].elemental + Equipment[5].elemental;

        //Assign Resistences from Armors

        //Mundane
        bluntDamageLimit = 100 + (mundane * 4);
        bluntDamageDebuff = 20 + (mundane * 2);
        slashDamageLimit = 100 + (mundane * 4);
        slashDamageDebuff = 20 + (mundane * 2);
        piercingDamageLimit = 100 + (mundane * 4);
        piercingDamageDebuff = 20 + (mundane * 2);

        //Alchemical
        acidDamageLimit = 100 + (alchemical * 4);
        acidDamageDebuff = 20 + (alchemical * 2);
        narcoticDamageLimit = 100 + (alchemical * 4);
        narcoticDamageDebuff = 20 + (alchemical * 2);
        sonicDamageLimit = 100 + (alchemical * 4);
        sonicDamageDebuff = 20 + (alchemical * 2);

        //Mystical
        arcaneDamageLimit = 100 + (mystical * 4);
        arcaneDamageDebuff = 20 + (mystical * 2);
        necroticDamageLimit = 100 + (mystical * 4);
        necroticDamageDebuff = 20 + (mystical * 2);
        radiantDamageLimit = 100 + (mystical * 4);
        radiantDamageDebuff = 20 + (mystical * 2);

        
    }


    void setPlayerUI()
    {

        setTextBoxes();

    }


    void setTextBoxes()
    {
        playerName.text = "Neven";
        textLevel.text = level.ToString();
        textTier.text = tier.ToString();
        //playerHealth.minValue = 0;
        //playerHealth.maxValue = maxHealth;
        //playerHealth.value = health;
        //playerEnergy.minValue = 0;
        //playerEnergy.maxValue = maxEnergy;
        //playerEnergy.value = energy;
        textHelath.text = health + " / " + maxHealth;
        textEnergy.text = energy + " / " + maxEnergy;
        textExperience.text = experience + " / " + nextLevel;
        textBrawn.text = brawn + "                              + " + brawnBonus;
        textAgility.text = agility + "                              + " + agilityBonus;
        textVitality.text = vitality + "                              + " + vitalityBonus;
        textIntelligence.text = intellegience + "                              + " + intelligenceBonus;
        textWisdom.text = wisdom + "                              + " + wisomBonus;
        textWillpower.text = willpower + "                              + " + willpowerBonus;
        textDefense.text = Defense.ToString();
        textMelee.text = physDamage.ToString() + "  (+" + brawnBonus.ToString() + ")";
        textRanged.text = magicDamage.ToString();
        textStatPoints.text = pointsToDistribute.ToString();

        if (Equipment.Count > 0)
        {
            textWeaponName.text = Equipment[0].name;
            textWeaponDamage.text = Equipment[0].baseDamage.ToString();
            textBluntValue.text = Equipment[0].bludgeoning.ToString();
            textSlashingValue.text = Equipment[0].slashing.ToString();
            textPiercingValue.text = Equipment[0].piercing.ToString();
            textAcidValue.text = Equipment[0].acid.ToString();
            textNarcoticValue.text = Equipment[0].narcotic.ToString();
            textPoisonValue.text = Equipment[0].sonic.ToString();
        }

        if (Equipment.Count > 1)
        {
            textArmorName.text = Equipment[1].name;
            textArmorDamageReduction.text = Equipment[1].damageReduction.ToString();
            textArmorWoundResist.text = Equipment[1].woundResist.ToString() + "%";
            textArmorMundane.text = Equipment[1].alchemical.ToString();
            textArmorRecovery.text = Equipment[1].recovery.ToString();
        }

        //Change Later
        textArmorSpeed.text = "";

        if (Equipment.Count > 5)
        {
            textRingName.text = Equipment[2].name;
            textFocusName.text = Equipment[3].name;
            textTrinketName.text = Equipment[4].name;
            textAmuletName.text = Equipment[5].name;
        }

    }

    //Finds level based on current experince and applies the level without a level up indicator
    void loadCurrentCharacterLevel()
    {
        nextLevel = nextLevel + (level * 1000);
        level += 1;
    }
    void Add(int a)
    {
        statinorout = a;
    }
    void Sub(int a)
    {
        statinorout = a;
    }
    void StatIncrease(string stat)
    {
       if(stat == "BrawnValue" && pointsToDistribute >= 0 && pointsToDistribute <= startPoints)
        {
            if (pointsToDistribute <= startPoints && statinorout != -1 && (pointsToDistribute - statinorout != -1) && (brawn + statinorout) >= brawnstart || pointsToDistribute == 0 && startPoints > 0 && statinorout == -1 && (brawn + statinorout) >= brawnstart)
            {
                brawn = brawn + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
            else if(pointsToDistribute < startPoints && (pointsToDistribute - statinorout != -1) && (brawn + statinorout) >= brawnstart)
            {
                brawn = brawn + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
        }
        if (stat == "AgilityValue" && pointsToDistribute >= 0 && pointsToDistribute <= startPoints)
        {
            if (pointsToDistribute <= startPoints && statinorout != -1 && (pointsToDistribute - statinorout != -1) && (agility + statinorout) >= astart || pointsToDistribute == 0 && startPoints > 0 && statinorout == -1 && (agility + statinorout) >= astart)
            {
                agility = agility + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
            else if (pointsToDistribute < startPoints && (pointsToDistribute - statinorout != -1) && (agility + statinorout) >= astart)
            {
                agility = agility + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
        }
        if (stat == "VitalityValue" && pointsToDistribute >= 0 && pointsToDistribute <= startPoints)
        {
            if (pointsToDistribute <= startPoints && statinorout != -1 && (pointsToDistribute - statinorout != -1) && (vitality + statinorout) >= vstart || pointsToDistribute == 0 && startPoints > 0 && statinorout == -1 && (vitality + statinorout) >= vitality)
            {
                vitality = vitality + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
            else if (pointsToDistribute < startPoints && (pointsToDistribute - statinorout != -1) && (vitality + statinorout) >= vstart)
            {
                vitality = vitality + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
                //print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
        }
        if (stat == "WisdomValue" && pointsToDistribute >= 0 && pointsToDistribute <= startPoints)
        {
            if (pointsToDistribute <= startPoints && statinorout != -1 && (pointsToDistribute - statinorout != -1) && (wisdom + statinorout) >= domstart || pointsToDistribute == 0 && startPoints > 0 && statinorout == -1 && (wisdom + statinorout) >= domstart)
            {
                wisdom = wisdom + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
                //print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
            else if (pointsToDistribute < startPoints && (pointsToDistribute - statinorout != -1) && (wisdom + statinorout) >= domstart)
            {
                wisdom = wisdom + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
        }
        if (stat == "IntValue" && pointsToDistribute >= 0 && pointsToDistribute <= startPoints)
        {
            if (pointsToDistribute <= startPoints && statinorout != -1 && (pointsToDistribute - statinorout != -1) && (intellegience + statinorout) >= istart || pointsToDistribute == 0 && startPoints > 0 && statinorout == -1 && (intellegience + statinorout) >= istart)
            {
                intellegience = intellegience + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
            else if (pointsToDistribute < startPoints && (pointsToDistribute - statinorout != -1) && (intellegience + statinorout) >= istart)
            {
                intellegience = intellegience + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
                //print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
        }
        if (stat == "WillValue" && pointsToDistribute >= 0 && pointsToDistribute <= startPoints )
        {
            if (pointsToDistribute <= startPoints && statinorout != -1 && (pointsToDistribute - statinorout != -1) && (willpower + statinorout) >= powstart || pointsToDistribute == 0 && startPoints > 0 && statinorout == -1 && (willpower + statinorout) >= powstart)
            {
                willpower = willpower + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
                //print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
            else if (pointsToDistribute < startPoints && (pointsToDistribute - statinorout != -1) && (willpower + statinorout) >= powstart)
            {
                willpower = willpower + (1 * statinorout);
                calculateStats();
                setPlayerUI();
                pointsToDistribute = pointsToDistribute - (1 * statinorout);
               // print(pointsToDistribute);
                textStatPoints.text = pointsToDistribute.ToString();

            }
        }
        
    }
    void ConfirmStats()
    {
        startPoints = pointsToDistribute;
        calculateStats();
        setPlayerUI();
        brawnstart = brawn;
        astart = agility;
        istart = intellegience;
        vstart = vitality;
        domstart = wisdom;
        powstart = willpower;
    }

    void CreateTestStats()
    {
        //Create a number generator
        System.Random rnd = new System.Random();

        //Set Level
        experience = 100000;

        //Use experience to find current level
        while (experience >= nextLevel)
        {
            loadCurrentCharacterLevel();
        }

        //Set Level Tier
        tier = (int)(level / 5) + 1;


        //Set Main Stats
        this.brawn = rnd.Next(1, 10);
        this.agility = rnd.Next(1, 10);
        this.vitality = rnd.Next(1, 10);
        this.intellegience = rnd.Next(1, 10);
        this.wisdom = rnd.Next(1, 10);
        this.willpower = rnd.Next(1, 10);


        createStash();
        createEquipment();

        

        
        int itemSelect = rnd.Next(1, 8);
        int itemEnchant = rnd.Next(4, 6);

        equipItem(1001, 4);
        //equipItem(1000 + itemSelect, itemEnchant);
        
        equipItem(2003, 13);
        equipItem(3001, 0);
        equipItem(4001, 10);
        equipItem(5001, 13);
        equipItem(6001, 0);

        for (int i = 0; i < Equipment.Count; i++)
        {
            Debug.Log(Equipment[i].name);
        }

        calculateStats();

        

       setPlayerUI();
    }

    void gainLevel()
    {
        nextLevel = nextLevel + (level * 1000);
        level += 1;
        hasLevelUp = true;
        levelsGained += 1;
    }

    

    //////////////////////////////////////////Saving and Loading\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    public PlayerData exportPlayerData()
    {
        PlayerData data = new PlayerData(brawn, agility, vitality, intellegience, wisdom, willpower, experience, Stash, Equipment);

        return data;
    }



    public void importPlayerData(PlayerData data)
    {
        setBrawn(data.brawn);
        setAgility(data.agility);
        setVitality(data.vitality);
        setIntelligence(data.intelligence);
        setWisdom(data.wisdom);
        setWillpower(data.willpower);
        setExperience(data.experience);
        setStash(data.Stash);
        setEquipment(data.Equipment);
       

    }

    public void equipItem(int ID, int enchant)
    {

        //Find Item from the Stash using the ID and equip the Item to that slot
        for (int i = 0; i <= Stash.Count; i++)
        {
            
            if (Stash[i].id == ID)
            {
                Equipment[Stash[i].slot] = Stash[i];
                
                
                break;
            }
        }
        

        //Using the ID, grab the desired enchantment and assign it to the Item
        for (int i = 0; i <= Equipment.Count; i++)
        {
            if (Equipment[i].id == ID)
            {

                Equipment[i].assignItem(enchant);
                
                break;
            }
        }

        
        

    }

    //Use for inital Character creation
    private void createStash()
    {
        Stash.Add(new FullItem(0));
        Stash.Add(new FullItem(1001));       
        Stash.Add(new FullItem(1002));       
        Stash.Add(new FullItem(1003));
        Stash.Add(new FullItem(1004));
        Stash.Add(new FullItem(1005));
        Stash.Add(new FullItem(1006));
        Stash.Add(new FullItem(1007));
        Stash.Add(new FullItem(1008));
        Stash.Add(new FullItem(2001));        
        Stash.Add(new FullItem(2002));
        Stash.Add(new FullItem(2003));
        Stash.Add(new FullItem(3001));
        Stash.Add(new FullItem(4001));
        Stash.Add(new FullItem(4002));
        Stash.Add(new FullItem(4003));
        Stash.Add(new FullItem(4004));
        Stash.Add(new FullItem(4005));
        Stash.Add(new FullItem(4006));
        Stash.Add(new FullItem(5001));
        Stash.Add(new FullItem(5002));
        Stash.Add(new FullItem(5003));
        Stash.Add(new FullItem(6001));      
    }

    //Used for inital Character creation
    private void createEquipment()
    {
        for (int i = 0; i < 6; i++)
        {
            Equipment.Add(new FullItem(0));
        }
        
    }


    /////////////////////////////////////////////////Getters and Setters\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    //Setters
    void setBrawn(int stat)
    {
        brawn = stat;
    }

    void setAgility(int stat)
    {

    }

    void setVitality(int stat)
    {
        vitality = stat;
    }

    void setIntelligence(int stat)
    {
        intellegience = stat;
    }

    void setWisdom(int stat)
    {
        wisdom = stat;
    }

    void setWillpower(int stat)
    {
        willpower = stat;
    }
    void setExperience(float stat)
    {
        experience = stat;
    }

    void setWounds(int stat)
    {
        wounds = stat;
    }
    void setStash(List<FullItem> stat)
    {
        Stash = stat;
    }
    void setEquipment(List<FullItem> stat)
    {
        Equipment = stat;
    }
}
