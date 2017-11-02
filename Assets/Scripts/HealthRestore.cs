using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRestore : MonoBehaviour {

    private EnemyHealth EnemyHealthScript;
    void Awake()
    {
        EnemyHealthScript = GetComponent<EnemyHealth>();
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            //加血
        }
        Destroy(gameObject);
    }
}
