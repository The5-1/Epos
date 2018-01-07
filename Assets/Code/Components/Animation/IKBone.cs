using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental; //transform matrix




[System.Serializable]
public class IKConstraint
{


}

[System.Serializable]
public class IKHingeConstraint : IKConstraint
{

}

public enum BoneIKTargetType { None = 0, Arm, Leg, Head, Spine, Wing };


public class IKTarget : MonoBehaviour
{

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, Vector3.one * (0.1f + float.Epsilon));
    }
}



public enum BoneIKRole { None = 0, Arm, Leg, Head, Spine, Wing};
public enum BoneIKSide { Left, Right, Center};

[System.Serializable]
public class BodyPartStats
{
    float DamageResistance = 0.0f; //keep it simple
}

[System.Serializable]
public class IKBone : MonoBehaviour {

    public bool DEBUG_trigger = false;

    public float length = 0.5f;
    public float distanceToRoot;
    public float thickness = 0.1f;
    public float width = 0.1f;

    protected IKSkeleton parentSkeleton; //the skeleton/Root these bones belong to
    protected IKBone parentFork; //The fork from which this chain branched
    protected IKBone parentBone; //The bone one before this
    protected List<IKBone> childBones;
    protected List<IKTarget> targets; //The targets that this bone is affected by

    public Vector3 offset_position;
    public Quaternion offset_rotation;

    protected CharacterJoint physicsJoint;
    protected Rigidbody physicsRigidbody;
    protected BoxCollider physicsCollider;

    protected bool jointBroken = false;
    protected bool jointDismembered = false;

    public IKConstraint constraint;
    //public constraintDirection
    public BoneIKRole targetRole = BoneIKRole.None;

    private void Awake()
    {
        childBones = new List<IKBone>();
    }

    public IKBone addBone(string nextName, float nextLengt, float nextWidth, float nextTickness, IKSkeleton parentSkeleton)
    {
        GameObject next = new GameObject(nextName);
        IKBone nextBone = next.AddComponent<IKBone>();

        nextBone.parentBone = this;
        this.childBones.Add(nextBone);

        next.transform.parent = this.gameObject.transform;
        //nextBone.offset = Matrix4x4.identity;
        next.transform.localRotation = Quaternion.identity;
        next.transform.localPosition = new Vector3(0.0f, length, 0.0f);

        nextBone.physicsRigidbody = next.gameObject.AddComponent<Rigidbody>();

        nextBone.physicsCollider = next.gameObject.AddComponent<BoxCollider>();
        if (this.physicsCollider != null)
            Physics.IgnoreCollision(this.physicsCollider, nextBone.physicsCollider);
        Physics.IgnoreCollision(parentSkeleton.mainCollider, nextBone.physicsCollider);
        nextBone.parentSkeleton = parentSkeleton;

        nextBone.physicsJoint = next.gameObject.AddComponent<CharacterJoint>();
        nextBone.physicsJoint.connectedBody = this.physicsRigidbody;
        nextBone.physicsJoint.enableProjection = true; //forces joints back into constraints should they leave them
        nextBone.physicsJoint.enablePreprocessing = true; //helps with overconstrainted systems

        nextBone.setLength(nextLengt);
        nextBone.setWidth(nextWidth);
        nextBone.setThickness(nextTickness);
        nextBone.ragdollJoint(false);

        nextBone.parentFork = nextBone.calcParentFork();
        nextBone.distanceToRoot = nextBone.calcDistanceToRoot();
        return nextBone;
    }


    public void initRoot(IKSkeleton skeleton)
    {
        this.physicsRigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.physicsRigidbody.isKinematic = true;
        this.length = 0.0f;
        this.parentBone = null;
        this.parentBone = null;
        this.parentSkeleton = skeleton;
    }

    public IKBone calcParentFork()
    {
        IKBone previous = this.parentBone;
        while (previous != null)
        {
            if (previous.transform.childCount > 1)
            { 
                return previous;
            }
            else
            { 
                previous = previous.parentBone;
            }
        }
        return null;
    }

    public void updateSkeletonTargets(IKTarget target)
    {
        IKBone previous = this;
        while (previous != null && previous.targets != null)
        {
            previous.targets.Add(target);
            previous = previous.parentBone;
        }
    }

    public float calcDistanceToRoot()
    {
        float dist = 0.0f;
        IKBone previous = this;
        while (previous != null)
        {
            dist+=length;
            previous = previous.parentBone;
        }
        return dist;
    }

    public IKTarget addTarget()
    {
        GameObject go = new GameObject(name + "_target");
        IKTarget target = go.AddComponent<IKTarget>();
        target.transform.parent = this.parentSkeleton.rootGO.transform;
        target.transform.rotation = this.transform.rotation;
        target.transform.position = this.transform.position;// + this.transform.localToWorldMatrix * new Vector3(0.0f, this.length, 0.0f);
        updateSkeletonTargets(target);
        return target;
    }


    public bool isFork()
    {
        return (this.childBones.Count > 1);
    }


    public void setLength(float length)
    {
        this.length = length;
        this.updateCollider();
    }

    public void setWidth(float width)
    {
        this.width = width;
        this.updateCollider();
    }

    public void setThickness(float thickness)
    {
        this.thickness = thickness;
        this.updateCollider();

    }

    private void updateCollider()
    {
        this.physicsCollider.size = new Vector3(width, length, thickness);
        this.physicsCollider.center = new Vector3(0.0f, this.physicsCollider.size.y * 0.5f, 0.0f);
    }

    public Matrix4x4 getLocalEndpoint()
    {
        return new Matrix4x4();
    }

    public Vector3 getEndPoint()
    {
        return this.gameObject.transform.InverseTransformPoint(new Vector3(0.0f, length, 0.0f));
    }


    public void ragdollJoint(bool doRagdoll)
    {
        this.physicsRigidbody.isKinematic = !doRagdoll;
    }

    public void ragdollJointAndChildren()
    {
        IKBone[] children = this.transform.GetComponentsInChildren<IKBone>();
        this.ragdollJoint(true);
        foreach (IKBone bone in children)
        {
            bone.ragdollJoint(true);
        }
    }

    public void breakJoint()
    {
        this.jointBroken = true;
        ragdollJointAndChildren();
    }

    public void dismemberJoint()
    {
        this.jointDismembered = true;
        ragdollJointAndChildren();
    }

    public void explodeJoint()
    {
        IKBone[] children = this.transform.GetComponentsInChildren<IKBone>();
        this.dismemberJoint();
        foreach (IKBone bone in children)
        {
            bone.dismemberJoint();
        } 
    }


    public Matrix4x4 getStarpointWorld()
    {
        Matrix4x4 mat = Matrix4x4.TRS(offset_position, offset_rotation, Vector3.one);
        mat *= parentBone.gameObject.transform.localToWorldMatrix;
        return mat;
    }

    public Matrix4x4 getEndpointWorld()
    {
        Matrix4x4 mat = Matrix4x4.Translate(new Vector3(0.0f, length, 0.0f));
        mat *= getStarpointWorld();
        return mat;
    }


    void FixedUpdate()
    {
        if (DEBUG_trigger)
        {
            this.addBone(this.name + "+", Random.Range(0.4f, 0.8f), Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), this.parentSkeleton);
            DEBUG_trigger = false;
        }
    }


    private void OnDrawGizmos()
    {
        if (this.parentBone != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.gameObject.transform.position, Mathf.Max(thickness,width)*0.5f);
            //Gizmos.DrawWireSphere(parentBone.gameObject.transform.position, Mathf.Max(thickness, width)*0.5f);
            Gizmos.DrawLine(parentBone.gameObject.transform.position, this.gameObject.transform.position);
        }
    }
}
