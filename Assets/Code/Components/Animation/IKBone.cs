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

public enum BoneIKRole { None = 0, Arm, Leg, Head, Spine, Wing};
public enum BoneIKSide { Left, Right, Center};

[System.Serializable]
public class BodyPartStats
{
    float DamageResistance = 0.0f; //keep it simple
}

public class IKBone : MonoBehaviour {

    public bool DEBUG_trigger = false;

    public float length = 0.5f;
    public float thickness = 0.1f;
    public float width = 0.1f;

    public Vector3 offset_position;
    public Quaternion offset_rotation;

    protected IKSkeleton parentSkeleton;
    protected IKBone parentBone;
    protected List<IKBone> childBones;

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

        nextBone.physicsJoint = next.gameObject.AddComponent<CharacterJoint>();
        nextBone.physicsJoint.connectedBody = this.physicsRigidbody;
        nextBone.physicsJoint.enableProjection = true; //forces joints back into constraints should they leave them
        nextBone.physicsJoint.enablePreprocessing = true; //helps with overconstrainted systems

        nextBone.setLength(nextLengt);
        nextBone.setWidth(nextWidth);
        nextBone.setThickness(nextTickness);
        nextBone.setKinematik();

        return nextBone;
    }

    public void initRoot(IKSkeleton skeleton)
    {
        this.physicsRigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.physicsRigidbody.isKinematic = true;
        this.length = 0.0f;
        this.parentBone = null;
        this.parentSkeleton = skeleton;
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

    public void setKinematik()
    {
        this.jointBroken = false;
        this.jointDismembered = false;
        this.physicsRigidbody.isKinematic = true;
    }

    public void breakJoint()
    {
        jointBroken = true;
        this.physicsRigidbody.isKinematic = false;
    }

    public void dismemberJoint()
    {
        jointDismembered = true;
        this.transform.parent = null;
        this.physicsRigidbody.isKinematic = false;
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
            this.addBone("newBone", Random.Range(0.4f, 0.8f), Random.Range(0.1f, 0.3f), Random.Range(0.1f, 0.3f), this.parentSkeleton);
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
