using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnim : MonoBehaviour {

    public float scale = 1;
    public float power = 3;
    public float timeScale = 1;

    private float offsetX;
    private float offsetY;
    private MeshFilter filter;

	// Use this for initialization
	void Start () 
    {
        filter = GetComponent<MeshFilter>();
        AnimWater();
	}
	
	// Update is called once per frame
	void Update ()
    {
        AnimWater();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;
	}

    void AnimWater()
    {
        Vector3[] vertices = filter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z) * power;
        }

        filter.mesh.vertices = vertices;
    }

    float CalculateHeight(float x, float y)
    {
        float xCord = x * scale + offsetX;
        float yCord = y * scale + offsetY;

        return Mathf.PerlinNoise(xCord, yCord);
    }
}
