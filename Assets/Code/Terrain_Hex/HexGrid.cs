using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour{

    private HexGridData hexGridData;
    private HexMesh hexMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public float zeroHeight;
    public Plane zeroPlane;

    private bool needsUpdate = true; //FIXME: not like this



    void Awake()
    {
        init();
        updateMesh();
    }

    private void init()
    {

        hexGridData = new HexGridData(40,40);
        hexMesh = new HexMesh(hexGridData);
        meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshCollider = this.gameObject.AddComponent<MeshCollider>();
        zeroPlane = new Plane();

        updateMesh();
    }

    private void Start()
    {
        //FIXME : resource dependent so it is in Start and not Awake right now.
        if (meshRenderer.material != Material_Manager.singleton.materials[0].material) meshRenderer.material = Material_Manager.singleton.materials[0].material;
    }

    public void updateMesh()
    {
        meshFilter.mesh =  hexMesh.recalculateMesh();
        meshCollider.sharedMesh = meshFilter.mesh;
        zeroPlane.SetNormalAndPosition(this.gameObject.transform.up, Vector3.zero + this.gameObject.transform.up * zeroHeight);
    }

    public void CheckUpdate() //this should be triggered externally and not checked every frame
    {
        if (needsUpdate)
        {
            updateMesh();
            needsUpdate = false;
        }
    }

    private void FixedUpdate()
    {
        CheckUpdate();
    }

    //=============DEBUG interaction methods================

    void Update()
    {
        if (Input.GetKey("mouse 2"))
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        Ray inputRay = Camera_Manager.singleton.activeCamera.ScreenPointToRay(Input.mousePosition); //TODO pass main camera from G
        RaycastHit hit;
        float dist = 0.0f;

        if (meshCollider.Raycast(inputRay, out hit, 10000.0f))
        {
            TouchCell(hit.point);
        }
        else if(zeroPlane.Raycast(inputRay, out dist))
        {
            TouchCell(inputRay.GetPoint(dist), true); //get point along ray at hit dist
        }
    }

    void TouchCell(Vector3 position, bool outside = false)
    {
        position = transform.InverseTransformPoint(position);
        if (!outside)
        {
            int idx = hexGridData.PositionToCellIndex(position);
            //Debug.Log(string.Format("rayhit HexGrid at xyz:{0} / HEX:{1} index:{2}", position, hexGridData.PositionToHex(position), idx));
            hexGridData.cells[idx].height = Random.Range(-1.0f, 1.0f);
            updateMesh();
        }
        else
        {
            //Debug.Log(string.Format("rayhit outside xyz:{0} / HEX:{1} fake-index:{2}", position, hexGridData.PositionToHex(position), hexGridData.PositionToCellIndex(position)));
        }
    }

}
