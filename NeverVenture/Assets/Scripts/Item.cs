using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Item {

    /* Attribute Explaniations
     * 
     * Name:          Name of the item in the inventory
     * Value:         Damage or armor value of an item
     * Price:         Base value of item to sell at a shop
     * Rating:        Level of the item
     * Speed:         Time in seconds to complete an attack animation or for armor additional time to recover from effect i.e. tripped
     * Damage Type:   The damage inflicted by weapons or damage resisted by armor
     * Damage Amount: Amount of damage inflicted of the special type or damage resisted by armor
     * Slot:          Number to determine which slot the item is equipped on the character
     * 
    */


    //1
    [XmlAttribute("Name")] //Prompt for Xml
    public string itemName; //Created variable for the item

    [XmlAttribute("BasicDamage")]//The unenchanted damage type the weapon will always use based on tier
    public int basicDamage;
    //2
    [XmlAttribute("Damage")] //Raw Damage Amount
    public float damage;
    //3
    [XmlAttribute("DamageReduction")]//Damage Reduction for Armor
    public int reduction;
    //4
    [XmlAttribute("WoundResist")]//Amount of wounds as a percent resisted for Armor
    public float woundResist;
    //5
    [XmlAttribute("Regeneration")]//Amount of health gained per play for Rings and Armor
    public int regeneration;
    //6
    [XmlAttribute("Recovery")]//Amount of energy gained per play for Amulets and Trinkets
    public int recovery;
    //7
    [XmlAttribute("Price")]// Value the item needs for upgrade in copper
    public float price;
    //8
    [XmlAttribute("AttackType")]//Shape of Attack (Area 3, Cone 2, Line 1) Also the rate for upgrading base damage
    public int attackType;
    //9
    [XmlAttribute("Mesh")]//CHANGE LATER variable for pulling the proper mesh
    public int mesh;

    //10
    [XmlAttribute("Slot")]//Slot the Item is used in the Equipment
    public int slot;
    //11
    [XmlAttribute("ID")]//Unique Identifier for item
    public int id;

}

////         none         //0
//public int bludgeoning; //1
//public int piercing;    //2
//public int slashing;    //3
//public int sonic;       //4
//public int acid;        //5
//public int narcotic;    //6

////   Magical
//public int arcane;      //7
//public int radiant;     //8
//public int necrotic;    //9
//public int fire;        //10
//public int clod;        //11
//public int shock;       //12


////Special Damage Resistance
//public int mundane;     //13
//public int alchemical;  //14
//public int mystical;    //15
//public int elemental;   //16