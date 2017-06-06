using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class PhysicsRandomPusher : MonoBehaviour {

    public float force = 1.0f;
    private Rigidbody rb;
    private Collider col;

    void Start () {
        rb = this.gameObject.GetComponent<Rigidbody>();
        if (rb == null) { this.gameObject.AddComponent<Rigidbody>(); }
        col = this.gameObject.GetComponent<Collider>();
        if (col == null) { this.gameObject.AddComponent<Collider>(); }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if(rb != null) rb.AddForce(Random.insideUnitSphere* force);
	}
}
