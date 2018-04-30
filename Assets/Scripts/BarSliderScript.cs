using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BarSliderScript : MonoBehaviour {

    public Vector3 HP;
    public UnitBehavior playerObj;
    public Image Wound, Health;
	// Use this for initialization
	void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        HP = playerObj.GetHealth();
        updateHealth();
        updateWounds();
	} 


    void updateHealth()
    {
        Health.fillAmount = (HP[0] - HP[1] - HP[2]) / HP[0];   
    }

    
    void updateWounds()
    {
        Wound.fillAmount = (HP[0] - HP[1]) / HP[0];
    }

    
}
