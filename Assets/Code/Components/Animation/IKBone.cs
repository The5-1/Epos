using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental; //transform matrix

public enum BoneIKTargetType { None = 0, Arm, Leg, Weapon, Head, Spine, Wing };


public class IKTarget : MonoBehaviour
{
    public IKBone bone;
    public IKBone targetRoot;

    public static IKTarget createTarget(string name, IKBone targetRoot)
    {
        GameObject go = new GameObject(name);
        IKTarget newTarget = go.AddComponent<IKTarget>();
        newTarget.transform.parent = targetRoot.transform;
        newTarget.targetRoot = targetRoot;
        return newTarget;
    }

    public void setBone(IKBone bone)
    {
        this.bone = bone;
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

/** Bone handles common Behaviour between all bones
 * no specific IK solver data
 * (IK solver will iterate entire chain/tree and gather relevant data)
*/

[System.Serializable]
public class IKBone : MonoBehaviour
{
    public float length = 0.5f;
    public float width = 0.1f;
    public float thickness = 0.1f;

    public Vector3 offset = Vector3.zero;
    public Quaternion orientation = Quaternion.identity;

    public IKSkeleton parentSkeleton;
    public IKBone parentBone;
    public List<IKBone> childBones;
   
    public Rigidbody physicsRigidbody;
    public BoxCollider physicsCollider;
    public CharacterJoint physicsJoint;

    public GameObject attachment;

    public bool jointBroken = false;
    public bool jointDismembered = false;

    private void Awake()
    {
        childBones = new List<IKBone>();
    }

    protected void initPhysics()
    {
        this.physicsRigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.setKinematik(true);

        this.physicsCollider = this.gameObject.AddComponent<BoxCollider>();

        this.physicsJoint = this.gameObject.AddComponent<CharacterJoint>();
        this.physicsJoint.enableProjection = true; //forces joints back into constraints should they leave them
        this.physicsJoint.enablePreprocessing = true; //helps with overconstrainted systems

        this.refreshCollider();
    }

    protected void initAttachment()
    {
        this.attachment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        this.attachment.name = this.name +"_attachment";
        this.attachment.GetComponent<Collider>().enabled = false;
        this.attachment.transform.parent = this.gameObject.transform;

        refreshAttachment();
    }

    public static IKBone createBone(string name, float length, float width, float thickness, IKSkeleton parentSkeleton)
    {
        GameObject boneGO = new GameObject(name);
        IKBone bone = boneGO.AddComponent<IKBone>();
        bone.parentSkeleton = parentSkeleton;
        bone.length = length;
        bone.width = width;
        bone.thickness = thickness;
        bone.initPhysics();
        bone.initAttachment();

        return bone;
    }

    public void connectTo(IKBone parentBone, bool hierarchy = true, bool ignoreCollisions = true, bool connectJoints = true)
    {
        this.parentBone = parentBone;
        parentBone.childBones.Add(this);

        refreshPosition(parentBone, hierarchy);

        if (connectJoints)
        {
            parentBone.physicsJoint.connectedBody = this.physicsRigidbody;
        }

        if (ignoreCollisions)
        {
            Physics.IgnoreCollision(parentBone.physicsCollider, this.physicsCollider);
            Physics.IgnoreCollision(parentBone.attachment.GetComponent<Collider>(), this.attachment.GetComponent<Collider>());
        }
    }

    private void refreshPosition(IKBone parentBone, bool hierarchy = false)
    {
        if (hierarchy) //set the previous bone as parent
        {
            this.transform.parent = parentBone.transform;
            this.transform.localRotation = Quaternion.identity * this.orientation;
            this.transform.localPosition = parentBone.getEndPointLocal() + this.offset;
        }
        else //set the root as parent
        {
            this.transform.parent = parentBone.parentSkeleton.rootBone.transform;
            this.transform.localRotation = Quaternion.identity * this.orientation;
            this.transform.localPosition = parentBone.getEndPointLocal() + this.offset;
        }
    }

    private void refreshCollider()
    {
        this.physicsCollider.size = new Vector3(this.width, this.length, this.thickness);
        this.physicsCollider.center = new Vector3(0.0f, this.physicsCollider.size.y * 0.5f, 0.0f);
    }

    private void refreshAttachment()
    {
        this.attachment.transform.localScale = new Vector3(this.width, this.length, this.thickness);
        this.attachment.transform.localRotation = Quaternion.identity;
        this.attachment.transform.localPosition = new Vector3(0.0f, this.length * 0.5f, 0.0f);
    }

    public void setKinematik(bool enable)
    {
        this.physicsRigidbody.isKinematic = enable;
    }

    public IKBone addBone(string name, float length, float width, float thickness)
    {
        IKBone newbone = IKBone.createBone(name, length, width, thickness, this.parentSkeleton);
        newbone.connectTo(this);
        return newbone;
    }

    public Vector3 getEndPointLocal()
    {
        return new Vector3(0.0f, length, 0.0f);
    }

    public Vector3 getEndPointWorld()
    {
        return this.transform.TransformPoint(getEndPointLocal());
    }

    public Vector3 getStartPointLocal()
    {
        return Vector3.zero;
    }

    public Vector3 getStartPointWorld()
    {
        return this.transform.TransformPoint(getStartPointLocal());
    }

    public void rotateLookAtLocal(Vector3 pos)
    {
        this.transform.LookAt(pos, parentBone.transform.localToWorldMatrix * Vector3.up);
        this.transform.localRotation *= Quaternion.AngleAxis(90.0f, Vector3.right);
    }

    public void rotateReset()
    {
        this.transform.localRotation = Quaternion.identity;
    }

    /*
    public IKBone addBone(string nextName, float nextLengt, float nextWidth, float nextTickness, IKSkeleton parentSkeleton)
    {
        GameObject next = new GameObject(nextName);
        next.transform.parent = this.gameObject.transform;
        next.transform.localRotation = Quaternion.identity;
        next.transform.localPosition = new Vector3(0.0f, length, 0.0f);

        if (true)
        {
            GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.GetComponent<Collider>().enabled = false;
            mesh.transform.parent = next.transform;
            mesh.transform.localScale = new Vector3(nextWidth, nextLengt, nextTickness);
            mesh.transform.localRotation = Quaternion.identity;
            mesh.transform.localPosition = new Vector3(0.0f, nextLengt * 0.5f, 0.0f);
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
    */


}




/*
[System.Serializable]
public class IKBoneFABRIK {

    public bool DEBUG_trigger = false;


    public float distanceToRoot;



    public IKBone parentFork; //The fork from which this chain branched


    public List<IKTarget> targets; //The targets that this bone is affected by

    public Vector3 offset_position;
    public Quaternion offset_rotation;

    public IKConstraint constraint;
    //public constraintDirection
    public BoneIKRole targetRole = BoneIKRole.None;






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
*/