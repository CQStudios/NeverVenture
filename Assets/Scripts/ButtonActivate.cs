using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;





public class ButtonActivate : MonoBehaviour {


    public string Name;
    public Text[] Turns;
    public RawImage[] TurnImages;
    public GameObject TurnObjects;
    public Texture Picture;
    // Use this for initialization
    void Start () {
        GameObject TurnObjects = GameObject.FindGameObjectWithTag("TurnObjects");
        Turns = TurnObjects.GetComponentsInChildren<Text>();
        TurnImages = TurnObjects.GetComponentsInChildren<RawImage>();
        Name = this.gameObject.name;
        Picture = Resources.Load<Texture>("Images/" + Name);
        print(name);
    }

// Update is called once per frame
void Update () {

    }
    void MeClicked()
    {
        if (Turns[0].text == "Turn 1")
        {
            Turns[0].text = Name.ToString();
            TurnImages[0].texture = Picture;
        }
        else if (Turns[1].text == "Turn 2")
        {
            Turns[1].text = Name.ToString();
            TurnImages[1].texture = Picture;
        }
        else if (Turns[2].text == "Turn 3")
        {
            Turns[2].text = Name.ToString();
            TurnImages[2].texture = Picture;
        }
        else if (Turns[3].text == "Turn 4")
        {
            Turns[3].text = Name.ToString();
            TurnImages[3].texture = Picture;
        }
        else if (Turns[4].text == "Turn 5")
        {
            Turns[4].text = Name.ToString();
            TurnImages[4].texture = Picture;
        }
        else if (Turns[5].text == "Turn 6")
        {
            Turns[5].text = Name.ToString();
            TurnImages[5].texture = Picture;
        }

    }
}
