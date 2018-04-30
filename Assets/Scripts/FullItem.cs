using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullItem {

    public bool hasBlueprint;


    public string name;
    public string prefix;
    public string suffix;
    public float baseDamage;
    public int basicDamage;
    public int attackType;   //
    public int mesh;         //Change as Needed ---------- Check Assign Values Function
    public int animation;    //
    public int enchantType;
    public int enchantTier;
    public int tier;
    public float price;
    public Item baseItem;
    public int damageReduction;
    public float woundResist;
    public int regneration;
    public int recovery;
    public int slot;
    public int id;
    

    //Special Damage Types
    //   Physical
    //         none         //0
    public int bludgeoning; //1
    public int piercing;    //2
    public int slashing;    //3
    public int sonic;       //4
    public int sonicTier;
    public int acid;        //5
    public int acidTier;
    public int narcotic;    //6
    public int narcoticTier;

    //   Magical
    public int arcane;      //7
    public int radiant;     //8
    public int necrotic;    //9
    public int fire;        //10
    public int fireTier;
    public int cold;        //11
    public int coldTier;
    public int shock;       //12
    public int shockTier;

    //Special Damage Resistance
    public int mundane;     //13
    public int mundaneTier;
    public int alchemical;  //14
    public int alchemicalTier;
    public int mystical;    //15
    public int mysticalTier;
    public int elemental;   //16
    public int elementTier;


    /// <summary>
    /// Full Item Constructor
    /// For use in the Equipment Slot for Enemies. Request an item using the Stash values and equip an item with the proper upgrades
    /// </summary>
    /// <param name="ID">Identification number for the Item</param>
    /// <param name="tier">Number of upgrades to the direct damage</param>
    /// <param name="enchant">Which enchant was chosen for the equipped item</param>
    /// <param name="enchantTier">Number of upgrades to the selected enchantment</param>
    //
    public FullItem(int ID, int tier, int enchant, int enchantTier)
    {

        ItemDatabase database = new ItemDatabase();

        baseItem = database.getItem(ID);
        this.baseDamage = baseItem.damage;
        this.basicDamage = baseItem.basicDamage;
        this.attackType = baseItem.attackType;
        this.mesh = baseItem.mesh;
        this.animation = 0;
        this.enchantType = 0;
        this.enchantTier = 0;
        assignItemValues();
        this.tier = tier;
        assignTierDamage(tier);

        this.enchantType = enchant;
        assignEnchantmentTier(enchant, enchantTier);
       
        assignDamageType(enchant);
        assignPrefix(tier);

        
    }




    /// <summary>
    /// Full Item contructor
    /// For storage use only(Stash). Use to build inital player stash. Loads an Item using the ID and assigns 
    /// the full item at zero values for the tier, enchantments and every other value that is upgraed in game
    /// </summary>
    /// <param name="id">Identification number for the Item</param>
    //

    public FullItem(int id)
    {

        ItemDatabase database = new ItemDatabase();

        this.id = id;
        this.baseItem = database.getItem(id);

        this.hasBlueprint = false;
        this.name = baseItem.itemName;
        this.baseDamage = baseItem.damage;
        this.basicDamage = baseItem.basicDamage;
        this.attackType = baseItem.attackType;
        this.mesh = baseItem.mesh;
        this.animation = 0;
        this.enchantType = 0;
        this.enchantTier = 0;
        this.tier = 1;
        this.price = baseItem.price;
        this.damageReduction = baseItem.reduction;
        this.woundResist = baseItem.woundResist;
        this.regneration = baseItem.regeneration;
        this.recovery = baseItem.recovery;
        this.slot = baseItem.slot;
        this.bludgeoning = 0;
        this.piercing = 0;
        this.slashing = 0;
        this.sonic = 0;
        this.sonicTier = 0;
        this.acid = 0;
        this.acidTier = 0;
        this.narcotic = 0;
        this.narcoticTier = 0;
        this.arcane = 0;
        this.radiant = 0;
        this.necrotic = 0;
        this.fire = 0;
        this.fireTier = 0;
        this.cold = 0;
        this.coldTier = 0;
        this.shock = 0;
        this.shockTier = 0;
        this.mundane = 0;
        this.mundaneTier = 0;
        this.alchemical = 0;
        this.alchemicalTier = 0;
        this.mystical = 0;
        this.mysticalTier = 0;
        this.elemental = 0;
        this.elementTier = 0;
        assignDamageType(baseItem.basicDamage);

    }





    public void assignItem(int enchant)
    {
        assignTierDamage(this.tier);
        assignDamageType(enchant);
        assignPrefix(this.tier);
        this.name = prefix + name + suffix;
        
    }




    /// <summary>
    /// This Area of the Full Item will return values of selected item with upgrades and enchantments
    /// </summary>

    void assignItemValues()
    {
        this.name = baseItem.itemName;
        this.baseDamage = baseItem.damage;
        this.damageReduction = baseItem.reduction;
        this.woundResist = baseItem.woundResist;
        this.regneration = baseItem.regeneration;
        this.recovery = baseItem.recovery;
        this.price = baseItem.price;
        this.slot = baseItem.slot;
        this.id = baseItem.id;



        //Change This Later For Animations and Meshes
        this.attackType = baseItem.attackType;
        this.mesh = baseItem.mesh;
    }


    //Gives the Item a damage type using assignMagicDamage
    void assignDamageType(int enchantType)
    {
        resetValues();
        int basicBonus = (int)this.basicDamage;
        ////////////////////////Basic Damage\\\\\\\\\\\\\\\\\\\\

        for (int i = 1; i <= this.tier; i++)
        {
            basicBonus += i;
        }
        if (this.basicDamage == 1)
        {
            this.bludgeoning = basicBonus;
        }
        else if (this.basicDamage == 2)
        {
            this.piercing = basicBonus;
        }
        else if (this.basicDamage == 3)
        {
            this.slashing = basicDamage;
        }

        else if (this.basicDamage == 7)
        {
            this.arcane = basicDamage;
        }

        else if (this.basicDamage == 8)
        {
            this.necrotic = basicDamage;
        }

        else if (this.basicDamage == 9)
        {
            this.radiant = basicDamage;
        }

        else { }



        switch (enchantType)
        {
            


            ///////////////////////Damage Assignment\\\\\\\\\\\\\\\\
            case 0:
                break;
            case 4:
                sonic = assignMagicDamage(sonicTier);
                    suffix = " of Thunder";
                break;
            case 5:
                acid = assignMagicDamage(acidTier);
                    suffix = " of Acid";
                break;
            case 6:
                narcotic = assignMagicDamage(narcoticTier);
                    suffix = " of Tranquillity";
                break;
            case 10:
                fire = assignMagicDamage(fireTier);
                    suffix = " of Flames";
                break;
            case 11:
                cold = assignMagicDamage(coldTier);
                    suffix = " of Chill";
                break;
            case 12:
                shock = assignMagicDamage(shockTier);
                    suffix = " of Lightning";
                break;

            //////////////////////Armor Resistance\\\\\\\\\\\\\\\\\\\\\  
            case 13:
                mundane = assignMagicDamage(mundaneTier);
                suffix = " of the Mundane";
                break;
            case 14:
                alchemical = assignMagicDamage(alchemicalTier);
                suffix = " of the Alchemist";
                break;
            case 15:
                mystical = assignMagicDamage(mysticalTier);
                suffix = " of the Mystic";
                break;
            case 16:
                elemental = assignMagicDamage(elementTier);
                suffix = " of the Elements";
                break;
            default:
                break;
        }
    }

    void assignTierDamage(int tier)
    {
        float tempDamage = baseDamage;
        float tempArmor = damageReduction;
        int bonus = attackType;

        //Get Values for Weapons
        if (baseDamage > 0)
        {
            for (int i = 1; i <= tier; i++)
            {
                tempDamage = tempDamage + bonus;
                bonus += 1;
                if (i % 3 == 1)
                {
                    bonus = bonus * 2;
                }
            }


        }// End Weapons

        //Light Armor
        if (damageReduction == 1)
        {
            for (int i = 1; i <= tier; i++)
            {
                tempArmor = tempArmor + bonus;
                bonus += 1;
                if (i % 3 == 0)
                {
                    bonus = bonus + (int)(bonus/4);
                }
            }

        }

        //Med Armor
        if (damageReduction == 3)
        {
            for (int i = 1; i <= tier; i++)
            {
                tempArmor = tempArmor + bonus;
                bonus += 1;
                if (i % 3 == 0)
                {
                    bonus = bonus + (int)(bonus / 2);
                }
            }
        }

        //Heavy Armor
        if (damageReduction == 4)
        {
            for (int i = 1; i <= tier; i++)
            {
                tempArmor = tempArmor + bonus;
                bonus += 1;
                if (i % 3 == 0)
                {
                    bonus = bonus * 2;
                }
            }
        }

        if (baseDamage > 0)
        {
            baseDamage = tempDamage + tier + sonicTier + acidTier + narcoticTier;
        }
        else if (damageReduction > 0)
        {
            damageReduction = (int)tempArmor + tier + mundaneTier + alchemicalTier + mysticalTier + elementTier;
        }
    }// End Tier Value Assignment

    //Gives the Item the amount of magic damage
    int assignMagicDamage(int mTier)
    {
        int magicDamage = 0;
        for (int i = 0; i <= mTier; i++)
        {
            magicDamage += i;
        }

        return magicDamage;
    }


    void resetValues()
    {
        bludgeoning = 0;
        piercing = 0;
        slashing = 0;
        acid = 0;
        sonic = 0;
        narcotic = 0;
        arcane = 0;
        necrotic = 0;
        radiant = 0;
        fire = 0;
        cold = 0;
        shock = 0;
        mundane = 0;
        alchemical = 0;
        mystical = 0;
        elemental = 0;
    }

    
    void assignPrefix(int tier)
    {
        switch (tier)
        {
            case 0:
                prefix = "Old ";
                break;
            case 1:
                prefix = "";
                break;
            case 2:
                prefix = "Reworked ";
                break;
            case 3:
                prefix = "Fine ";
                break;
            case 4:
                prefix = "Masterwork ";
                break;
            case 5:
                prefix = "Remarkable ";
                break;
            case 6:
                prefix = "Grand ";
                break;
            case 7:
                prefix = "Profound ";
                break;
            case 8:
                prefix = "Legendary ";
                break;
        }
    }


    void assignEnchantmentTier(int enchant, int tier)
    {
        switch (enchant)
        {
            case 0:
                break;
            case 4:
                sonicTier = tier - 1;
                break;
            case 5:
                acidTier = tier - 1;
                break;
            case 6:
                narcoticTier = tier - 1;
                break;
            case 10:
                fireTier = tier - 1;
                break;
            case 11:
                coldTier = tier - 1;
                break;
            case 12:
                shockTier = tier - 1;
                break;
            case 13:
                mundaneTier = tier - 1;
                break;
            case 14:
                alchemicalTier = tier - 1;
                break;
            case 15:
                mysticalTier = tier - 1;
                break;
            case 16:
                elementTier = tier - 1;
                break;
            default:
                break;

        }
    }
}
