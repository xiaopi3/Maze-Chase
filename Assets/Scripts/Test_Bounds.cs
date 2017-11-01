using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Bounds : MonoBehaviour {
    public Transform ob;
	// Use this for initialization
	void Update () {
        RaycastHit hitInfo;
        if(Physics.Linecast(transform.position, ob.position, out hitInfo))
            print(hitInfo.collider.tag);
	}
	
}
