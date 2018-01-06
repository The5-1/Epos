using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IKConstraint
{


}
[System.Serializable]
public class IKHingeConstraint : IKConstraint
{

}

public enum BoneIKTargetRole { isNoTarget = 0, Arm = 1, Leg = 2, Head = 3, Spine = 4, Wing = 5};

public class IKBone : MonoBehaviour {

    public float length = 0.5f;
    public float thickness = 0.1f;

    protected IKBone parentBone;
    protected List<IKBone> childBones;

    protected CharacterJoint physicsJoint;
    protected Rigidbody physicsRigidbody;
    protected CapsuleCollider physicsCollider;

    protected bool jointBroken = false;
    protected bool jointDismembered = false;

    public Transform target;
    public IKConstraint constraint;
    //public constraintDirection
    public BoneIKTargetRole targetRole = BoneIKTargetRole.isNoTarget;

    private void Awake()
    {
        childBones = new List<IKBone>();
    }

    public IKBone addBone(string nextName, float nextLengt, float nextTickness, Collider ignoredCollider)
    {
        GameObject next = new GameObject(nextName);
        IKBone nextBone = next.AddComponent<IKBone>();

        nextBone.parentBone = this;
        this.childBones.Add(nextBone);

        next.transform.parent = this.gameObject.transform;
        next.transform.localRotation = Quaternion.identity;
        next.transform.localPosition = new Vector3(0.0f, length, 0.0f);

        nextBone.physicsRigidbody = next.gameObject.AddComponent<Rigidbody>();

        nextBone.physicsCollider = next.gameObject.AddComponent<CapsuleCollider>();
        if (this.physicsCollider != null)
            Physics.IgnoreCollision(this.physicsCollider, nextBone.physicsCollider);
        Physics.IgnoreCollision(ignoredCollider, nextBone.physicsCollider);

        nextBone.physicsJoint = next.gameObject.AddComponent<CharacterJoint>();
        nextBone.physicsJoint.connectedBody = this.physicsRigidbody;

        nextBone.setLength(nextLengt);
        nextBone.setThickness(nextTickness);

        return nextBone;
    }


    public void setThickness(float radius)
    {
        this.thickness = radius;
        this.updateCollider();

    }

    public void setLength(float length)
    {
        this.length = length;
        this.updateCollider();
    }

    private void updateCollider()
    {
        this.physicsCollider.radius = this.thickness;
        this.physicsCollider.height = this.length + this.thickness;
        this.physicsCollider.center = new Vector3(0.0f, this.physicsCollider.height * 0.5f, 0.0f);
    }

    public void initRoot()
    {
        this.physicsRigidbody = this.gameObject.AddComponent<Rigidbody>();
        this.physicsRigidbody.isKinematic = true;
        this.length = 0.0f;
        this.parentBone = null;
    }

    public Matrix4x4 getLocalEndpoint()
    {
        return new Matrix4x4();
    }

    public Vector3 getEndPoint()
    {
        return this.gameObject.transform.InverseTransformPoint(new Vector3(0.0f, length, 0.0f));
    }

    public void breakJoint()
    {
        jointBroken = true;
    }

    public void dismemberJoint()
    {
        jointDismembered = true;
        this.transform.parent = null;
    }

    private void OnDrawGizmos()
    {
        if (this.parentBone != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.gameObject.transform.position, thickness);
            Gizmos.DrawWireSphere(parentBone.gameObject.transform.position, thickness);
            Gizmos.DrawLine(this.gameObject.transform.position, parentBone.gameObject.transform.position);
        }
    }
}
