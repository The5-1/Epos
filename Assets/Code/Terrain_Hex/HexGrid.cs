using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour{

    private HexGridData hexGridData;
    private HexMesh hexMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private bool needsUpdate = true; //FIXME: not like this

    void Awake()
    {
        init();
    }

    private void init()
    {
        hexGridData = new HexGridData(10,10);
        hexMesh = new HexMesh(hexGridData);
        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
    }

    private void Start()
    {
        //resource dependent stuff should only start in Start. During Awake, resources are still being loaded in the other classes
        if (meshRenderer.material != Material_Manager.singleton.materials[0].material) meshRenderer.material = Material_Manager.singleton.materials[0].material;
    }

    public void updateMesh()
    {
        meshFilter.mesh =  hexMesh.recalculateMesh();
    }


    private void FixedUpdate()
    {
        if(needsUpdate)
        {
            updateMesh();
        }
    }

}
