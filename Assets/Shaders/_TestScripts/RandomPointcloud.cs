using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RandomPointcloud : MonoBehaviour {

    [Range(0.0f, 100.0f)]
    public float size = 10.0f;
    private float prev_size;

    [Range(0, 65536-1)] //a mesh in unity can only have 65536 vertices
    public int numPoints = 60000;
    private int prev_numPoints;

    public enum cloudType {cube, sphere};
    public cloudType type = cloudType.sphere;
    private cloudType prev_type;

    private Mesh mesh;

    private void Awake()
    {
        this.gameObject.AddComponent<MeshFilter>();
        this.gameObject.AddComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;
        CreateMesh();
        prev_numPoints = numPoints;
        prev_type = type;
    }

    private void FixedUpdate()
    {
        if(prev_numPoints != numPoints || prev_type != type || prev_size != size)
        {
            prev_size = size;
            prev_numPoints = numPoints;
            prev_type = type;
            updatePoints();
        }
    }


    private void updatePoints()
    {
        mesh.Clear();
        CreateMesh();
    }

    private Vector3 PointInSphere(float radius)
    {
        float distribution = Random.Range(0.0f, 1.0f);
        distribution =1.0f-distribution;
        distribution *= distribution;
        distribution *= distribution;
        distribution = 1.0f - distribution;
        return PointInCube(1).normalized * distribution * radius; 
    }

    private Vector3 PointInCube(float edgelenght)
    {
        return new Vector3(Random.Range(-edgelenght*0.5f, edgelenght * 0.5f), Random.Range(-edgelenght * 0.5f, edgelenght * 0.5f), Random.Range(-edgelenght * 0.5f, edgelenght * 0.5f));
    }

    private void CreateMesh()
    {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
        Color[] colors = new Color[numPoints];

        for (int i = 0; i < points.Length; ++i)
        {
            switch(type)
            {
                case cloudType.cube:
                    points[i] = PointInCube(size);
                    break;
                case cloudType.sphere:
                    points[i] = PointInSphere(size);
                    break;
                default:
                    return;
            }

            indecies[i] = i;
            colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        }

        mesh.vertices = points;
        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }

}
