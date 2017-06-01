using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGrid
{

    public static class Tile_Metrics
    {
        public const float tilesize = 0.5f;
        public const byte resolution = 4;

        public static Mesh makeTileMesh()
        {
            ushort numverts = (ushort)(resolution * resolution);
            Mesh mesh = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            int[] indices = new int[numverts];
            for(ushort i = 0; i < numverts; i++)
            { 
                
            }
            mesh.SetVertices(verts);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            return mesh;
        }
    }

    public class TerrainTile : MonoBehaviour
    {
        public float height;


    }

    public class TerrainGrid : MonoBehaviour
    {
        public TerrainTile[][] grid;


    }


    public class TerrainMesh : MonoBehaviour {

        [SerializeField] private List<Mesh> terrainMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        void Awake() {
            init();
        }


        private void init()
        {
            terrainMesh = new List<Mesh>();
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();

            terrainMesh.Add(MeshCreator.GridMesh(1.0f,10));

            meshFilter.mesh = terrainMesh[0];
        }

        private void Start()
        {
            //resource dependent stuff should only start in Start. During Awake, resources are still being loaded in the other classes
            meshRenderer.material = Material_Manager.singleton.materials[0].material;
        }


        void FixedUpdate()
        {

        }
    }

}