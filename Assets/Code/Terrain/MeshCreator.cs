using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MeshCreator {

    public static void Quad(out List<Vector3> vertices, out List<int> indices, float size)
    {
        vertices = new List<Vector3>(4);
        indices = new List<int>(6);

        vertices.Add(new Vector3(0.0f, 0.0f, 0.0f));
        vertices.Add(new Vector3(size, 0.0f, 0.0f));
        vertices.Add(new Vector3(0.0f, 0.0f, size));
        vertices.Add(new Vector3(size, 0.0f, size));

        //indices[0] = 

    }

    public static Mesh Quad(out List<Vector3> vertices, out List<int> indices, Vector3 BL, Vector3 TR)
    {
        vertices = new List<Vector3>(4);
        vertices.Add(new Vector3(BL.x, BL.y, BL.z));
        vertices.Add(new Vector3(BL.x+TR.x, BL.y, BL.z+TR.z));
        vertices.Add(new Vector3(BL.x, BL.y+TR.y, BL.z));
        vertices.Add(new Vector3(BL.x+TR.x, BL.y+TR.y, BL.z+TR.z));

        indices = new List<int>(6) { 0, 1, 2, 2, 3, 1 };

        Mesh m = new Mesh();
        m.SetVertices(vertices);
        m.SetTriangles(indices.ToArray(),0);
        return m;
    }

    public static void Grid(out Vector3[] vertices, out int[] triangles, float tilesize = 1.0f, ushort tilesX = 1, ushort tilesY = 0, bool centered = false)
    {
        if (tilesX < 1) tilesX = 1;
        if (tilesY < 1) tilesY = tilesX;

        float offsetX = 0.0f;
        if (centered) { offsetX = -tilesize / 2.0f; }

        float offsetY = 0.0f;
        if (centered) { offsetY = -tilesize / 2.0f; }

        int numVerts = (tilesX+1) * (tilesY+1);
        int numTris = tilesX * tilesY * 6;
        vertices = new Vector3[numVerts];
        triangles = new int[numTris];

        for (int y = 0; y < tilesY + 1; y++)
        {
            for(int x = 0; x < tilesX + 1; x++)
            {
                vertices[y * (tilesY + 1) + x] = new Vector3(x+ offsetX, 0.0f,y+ offsetY) *tilesize;
            }
        }
        for (int ti = 0, vi = 0, y = 0; y < tilesY; y++, vi++)
        {
            for (int x = 0; x < tilesX; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + tilesX + 1;
                triangles[ti + 5] = vi + tilesX + 2;
            }
        }
    }

    public static Mesh GridMesh(float tilesize = 1.0f, ushort tilesX = 1, ushort tilesY = 0, bool centered = false)
    {
        Vector3[] vertices;
        int[] triangles;
        Mesh mesh = new Mesh();
        Grid(out vertices, out triangles, tilesize, tilesX, tilesY, centered);

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
