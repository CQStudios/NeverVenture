using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


//Watches a floor's map (list of WRooms) and generates a texture2d based on the size of the floor.
public class MiniMap {
    private WFloor myfloor;
    private Image TexMap;
    private int width, height;
    private Vector2 rawFloorSize , topleft;
    private float scalex, scaley, truscale;

    public Color Unrevealed, Revealed, Visited, NotCleared;

    public MiniMap(WFloor floor)
    {
        myfloor = floor;
        rawFloorSize = Vector2.zero;    //a new minimap does not create the minimap at creation.
        topleft = rawFloorSize;

    }

    public void SetTextureSize(int w, int h)
    {
        width = w;
        height = h;
    }

    //update the texture (must be called remotely, preferably RIGHT before putting it on something)
    public void Update(Image tex)
    {
        Debug.Log("MiniMap_Update. setting texmap:" + tex.preferredWidth + "," + tex.preferredHeight);
        TexMap = tex;
        Debug.Log("MiniMap_Update. setting rawfloorsize");
        rawFloorSize = FindRoomDimensions();    //find the integer size of the floor in terms of rooms. 1 unit per roomwidth.
        if (rawFloorSize == Vector2.zero || rawFloorSize.x == 0 || rawFloorSize.y == 0)
        {
            Debug.Log("MiniMap_Update. setting rawfloorsize failed. it is still zero.");
            return;
        }
        Debug.Log("MiniMap_Update. setting map size");
        SetMapSize();
        Debug.Log("MiniMap_Update. drawing texture.");
        DrawMap();
    }

    //set constraints on size such that the map we draw will always fit within the given size
    //since rooms are square, we maintain the smaller scale
    private void SetMapSize()
    {
        //does nothing if the floor has nothing/inacceptible bounds.
        if (rawFloorSize == Vector2.zero || rawFloorSize.x == 0 || rawFloorSize.y == 0)
        {
            Debug.Log("SetMapSize failed because rawfloorsize is still zero.");
            return;
        }

        scalex = width / rawFloorSize.x; //the scale required such that the rawfloorsize.x could hit every column of pixels (width)
        scaley = height / rawFloorSize.y; //the scale required such that the rawfloorsize.x could hit every column of pixels (height)
        if (scalex > scaley)
            truscale = scaley;
        else
            truscale = scalex;
        Debug.Log("MiniMap_SetMapSize. scalex:" + scalex + " scaley:"+ scaley +" truscale:" + truscale);
        //TexMap.
        //TexMap.Resize(width, height);
    }

    public Image GetMap()
    {
        return TexMap;
    }
	


    //internally called, returns the maximum dimensions of the room such that each traversal in a direction is 1 unit. 
    //eg the start room going to a hallway, and then to an end room would look like [][][], and thus the dimensions would be 3x1.
    //We return that vector2 when called, and usually store it in our rawFloorSize value
    private Vector2 FindRoomDimensions()
    {
        //declare floats establishing bounds of the map.
        float Xmax = -Mathf.Infinity, Xmin = Mathf.Infinity, Ymax = -Mathf.Infinity, Ymin = Mathf.Infinity;

        //room locations are multiples of the floor's roomsize value
        foreach (Vector3 o in myfloor.roomLocations)
        {
            if (o.x > Xmax)
                Xmax = o.x;
            if (o.x < Xmin)
                Xmin = o.x;
            if (o.z > Ymax)
                Ymax = o.z;
            if (o.z < Ymin)
                Ymin = o.z;
        }
        Xmax /= myfloor.roomWidth;
        Xmin /= myfloor.roomWidth;
        Ymax /= myfloor.roomWidth;
        Ymin /= myfloor.roomWidth;
        topleft.Set(Xmin-0.5f, Ymax+0.5f);                      //where (0,0) should be. half-point offsets allow for drawing the room in the corner.
        return new Vector2(Xmax - Xmin + 1, Ymax - Ymin + 1);   //add 1 because a single room map would result in <0,0> otherwise.
    }

    //draws the map to the texture we hold using truscale
    //assumes correct truscale, and also that x = 0 = location - xmin.
    private void DrawMap()
    {
        //Debug.Log("MiniMap_DrawMap. Texture size is width:" + TexMap.width + " height:" + TexMap.height + "   truscale is: "+ truscale);
        //for (int x = 0; x < TexMap.width; x++)
        //{
            //for (int y = 0; y < TexMap.height; y++)
            //{
                //fucking fill it with goddamn background color. shit laggy af.
                //don't call this that often dude.
                //TexMap.SetPixel(x, y, Unrevealed);
            //}
        //}
        Color current = Unrevealed;
        Vector2 center = new Vector2();
        foreach (WRoom o in myfloor.map)
        {
            //if we've been revealed, we have special drawing to do.
            if (o.hasBeenRevealed)
                current = Revealed;
            else{ current = Unrevealed; } //continue; }
            if (o.hasBeenVisited)
                current = Visited;
            if (!o.hasBeenCleared)
                current = new Color((current.r + NotCleared.r)/2, (current.g + NotCleared.g) / 2, (current.b + NotCleared.b) / 2);
            if (o.hasObjective)
                current = NotCleared;
            //get the center of the block in integral terms
            center = new Vector2(o.Location.x / myfloor.roomWidth, o.Location.z / myfloor.roomWidth);
            //get it in terms of pixel locations now (relative to te topleft position of the topleft-most room)
            center = ( center + topleft )* truscale ;

            //we have our color for our block, now draw the block
            for (int x = (int)(center.x - truscale); x <= (int)(center.x + truscale); x++)
            {
                for (int y = (int)(center.y - truscale); y <= (int)(center.y + truscale); y++) { 
                    Debug.Log("MiniMap_DrawMap. " + x + ", " + y);
                    //TexMap.SetPixel(x, y, current); //x,y is a pixel location
                }
            }
            }
        //TexMap.Apply();
    }

	
}
