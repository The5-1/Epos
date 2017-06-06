using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour{

    private HexGridData hexGridData;
    private HexGridMesh hexGridMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public float zeroHeight;
    public Plane zeroPlane;

    [SerializeField]
    private bool needsUpdate = true; //FIXME: not like this


    private int lastIndex = -1;

    void Awake()
    {
        init();
        updateMesh();
    }

    private void init()
    {

        hexGridData = new HexGridData(40,40,2.0f);
        hexGridMesh = new HexGridMesh(hexGridData);
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
        updateCollision();
    }

    public void flagNeedsUpdate()
    {
        needsUpdate = true;
    }

    public void updateMeshAndColision()
    {
        updateMesh();
        updateCollision();
    }

    public void updateMesh()
    {
        meshFilter.mesh =  hexGridMesh.recalculateMesh();
    }

    /*
    private void updateCollisionVertices()
    {
        Does not seem to work
        meshCollider.sharedMesh.SetVertices(hexGridMesh.meshBuilder.vertices);
    }
    */

    private void updateCollision()
    {
        meshCollider.sharedMesh = meshFilter.mesh; //FIXME: maintain a more simple mesh that only updates vertex positions
        zeroPlane.SetNormalAndPosition(this.gameObject.transform.up, Vector3.zero + this.gameObject.transform.up * zeroHeight);
    }

    public void CheckUpdate() //this should be triggered externally and not checked every frame
    {
        if (needsUpdate)
        {
            updateMeshAndColision();
            needsUpdate = false;
        }
    }

    private void FixedUpdate()
    {
        CheckUpdate(); //this is better since we wont call update more than once per physics frame
    }

    //=============DEBUG interaction methods================

    void Update()
    {
        if (Input.GetKey("mouse 0") || Input.GetKey("mouse 2"))
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
            if(idx != lastIndex)
            { 
                hexGridData.cells[idx].height += Random.Range(-0.5f, 0.5f);
                flagNeedsUpdate();
                lastIndex = idx;
            }
        }
        else
        {
            //Debug.Log(string.Format("rayhit outside xyz:{0} / HEX:{1} fake-index:{2}", position, hexGridData.PositionToHex(position), hexGridData.PositionToCellIndex(position)));
        }
    }

}
