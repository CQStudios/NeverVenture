using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour {
    private List<HexTileComponent> components;

    private int x = 0;
    public int X
    {
        get { return x; }
        set { x = value; }
    }
    private int y = 0;
    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    //reference to the unit script for the object that exists on this tile.
    private List<UnitBehavior> objects;
    public List<UnitBehavior> GetOccupied()
    {
        return objects;
    }
    public void AddOccupied( UnitBehavior o)
    {
        if (!objects.Contains(o))
            objects.Add(o);
    }
    public void RemoveOccupied(UnitBehavior o)
    {
        if (objects.Contains(o))
            objects.Remove(o);
    }

    // Use this for initialization
    void Start () {
        components = new List<HexTileComponent>();
		foreach( Transform component in transform)
        {
            components.Add(component.GetComponent<HexTileComponent>());
        }
        objects = new List<UnitBehavior>();
	}
	

	// Update is called once per frame
	void Update () {

        Highlight(Hovered());

        if (WasClicked())
        {
            //Debug.Log("Clicked element at: " + X + ", " + Y);
            foreach (UnitBehavior o in objects)
            {
                Debug.Log(o.name);
            }
        }
	}

    bool WasClicked()
    {
        bool result = false;
        foreach (HexTileComponent T in components)
        {
            result = (result | T.MouseClicked);
            T.MouseClicked = false;
        }
        return result;
    }

    bool Hovered()
    {
        foreach (HexTileComponent T in components)
        {
            if (T.MouseOver)
            {
                return true;
            }
        }
        return false;
    }

    void Highlight(bool on)
    {
        foreach (HexTileComponent T in components)
        {
            T.Highlight(on);
        }
    }



}
