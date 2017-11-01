using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    public float Health = 100;
    public float HealthDangerLevel = 20;
    private AI_Enemy AI_Enemy_scripts;
    void Awake()
    {
        AI_Enemy_scripts = GetComponent<AI_Enemy>();
    }
    public void ChangeHealth(float Amount)
    {
        Health += Amount;
        if (Health <= 0)
        {
            StopAllCoroutines();
            Destroy(gameObject);
            return;
        }
        if (Health > HealthDangerLevel) return;

        StopAllCoroutines();
        //StartCoroutine(AI_Enemy_scripts.State_SeekHealth());
    }
}
