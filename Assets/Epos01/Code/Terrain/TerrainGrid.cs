using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGrid
{
    public class TerrainGrid : MonoBehaviour
    {
        [SerializeField] private TerrainGrid_Data terrainData;
        [SerializeField] private List<Mesh> terrainMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        void Awake()
        {
            init();
        }

        private void init()
        {
            terrainMesh = new List<Mesh>();
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            //Debug.Log(meshRenderer.material);

            terrainMesh.Add(MeshCreator.GridMesh(1.0f, 10));

            meshFilter.mesh = terrainMesh[0];
        }

        private void Start()
        {
            //resource dependent stuff should only start in Start. During Awake, resources are still being loaded in the other classes
            if (meshRenderer.material != Material_Manager.singleton.materials[0].material) meshRenderer.material = Material_Manager.singleton.materials[0].material;
        }


        void FixedUpdate()
        {

        }
    }
}
