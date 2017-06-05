using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//http://www.gamasutra.com/blogs/JayelindaSuridge/20130903/199457/Modelling_by_numbers_Part_One_A.php
public class MeshBuilder
{
    //https://docs.unity3d.com/ScriptReference/Mesh.html
    public List<Vector3> vertices;
    public List<int> indices;
    public List<Vector2> uvs;
    public List<Vector2> uvs2;
    public List<Vector2> uvs3;
    public List<Vector2> uvs4;
    public List<Vector3> normals;
    public List<Vector4> tangents; //w to flip the binormal, unit vector in horizontal (U) texture direction
    public List<Color32> colors; //mesh.color32 = RGBA 4bytes

    public MeshBuilder()
    {
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        indices = new List<int>();
    }

    public void Clear()
    {
        vertices.Clear();
        normals.Clear();
        uvs.Clear();
        indices.Clear();
    }

    /// <summary>
    /// returns the index of the first vertex created
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    /// <returns></returns>
    public int AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count; //already +1!!!
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        indices.Add(vertexIndex);
        indices.Add(vertexIndex + 1);
        indices.Add(vertexIndex + 2);
        return vertexIndex;
    }

    public void AddTriangle(int t1, int t2, int t3)
    {
        indices.Add(t1);
        indices.Add(t2 + 1);
        indices.Add(t3 + 2);
    }

    public Mesh CreateMesh()
    {
        if (vertices.Count > 1 && indices.Count > 1)
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices.ToArray();

            if (uvs != null && uvs.Count == vertices.Count) mesh.uv = uvs.ToArray();
            if (uvs2 != null && uvs2.Count == vertices.Count) mesh.uv2 = uvs2.ToArray();
            if (uvs3 != null && uvs2.Count == vertices.Count) mesh.uv3 = uvs3.ToArray();
            if (uvs4 != null && uvs4.Count == vertices.Count) mesh.uv4 = uvs4.ToArray();

            if (normals != null && normals.Count == vertices.Count)
            {
                mesh.normals = normals.ToArray();
                if (tangents != null && tangents.Count == vertices.Count)
                {
                    mesh.tangents = tangents.ToArray();
                }
            }
            else mesh.RecalculateNormals();

            if (colors != null && colors.Count == vertices.Count)
            {
                mesh.colors32 = colors.ToArray();
            }

            mesh.RecalculateBounds();

            return mesh;
        }
        else
        {
            return null;
        }
    }
}