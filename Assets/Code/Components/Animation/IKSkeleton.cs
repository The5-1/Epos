using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public enum BoneChainType { Angle, Zigzag, Tentacle};


[System.Serializable]
public class IKChain
{
    public string name;
    public IKSkeleton parentSkeleton;
    public IKBone fork; //the Bone this chain forks from
    public IKTarget target;

    public IKChain(string name, IKBone fork, IKSkeleton parentSkeleton)
    {
        this.name = name;
        this.fork = fork;
        this.parentSkeleton = parentSkeleton;

        GameObject go = new GameObject(name + "_target");
        target = go.AddComponent<IKTarget>();
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


    /*
    public void addChain(string name, IKBone fork)
    {
        chains.Add(new IKChain(name, fork,this));
        Transform parent = fork.gameObject.transform.parent;
        while(parent != null)
        {
            parent.
        }

    }
    */

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
    }

    private void Start()
    {
        for(int i = 0; i < 7; i++)
        {
            DEBUG_newestBone = DEBUG_newestBone.addBone("bone_" + i , Random.Range(0.2f,0.8f), Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), this);

            //IKBone[] bones = this.gameObject.GetComponentsInChildren<IKBone>();
            //int r = Random.Range(0, bones.Length - 1);
            //newestBone = bones[r];             
        }

        DEBUG_newestBone.addTarget();
    }

    public void FABRIK_forwardStep()
    {
        foreach(IKTarget target in this.root.targets)
        {
            IKBone targetBone = target.bone;

            if((this.root.transform.position-target.transform.position).magnitude > targetBone.distanceToRoot )
            {
                //Do stuff if the bone can't possibly reach the target

            }

            FABRIK_forwardsStep_Bone(targetBone);
        }
    }

    public void FABRIK_forwardsStep_Bone(IKBone bone)
    {
        Vector3 bone_start_world = bone.transform.position;
        Vector3 bone_end_world = bone.getEndPointWorld();
        Debug.DrawLine(bone_start_world, bone_end_world, Color.yellow);
        Vector3 bone_target_world = bone.getTargetCenterWorld();

        Vector3 vector_start_to_target = bone_target_world - bone_start_world;
        Debug.DrawLine(bone_start_world, bone_target_world, Color.yellow);
        Vector3 bone_new_start_world = bone_target_world - vector_start_to_target.normalized * bone.length;
        Debug.DrawLine(bone_new_start_world, bone_target_world, Color.red);

        bone.gameObject.transform.SetPositionAndRotation(bone_new_start_world, Quaternion.LookRotation(bone_target_world, new Vector3(0.0f,0.0f,1.0f)));
    }

    //Framerate dependent
    void Update()
    {
        FABRIK_forwardStep();
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
