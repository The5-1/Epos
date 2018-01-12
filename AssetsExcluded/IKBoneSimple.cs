using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBoneSimple : MonoBehaviour
{
    public string name;

    public float length;
    public float width;
    public float thickness;

    IKBoneSimple parentBone;
    List<IKBoneSimple> childBones;

    public Rigidbody physicsRB;
    public Collider physicsCollider;
    public Joint physicsJoint;
    public GameObject attachmentGO;

    private void Awake()
    {
        if (false)
        {
            this.physicsRB = this.gameObject.AddComponent<Rigidbody>();
            this.physicsCollider = this.gameObject.AddComponent<BoxCollider>();
            this.physicsJoint = this.gameObject.AddComponent<CharacterJoint>();
        }

        if (true)
        {
            attachmentGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            attachmentGO.GetComponent<Collider>().enabled = false;
            attachmentGO.transform.parent = this.transform;
            attachmentGO.transform.localScale = new Vector3(width, length, thickness);
            attachmentGO.transform.localRotation = Quaternion.identity;
            attachmentGO.transform.localPosition = new Vector3(0.0f, length * 0.5f, 0.0f);
        }
    }

    void init(string name, float length, float width, float thickness, IKBoneSimple parentBone)
    {
        this.name = name;
        this.length = length;
        this.width = width;
        this.thickness = thickness;
        this.parentBone = parentBone;
    }

    void refresh()
    {

    }

}
