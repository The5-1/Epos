using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using The5.Math;


/** Solver
 * can be a line or tree of bones
 * has one or more targets
 * has one rooot
 * has a solve() method that updates positions
*/
[System.Serializable]
public abstract class IKSolver
{
    public string name;
    public IKBone chainRoot;
    public IKBone targetRoot;

    public IKSolver(string name, IKBone chainRoot, IKBone targetRoot)
    {
        this.name = name;
        this.chainRoot = chainRoot;
        this.targetRoot = targetRoot;
    }

    public abstract void solve();
}


/** A tree or chain defined by the first bone of it */
[System.Serializable]
public class IKSolverTwoBone : IKSolver
{   
    public IKBone upper;
    public IKBone lower;
    public IKBone end;
    public IKTarget target;
    public IKTarget hint;

    public IKSolverTwoBone(string name, IKBone chainRoot, IKBone targetRoot ,float length, float width, float thickness, float proportion) : base(name, chainRoot, targetRoot)
    {
        upper = chainRoot.addBone(name + "_upper", length * proportion, width * proportion, thickness * proportion);
        float proportionInv = 1.0f - proportion;
        lower = upper.addBone(name + "_lower", length * proportionInv, width * proportionInv, thickness * proportionInv);
        float maxWT = Mathf.Max(width, thickness);
        end = lower.addBone(name + "_end", maxWT * proportionInv, maxWT * proportionInv, maxWT * proportionInv);

        target = IKTarget.createTarget(name + "_target", targetRoot);
        target.transform.position = lower.getEndPointWorld();
    }

    public override void solve()
    {   
        Vector3 toTarget = target.transform.position - upper.getStartPointWorld();
        float maxLength = upper.length + lower.length;
        float minLength = upper.length - lower.length;
        float targetLength = toTarget.magnitude;

        upper.rotateLookAtLocal(target.transform.position);
        lower.rotateReset();
        end.rotateReset();
        

        if (maxLength < targetLength)
        {
            //Quaternion rotTowardsEnd = Quaternion.LookRotation(toTarget,upper.transform.forward);
            //rotTowardsEnd *= Quaternion.AngleAxis(90.0f, Vector3.right);
            //upper.transform.rotation = rotTowardsEnd;
            //lower.transform.rotation = rotTowardsEnd;         

            Debug.DrawLine(upper.getStartPointWorld(), target.transform.position, Color.red);
            return;
        }
        else if (targetLength < minLength)
        {


            Debug.DrawLine(upper.getStartPointWorld(), target.transform.position, Color.red);
            return;
        }
        else
        {
            

            //Debug.Log("upper: " + upper.length);
            //Debug.Log("lower: " + lower.length);
            //Debug.Log("distance: " + targetLength);

            float shoulderAngle = The5.Math.Trigonometry.angleOppositeC(upper.length, targetLength, lower.length);
            float ellbowAngle = The5.Math.Trigonometry.angleOppositeC(upper.length, lower.length, targetLength);
            float shoulderTwist = 0.0f;
            float wristTwisth = 0.0f;


            //Debug.Log("ellbow: " + ellbowAngle);
            //Debug.Log("shoulder: " + shoulderAngle);

            upper.transform.localEulerAngles += new Vector3(shoulderAngle, 0.0f, 0.0f);
            lower.transform.localEulerAngles -= new Vector3(180.0f - ellbowAngle, 0.0f, 0.0f);


            Debug.DrawLine(upper.getStartPointWorld(), target.transform.position, Color.green);
            return;
        }
        
    }
}

/**A 12-DOF Analytic Inverse Kinematics Solver for Human Motion Control*/
public class IKSolverHuman_ArmChain : IKSolver
{
    /** Law of cosines
     * for general tirangle with lengths a b c with angles A° B° C°
     * c²= a² + b² - 2AB*cos(C°) 
     * C° = acos( (a² + b² - c²)/(2*a*b)) 
     */

    IKBone root_chest;
    IKBone collar;
    IKBone upper;
    IKBone lower;
    IKBone hand;

    public IKTarget target;

    public IKSolverHuman_ArmChain(string name, IKBone chainRoot, IKBone targetRoot) : base(name, chainRoot, targetRoot)
    {

    }

    public override void solve()
    {
        Vector3 shoulderToWrist = hand.getEndPointWorld() - upper.transform.position;
        float shoulderToWrist_len = shoulderToWrist.magnitude;
        float ellbowAngle = The5.Math.Trigonometry.angleOppositeC(upper.length, lower.length, shoulderToWrist_len);

    }
}




[System.Serializable]
public class IKSolverCircleIntersection : IKSolver
{
    //** a new triangle theorem to solve the inverse kinematics problem for characters with highly articulated limbs */

    List<IKBone> bones;


    public IKSolverCircleIntersection(string name, IKBone chainRoot, IKBone targetRoot) : base(name, chainRoot, targetRoot)
    {
        bones = new List<IKBone>();

        IKBone previous = chainRoot;
        for(int i = 0; i < 10; i++)
        {
            IKBone newbone = previous.addBone("bone_" + i, 0.3f, 0.1f, 0.1f);
            bones.Add(newbone);
            previous = newbone;
        }
    }

    public override void solve()
    {
        for(int i = 1; i < bones.Count-1; i++) //iterate over n-2
        {
            IKBone child = bones[i - 1]; //seems more like the parent

            //Call method currentNode.setCurrentRadius with ( (currentNode.getTotalRadius plus currentNode.getActiveRadius) is greater than distance)
            float radius = child.length;

            if(child.childBones[0] != null && child != null)
            {
                float childToCurrentRadius = child.length;


            }
        }
    }
}


[System.Serializable]
public class IKSolverJacobian
{

}


[System.Serializable]
public class IKSolverFABRIK
{

}
