using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteWaterPlane : MonoBehaviour {

    public float size = 1;
    public int planeSize = 16;

    private MeshFilter filter;

	// Use this for initialization
	void Start () 
    {
        filter = GetComponent<MeshFilter>();
        filter.mesh = GenerateMesh();
        //generate obstace collision
        GetComponent<MeshCollider>();
        //AddComponent&lt;MeshCollider&gt();
	}

    private Mesh GenerateMesh()
    {
        Mesh m = new Mesh();

        var vertices = new List<Vector3>(); //Contains x,y,z vertices
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>(); //Only contains x,z vertices

        for(int x = 0; x < planeSize + 1; x++)
        {
            for(int y = 0; y < planeSize + 1; y++)
            {
                vertices.Add(new Vector3(-size * 0.5f + size * (x / ((float)planeSize)), 0, -size * (y / ((float)planeSize))));
                normals.Add(Vector3.up);
                uvs.Add(new Vector2(x / (float)planeSize, y / (float)planeSize));
            }
        }

        var triangles = new List<int>();
        var vertCount = planeSize + 1;
        for(int i = 0; i < vertCount * vertCount - vertCount; i++)
        {
            if((i + 1) % vertCount == 0)
            {
                continue;
            }
            triangles.AddRange(new List<int>()
            {
                i + 1 + vertCount, i + vertCount, i,
                i, i + 1, i + vertCount + 1
            });
        }

        m.SetVertices(vertices);
        m.SetNormals(normals);
        m.SetUVs(0, uvs);
        m.SetTriangles(triangles, 0);

        return m;

    }

    

}
