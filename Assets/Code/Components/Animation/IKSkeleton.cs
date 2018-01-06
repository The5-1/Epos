using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/*
public enum BoneChainType { Angle, Zigzag, Tentacle};
[System.Serializable]
public class IKLimb
{
    string name;
    GameObject root; //parent this attaches to

    uint numBones;

    float reactive; //limb tries to keep ballance (Arms, Legs, Tails)
    float stiffness; //how wobbly or stiff (tentacle vs antenna)
    float randomMotion; //random twitching (tentacles, antenna, ...)

    BoneIKTargetRole role;
    BoneChainType type;

    public IKLimb(string name, GameObject parent, uint numBones, BoneIKTargetRole role = BoneIKTargetRole.Arm, BoneChainType type = BoneChainType.Angle)
    {
        this.name = name;
        this.root = parent;
        this.numBones = numBones;
        bones = new List<IKBone>();
        this.role = role;
        this.type = type;

        makeGameObjects();
    }

    ~IKLimb()
    {

    }

    void makeGameObjects()
    {
        GameObject previous = root;

        for (int i = 0; i < numBones; i++)
        {
            GameObject current = new GameObject(this.name + "_" + i);
            current.transform.parent = previous.transform;
            IKBone newBone = current.AddComponent<IKBone>();
            bones.Add(newBone);
            previous = current;
        }
    }

    void deleteGameObjects()
    {
        foreach (IKBone go in bones)
        {
            IKBone bone = go.GetComponent<IKBone>();
            bone.delete();
        }
    }

}
*/

[System.Serializable]
public class IKStature
{
    public float hipHeight = 1.0f;
}


public class IKSkeleton : MonoBehaviour
{
    public bool DEBUG_trigger = false;

    IKStature stature;

    Collider mainCollider;

    public GameObject rootGO;
    public IKBone root;
    public IKBone newestBone;

    // Use this for initialization
    void Awake()
    {
        stature = new IKStature();
        mainCollider = this.transform.root.GetComponent<Collider>();

        rootGO = new GameObject("Root");
        rootGO.transform.parent = this.gameObject.transform;
        rootGO.transform.localRotation = Quaternion.identity;
        rootGO.transform.localPosition = new Vector3(0.0f, stature.hipHeight, 0.0f);
        root = rootGO.gameObject.AddComponent<IKBone>();
        root.initRoot();
        newestBone = root;
    }

    private void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            newestBone = newestBone.addBone("newBone", 0.4f, 0.1f, mainCollider);
        }
    }


    //Framerate dependent
    void Update()
    {

    }

    //constant time (e.g. Physics)
    void FixedUpdate()
    {
        if(DEBUG_trigger)
        {
            newestBone = newestBone.addBone("newBone",0.4f,0.1f, mainCollider);
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
