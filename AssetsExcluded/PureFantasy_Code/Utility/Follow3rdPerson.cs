using UnityEngine;
using System.Collections;

public class Follow3rdPerson : MonoBehaviour {

    public Transform target;
    public float heightOffset = 10f;
    public float speed = 0.5f;

    void FixedUpdate()
    {
        this.transform.LookAt(target);

        Vector3 targetPos = target.position + target.transform.forward * -1.0f * 10.0f;
        targetPos.y = target.position.y + heightOffset;
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, speed * Time.deltaTime);
    }
}
