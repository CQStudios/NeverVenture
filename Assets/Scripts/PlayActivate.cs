using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayActivate : MonoBehaviour {

    public string Name;
    public Text[] Turns;
    public string[] MoveList;
    public GameObject TurnObjects;
    // Use this for initialization
    void Start()
    {
        GameObject TurnObjects = GameObject.FindGameObjectWithTag("TurnObjects");
        Turns = TurnObjects.GetComponentsInChildren<Text>();
        Name = this.gameObject.name;
        print(name);
    }

    // Update is called once per frame
    void Update () {
		
	}
    void MePlay()
    {
        print("REEEEEEEEEEEEEEEEEE");
        if (Turns[0].text != "Turn 1")
        {
            MoveList[0] = Turns[0].text;
            Turns[0].text = "Turn 1".ToString();
            if (Turns[1].text != "Turn 1")
            {
                MoveList[1] = Turns[1].text;
                Turns[1].text = "Turn 2".ToString();
                if (Turns[2].text != "Turn 1")
                {
                    MoveList[2] = Turns[2].text;
                    Turns[2].text = "Turn 3".ToString();
                    if (Turns[3].text != "Turn 1")
                    {
                        MoveList[3] = Turns[3].text;
                        Turns[3].text = "Turn 4".ToString();
                        if (Turns[4].text != "Turn 1")
                        {
                            MoveList[4] = Turns[4].text;
                            Turns[4].text = "Turn 5".ToString();
                            if (Turns[5].text != "Turn 1")
                            {
                                MoveList[5] = Turns[5].text;
                                Turns[5].text = "Turn 6".ToString();
                            }
                        }
                    }
                }

            }

        }
        

    }
}
