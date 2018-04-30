using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour {

    public const string path = "items";

	// Use this for initialization
	void Start () {
        ItemContainer ic = ItemContainer.Load(path);


        //print item list to Console
        foreach (Item item in ic.items)
        {
            print(item.itemName);
        }
	}
	
	
}
