using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget2 : MonoBehaviour
{
    public IKChainBase chain;

    public void init(IKChainBase chain)
    {
        this.chain = chain;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, Vector3.one * (0.1f + float.Epsilon));
    }
}


public abstract class IKChainBase
{
    public string name;
    public readonly float maxLength;

    protected GameObject root; //the object this chain is attached to
    protected List<IKBoneSimple> bones;
    protected IKTarget2 target; //the target to stretch to

    protected IKChainBase(string name)
    {
        this.name = name;
    }

    public abstract void Update();

    public abstract float UpdateMaxLength();

    public void attach(GameObject parent)
    {
        first.gameObject.transform.parent = parent.transform;
    }

}

public class IKChainTwoBone : IKChainBase
{
    private IKBoneSimple second;

    public Transform hint;

    protected IKChainTwoBone(string name) : base(name)
    {
        GameObject first_GO = new GameObject(name + "_first");
        first = first_GO.AddComponent<IKBoneSimple>();
        GameObject second_GO = new GameObject(name + "_second");
        second = second_GO.AddComponent<IKBoneSimple>();

        GameObject target_GO = new GameObject(name + "_target");
        target = target_GO.AddComponent<IKTarget2>();
        target.init(this);
    }

    public override void Update()
    {
        if (root == null || target == null || first == null || second == null) return;

    }

    public override float UpdateMaxLength()
    {
        return 1.0f;
    }
}


public class IKSkeleton2 : MonoBehaviour {

    List<IKChainBase> ikChains;


    private void Awake()
    {
        ikChains = new List<IKChainBase>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
