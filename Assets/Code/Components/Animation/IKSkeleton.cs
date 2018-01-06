using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoneChainRole { Arm, Leg, Head, Spine, Wing, None};

public enum BoneChainType { Angle, Zigzag, Tentacle};

[System.Serializable]
public class IKLimb
{
    string name;
    GameObject root; //parent this attaches to

    uint numBones;
    List<IKBone> bones;
    Transform target;
    Transform hint;

    float reactive; //limb tries to keep ballance (Arms, Legs, Tails)
    float stiffness; //how wobbly or stiff (tentacle vs antenna)
    float randomMotion; //random twitching (tentacles, antenna, ...)

    BoneChainRole role;
    BoneChainType type;

    public IKLimb(string name, GameObject parentToAttachTo, uint numBones, BoneChainRole role = BoneChainRole.Arm, BoneChainType type = BoneChainType.Angle)
    {
        this.name = name;
        this.root = parentToAttachTo;
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

public class IKSkeleton : MonoBehaviour
{

    public bool DEBUG_trigger = false;

    List<IKLimb> limbs;



    // Use this for initialization
    void Awake()
    {
        limbs = new List<IKLimb>();
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
            limbs.Add(new IKLimb("testLimb", this.gameObject, 10));
            DEBUG_trigger = false;
        }
    }

    //Late update updates after everything else was moved, perfect for cameras
    void LateUpdate()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, 1.0f);
    }
}
