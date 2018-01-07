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

public enum BoneIKTargetType { None = 0, Arm, Leg, Weapon, Head, Spine, Wing };


public class IKTarget : MonoBehaviour
{
    public IKBone bone;

    public void setBone(IKBone end)
    {
        this.bone = end;
    }

    public float getDistanceToBone()
    {
        return (this.transform.position - bone.getEndPointWorld()).magnitude;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, Vector3.one * (0.1f + float.Epsilon));
    }
}

public class IKTargetCenter
{
    private int numTargets;
    private Vector3 sumTargets;
    private Quaternion sumRotations;


    public IKTargetCenter()
    {
        numTargets = 0;
        sumTargets = Vector3.zero;
        sumRotations = Quaternion.identity; //meaningfull average of rotations?
    }

    public void addTarget(IKTarget target) //, Quaternion rotation
    {
        sumTargets += target.transform.position;
        numTargets++;
    }

    public Vector3 getCenterPosition()
    {
        return sumTargets / Mathf.Max(numTargets, 1);
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

    public IKSkeleton parentSkeleton; //the skeleton/Root these bones belong to
    public IKBone parentFork; //The fork from which this chain branched
    public IKBone parentBone; //The bone one before this
    public List<IKBone> childBones;
    public List<IKTarget> targets; //The targets that this bone is affected by

    public Vector3 offset_position;
    public Quaternion offset_rotation;

    public CharacterJoint physicsJoint;
    public Rigidbody physicsRigidbody;
    public BoxCollider physicsCollider;

    public bool jointBroken = false;
    public bool jointDismembered = false;

    public IKConstraint constraint;
    //public constraintDirection
    public BoneIKRole targetRole = BoneIKRole.None;

    private void Awake()
    {
        childBones = new List<IKBone>();
        targets = new List<IKTarget>();
    }

    public IKBone addBone(string nextName, float nextLengt, float nextWidth, float nextTickness, IKSkeleton parentSkeleton)
    {
        GameObject next = new GameObject(nextName);
        next.transform.parent = this.gameObject.transform;
        next.transform.localRotation = Quaternion.identity;
        next.transform.localPosition = new Vector3(0.0f, length, 0.0f);

        if(true){ 
            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.GetComponent<Collider>().enabled = false;
            mesh.transform.parent = next.transform;
            mesh.transform.localScale = new Vector3(nextWidth, nextLengt, nextTickness);
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localPosition = new Vector3(0.0f, nextLengt* 0.5f, 0.0f);
        }

        IKBone nextBone = next.AddComponent<IKBone>();
        nextBone.parentBone = this;
        this.childBones.Add(nextBone);

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

        nextBone.parentFork = nextBone.updateParentFok();
        nextBone.distanceToRoot = nextBone.updateDistanceToRoot();
        return nextBone;
    }

    public void enablePhysics(bool enable)
    {
        this.physicsCollider.enabled = enable;
    }


    public void initRoot(IKSkeleton skeleton)
    {
        this.name = skeleton.name + "_bone_root";
        this.physicsRigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.physicsRigidbody.isKinematic = true;
        this.length = 0.0f;
        this.parentBone = null;
        this.parentFork = null;
        this.parentSkeleton = skeleton;
    }

    public IKBone updateParentFok()
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
        while (previous != null)
        {
            previous.targets.Add(target);
            previous = previous.parentBone;
        }
    }

    public float updateDistanceToRoot()
    {
        float dist = 0.0f;
        IKBone previous = this;
        while (previous != null)
        {
            dist+= previous.length;
            previous = previous.parentBone;
        }
        return dist;
    }

    public IKTarget addTarget()
    {
        GameObject go = new GameObject(name + "_target");
        IKTarget newTarget = go.AddComponent<IKTarget>();
        newTarget.setBone(this);
        newTarget.transform.parent = this.parentSkeleton.rootGO.transform;
        newTarget.transform.rotation = this.transform.rotation;
        newTarget.transform.position = this.transform.position;// + this.transform.localToWorldMatrix * new Vector3(0.0f, this.length, 0.0f);
        updateSkeletonTargets(newTarget);
        return newTarget;
    }

    public bool isFork()
    {
        return (this.childBones.Count > 1);
    }


    public Vector3 getTargetCenterWorld()
    {
        Vector3 sumPos = Vector3.zero;
        int numTargets = this.targets.Count;
        int numChildren = this.childBones.Count;
        if(numTargets > 0 && numChildren == 0)
        {    
            foreach(IKTarget t in this.targets)
            {
                sumPos += t.transform.position;
            }
            return sumPos / numTargets;
        }
        else if (numTargets > 0 && numChildren > 0)
        {
            foreach (IKBone b in this.childBones)
            {
                sumPos += b.transform.position;
            }
            return sumPos / numChildren;
        }
        return sumPos;
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


    public Vector3 getEndPointWorld()
    {
        return this.transform.TransformPoint(new Vector3(0.0f, length, 0.0f));
    }

    /*
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
    */

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
