using UnityEngine;
using System.Collections;

public class FollowLookAtScript : MonoBehaviour
{
    public Transform target;
    public float heightOffset = 10f;

    void FixedUpdate()
    {
        this.transform.LookAt(target);

        Vector3 targetPos = target.position + this.transform.forward * -1.0f * 10.0f;
        targetPos.y = heightOffset;
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, 10.0f * Time.deltaTime);
    }
}