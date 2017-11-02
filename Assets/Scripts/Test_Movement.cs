using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Movement : MonoBehaviour {
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.AddForce(transform.forward * 10,ForceMode.Force);	
	}
}
