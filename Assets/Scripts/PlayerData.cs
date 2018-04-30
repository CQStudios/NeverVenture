using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class PlayerData {
    public int brawn, agility, vitality, intelligence, wisdom, willpower;
    public float experience;
    public List<FullItem> Stash = new List<FullItem>();
    public List<FullItem> Equipment = new List<FullItem>();



    public PlayerData(int brawn, int agility, int vitality, int intelligence, int wisdom, int willpower, float experience, List<FullItem> Stash, List<FullItem> Equipment)
    {
        this.brawn = brawn;
        this.agility = agility;
        this.vitality = vitality;
        this.intelligence = intelligence;
        this.wisdom = wisdom;
        this.willpower = willpower;
        this.experience = experience;
        this.Stash = Stash;
        this.Equipment = Equipment;
    }

    public PlayerData(PlayerData data)
    {
        this.brawn = data.brawn;
        this.agility = data.agility;
        this.vitality = data.vitality;
        this.intelligence = data.intelligence;
        this.wisdom = data.wisdom;
        this.willpower = data.willpower;
        this.experience = data.experience;
        this.Stash = data.Stash;
        this.Equipment = data.Equipment;
    }


    //Getters and Setters

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
        intelligence = stat;
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
    void setStash(List<FullItem> Stash)
    {
        this.Stash = Stash;
    }
    void setEquipment(List<FullItem> Equipment)
    {
        this.Equipment = Equipment;
    }

}
