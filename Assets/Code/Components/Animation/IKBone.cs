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


public class IKBone : MonoBehaviour {

    float length;


    public void breakJoint()
    {

    }

    public void dismemberJoint()
    {

    }

    public void delete()
    {
        this.enabled = false;
        Destroy(this);
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
