using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase {

    //Physical Weapons [Slot 0] Used to Inflict Physical Damage types
    public Item sword;       
    public Item hammer;
    public Item morningstar;
    public Item bow;
    public Item sling;
    public Item chakrams;
    public Item daggers;
    public Item pellets;
    public Item whip;


    //Armor [Slot 1]  Helps protect agains t physical damages
    public Item leather;
    public Item chain;
    public Item plate;

    //Ring [Slot 2]  Boosts the recovery rate of health
    public Item ring;

    //Magic Weapons [Slot 3]  Used to Infict Magical damage types
    public Item staff;
    public Item orb;
    public Item wand;
    public Item darkWand;
    public Item brightOrb;
    public Item arcaneStaff;

    //Focus [Slot 4]  Helps protect against magic damage
    public Item shield;
    public Item ward;
    public Item trinket;

    //Amulet [Slot 5]
    public Item necklace;

    //Missing Item
    public Item empty;


    //1 Name    2 Basic Damage Type     2.1 Damage   3 Damage Reduction    4 Resistance to Wounds   5 Health Gained    6 Energy Gained 
    //7 Price to upgrade   8 Shape of Attack/Upgrade Rate     8.1 Mesh    9 Slot for Item    10 Identifier
    public ItemDatabase()
    {
        sword = setItem("Sword", 3, 7, 0, 0, 0, 0, 10, 3, 0, 0, 1001);
        bow = setItem("Bow", 2, 5, 0, 0, 0, 0, 6, 1, 0, 0, 1002);
        daggers = setItem("Daggers", 2, 6, 0, 0, 0, 0, 8, 2, 0, 0, 1003);
        hammer = setItem("Hammer", 1, 7, 0, 0, 0, 0, 10, 3, 0, 0,1004);
        morningstar = setItem("Morning Star", 2, 7, 0, 0, 0, 0, 9, 3, 0, 0, 1005);
        sling = setItem("Sling", 1, 5, 0, 0, 0, 0, 7, 1, 0, 0, 1006);
        pellets = setItem("Pellets", 1, 6, 0, 0, 0, 0, 8, 2, 0, 0, 1007);
        whip = setItem("Whip", 3, 6, 0, 0, 0, 0, 8, 2, 0, 0, 1008);
        staff = setItem("Staff", 8, 7, 0, 0, 0, 0, 10, 3, 0, 3, 4001);
        orb = setItem("Orb", 9, 6, 0, 0, 0, 0, 8, 2, 0, 3, 4002);
        wand = setItem("Wand", 7, 5, 0, 0, 0, 0, 6, 1, 0, 3, 4003);
        darkWand = setItem("Dead Branch", 9, 5, 0, 0, 0, 0, 7, 1, 0, 3, 4004);
        brightOrb = setItem("Light Orb", 8, 6, 0, 0, 0, 0, 8, 2, 0, 3, 4005);
        arcaneStaff = setItem("Arcane Staff", 7, 7, 0, 0, 0, 0, 10, 3, 0, 3, 4006);
        chain = setItem("Chain Mail", 13, 0, 3, 90, 1, 0, 8, 1, 0, 1, 2001);
        plate = setItem("Plate", 14, 0, 4, 80, 2, 0, 10, 1, 0, 1, 2002);
        leather = setItem("Leather Garbs", 15, 0, 1, 70, 3, 1, 6, 1, 0, 1, 2003);
        ward = setItem("Ward", 9, 0, 1, 0, 2, 1, 6, 1, 0, 4, 5001);
        shield = setItem("Shield", 8, 0, 1, 0, 1, 1, 10, 1, 0, 4, 5002);
        trinket = setItem("Ward", 7, 0, 1, 0, 1, 2, 8, 1, 0, 4, 5003);
        necklace = setItem("Amulet", 16, 0, 0, 0, 0, 1, 12, 1, 0, 5, 6001);
        ring = setItem("Ring", 16, 0, 0, 0, 1, 0, 12, 1, 0, 2, 3001);
        empty = setItem("No Item Equipped", 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 0);


    }

    

    public Item getItem(int ID)
    {
        
       
        switch (ID)
        {
            //////Weapons\\\\\\\\\\\\\
            case 1001:
                return sword;
                
            case 1002:
                return bow;
                
            case 1003:
                return daggers;
                
            case 4001:
                return staff;
                
            case 4002:
                return orb;

            case 4003:
                return wand;

            /////Armor\\\\\\\\\\\\\
            case 2001:
                return chain;

            case 2002:
                return plate;

            case 2003:
                return leather;

            case 5001:
                return ward;

            case 5002:
                return shield;

            case 5003:
                return trinket;

            //////Rings\\\\\\\\\\\\\
            case 3001:
                return ring;

            //////Amulets\\\\\\\\\\\
            case 6001:
                return necklace;


            default:
                return empty;
        }


    }



    private Item setItem(string name,int basicDamage, int damage, int damageReduction, int woundResist, int Regeneration, int recovery, int price, int attackType, int mesh, int slot, int ID)
    {
        Item tempItem = new Item();

        tempItem.itemName = name;
        tempItem.basicDamage = basicDamage;
        tempItem.damage = damage;
        tempItem.reduction = damageReduction;
        tempItem.woundResist = woundResist;
        tempItem.regeneration = Regeneration;
        tempItem.recovery = recovery;
        tempItem.price = price;
        tempItem.attackType = attackType;
        tempItem.mesh = mesh;
        tempItem.slot = slot;
        tempItem.id = ID;

        return tempItem;
    }
}