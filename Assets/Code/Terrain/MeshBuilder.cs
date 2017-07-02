using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//http://www.gamasutra.com/blogs/JayelindaSuridge/20130903/199457/Modelling_by_numbers_Part_One_A.php
[System.Serializable]
public class MeshBuilder
{
    //Double Buffer VS MarkDynamic:
    //MarkDynamic is new and tries to replace manual duble buffering
    //https://forum.unity3d.com/threads/huge-performance-loss-in-mesh-createvbo-for-dynamic-meshes-ios.118723/#post-1292009
    //https://github.com/MattRix/Futile/issues/185
    //https://docs.unity3d.com/Manual/MobileOptimisation.html

    //avoid mesh.clear()!!!

    public Mesh mesh;
    private bool doublebuffer = true; //deprecated, use marc dynamic
    private bool dynamic = false;

    //https://docs.unity3d.com/ScriptReference/Mesh.html
    public List<Vector3> vertices;
    private bool verticesChanged = false;
    private int verticesCount;
    public List<int> indices;
    private bool indicesChanged = false;
    public List<Vector2> uvs0;
    private bool uv0Changed = false;
    public List<Vector2> uvs1;
    private bool uv1Changed = false;
    public List<Vector2> uvs2;
    private bool uv2Changed = false;
    public List<Vector2> uvs3;
    private bool uv3Changed = false;
    public List<Vector3> normals;
    public List<Vector4> tangents; //w to flip the binormal, unit vector in horizontal (U) texture direction
    private bool normalsChanged = false;
    public List<Color32> colors; //mesh.color32 = RGBA 4bytes
    private bool colorsChanged = false;

    public MeshBuilder(bool dyn = false, bool buffer = false)
    {
        mesh = new Mesh();
        vertices = new List<Vector3>();
        verticesCount = 0;
        indices = new List<int>();
        normals = new List<Vector3>();
        uvs0 = new List<Vector2>();
        colors = new List<Color32>();
        setDynamic(dyn);
    }

    public void setDynamic(bool dyn)
    {
        dynamic = dyn;
        checDynamic();
    }

    private void checDynamic()
    {
        if (dynamic)
        {
            //https://docs.unity3d.com/ScriptReference/Mesh.MarkDynamic.html
            mesh.MarkDynamic();
        }
    }

    public void setBuffered(bool buffer)
    {
        doublebuffer = buffer;
    }

    public void Clear()
    {
        //avoid mesh.clear()!!!
        vertices.Clear();
        verticesCount = 0;
        indices.Clear();
        normals.Clear();
        uvs0.Clear();
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
        //Debug.Log(string.Format("verts: {0}, indices: {1} ", vertices.Count, indices.Count));
        //int vertexIndex = vertices.Count; //already +1!!!
        int vertexIndex = verticesCount;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        verticesCount += 3;
        indices.Add(vertexIndex);
        indices.Add(vertexIndex + 1);
        indices.Add(vertexIndex + 2);
        verticesChanged = true;
        indicesChanged = true;
        return vertexIndex;
    }

    public void ColorTrinagles(Color32 c1, int numTris = 1)
    {
        for(int i = 0; i < numTris; i++)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c1);
        }
        colorsChanged = true;
    }

    public void AddTriangle(int t1, int t2, int t3)
    {
        indices.Add(t1);
        indices.Add(t2 + 1);
        indices.Add(t3 + 2);
        verticesChanged = true;
        indicesChanged = true;
    }

    public Mesh UpdateMesh()
    {
        if (vertices.Count > 1 && indices.Count > 1)
        {
            //http://answers.unity3d.com/questions/1215847/mesh-setvertices-vs-vertices.html
            //mesh.vertices= vertices.ToArray(); //dprecated and produces garbage list, see above!

            if (verticesChanged)
            {
                mesh.SetVertices(vertices);
                verticesChanged = false;
            }

            if (indicesChanged)
            {
                mesh.SetTriangles(indices, 0, false); //false: no bounds recalculation
                indicesChanged = false;
            }

            if (uv0Changed && uvs0 != null && uvs0.Count == vertices.Count)
            {
                mesh.SetUVs(0, uvs0);
                uv0Changed = false;
            }

            if (uv1Changed && uvs1 != null && uvs1.Count == vertices.Count)
            {
                mesh.SetUVs(1, uvs0);
                uv1Changed = false;
            }
            if (uv2Changed && uvs2 != null && uvs1.Count == vertices.Count)
            {
                mesh.SetUVs(2, uvs0);
                uv0Changed = false;
            }
            if (uv3Changed && uvs3 != null && uvs3.Count == vertices.Count)
            {
                mesh.SetUVs(3, uvs0);
                uv3Changed = false;
            }

            if (normalsChanged &&  normals != null && normals.Count == vertices.Count)
            {
                mesh.SetNormals(normals);
                if (tangents != null && tangents.Count == vertices.Count)
                {
                    mesh.SetTangents(tangents);
                }
                uv3Changed = false;
            }
            else mesh.RecalculateNormals();

            if (colorsChanged && colors != null && colors.Count == vertices.Count)
            {
                mesh.SetColors(colors);
                colorsChanged = false;
            }

            mesh.RecalculateBounds();
            //mesh.UploadMeshData(true); //markNoLogerReadable makes Mesh data not be acessible from the script anymore!!

            return mesh;
        }
        else
        {
            return null;
        }
    }

    public void UpdateVertices()
    {

    }

}