using System.Collections;
using System.Collections.Generic;
using UnityEngine;





//public enum BoneChainType { Angle, Zigzag, Tentacle};

[System.Serializable]
public class IKChain
{
    public string name;
    public IKSkeleton parentSkeleton;
    public IKBone start;
    public IKBone end;
    public GameObject target;

    public IKChain(string name, IKBone start, IKBone end, IKSkeleton parentSkeleton)
    {
        this.name = name;
        this.start = start;
        this.end = end;
        this.parentSkeleton = parentSkeleton;

        target = new GameObject(name + "_target");
        target.transform.parent = this.parentSkeleton.transform;
    }
}

[System.Serializable]
public class IKStature
{
    public float hipHeight = 1.0f;
}


public class IKSkeleton : MonoBehaviour
{
    public bool DEBUG_trigger = false;
    public IKBone DEBUG_newestBone;


    public IKStature stature;
    public Collider mainCollider;
    public GameObject rootGO;
    public IKBone root;

    public List<IKChain> chains;

    public void makeLimb(string name, IKBone start, IKBone end)
    {
        chains.Add(new IKChain(name, start,end,this));
    }

    // Use this for initialization
    void Awake()
    {
        rootGO = new GameObject("Root");
        this.mainCollider = this.gameObject.GetComponent<Collider>();
        rootGO.transform.parent = this.gameObject.transform;
        rootGO.transform.localRotation = Quaternion.identity;
        rootGO.transform.localPosition = new Vector3(0.0f, stature.hipHeight, 0.0f);
        root = rootGO.gameObject.AddComponent<IKBone>();
        root.initRoot(this);
        DEBUG_newestBone = root;

        stature = new IKStature();

        chains = new List<IKChain>();
    }

    private void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            DEBUG_newestBone = DEBUG_newestBone.addBone("newBone", Random.Range(0.2f,0.8f), Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), this);

            //IKBone[] bones = this.gameObject.GetComponentsInChildren<IKBone>();
            //int r = Random.Range(0, bones.Length - 1);
            //newestBone = bones[r];             

        }
    }


    //Framerate dependent
    void Update()
    {
        foreach(IKChain c in chains)
        {

        }
    }

    //constant time (e.g. Physics)
    void FixedUpdate()
    {
        if(DEBUG_trigger)
        {
            DEBUG_newestBone = DEBUG_newestBone.addBone("newBone", Random.Range(0.2f, 0.6f), Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), this);
            DEBUG_trigger = false;
        }
    }

    //Late update updates after everything else was moved, perfect for cameras
    void LateUpdate()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(rootGO != null)
        {
            Gizmos.DrawCube(rootGO.transform.position, Vector3.one *0.1f);
            Gizmos.DrawWireCube(rootGO.transform.position, Vector3.one*(0.1f+float.Epsilon));
        }
        Gizmos.DrawSphere(this.transform.position, 0.05f);
        Gizmos.DrawWireSphere(this.transform.position, 0.05f + float.Epsilon);
    }
}
