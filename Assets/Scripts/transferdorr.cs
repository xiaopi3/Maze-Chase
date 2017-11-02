using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transferdorr : MonoBehaviour {

    public Transform to;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            other.transform.position = to.position;
        }
    }
}
