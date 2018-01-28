using UnityEngine;
using System.Collections;

public class LookAtScript : MonoBehaviour
{
    public Transform target;

    void FixedUpdate()
    {
        this.transform.LookAt(target);
    }
}