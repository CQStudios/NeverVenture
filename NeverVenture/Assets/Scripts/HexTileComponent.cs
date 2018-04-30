using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTileComponent : MonoBehaviour {

    //When the mouse hovers over the GameObject, it turns to this color (red)
    Color m_MouseOverColor = Color.red;
    //This stores the GameObject’s original color
    Color m_OriginalColor;
    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    MeshRenderer m_Renderer;

    private bool mouseOver = false;
    public bool MouseOver
    {
        get { return mouseOver; }
        set { mouseOver = value; }
    }

    private bool mouseclicked = false;
    public bool MouseClicked
    {
        get { return mouseclicked; }
        set { mouseclicked = value; }
    }





    // Use this for initialization
    void Start () {
        //Fetch the mesh renderer component from the GameObject
        m_Renderer = GetComponent<MeshRenderer>();
        //Fetch the original color of the GameObject
        m_OriginalColor = m_Renderer.material.color;
    }

    void OnMouseOver()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }

    private void OnMouseUpAsButton()
    {
        mouseclicked = true;
    }


    public void Highlight(bool on)
    {
        if (on)
        {
            //Change the color of the GameObject to red when the mouse is over GameObject
            m_Renderer.material.color = m_MouseOverColor;
        }
        else
        {
            //Reset the color of the GameObject back to normal
            m_Renderer.material.color = m_OriginalColor;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}


}
