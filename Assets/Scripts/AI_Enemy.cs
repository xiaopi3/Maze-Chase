using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AI_ENEMY_STATE
{
    IDLE=2081823275,
    PATRL=207038023,
    CHASE=1463555229,
    ATTACK=1080829965,
    SEEKHEALTH=-833380208
};
public class AI_Enemy : MonoBehaviour {

    Animator ThisAnimator;
    NavMeshAgent ThisAgent;
    Transform ThisTransform, PlayerTransform;
    BoxCollider ThisCollider;
    public AI_ENEMY_STATE CurrentState = AI_ENEMY_STATE.IDLE;
    void Awake()
    {
        ThisAnimator = GetComponent<Animator>();
        ThisAgent = GetComponent<NavMeshAgent>();
        ThisTransform = transform;
        //PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        ThisCollider = GetComponent<BoxCollider>();
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public IEnumerator State_Idle()
    {
        CurrentState = AI_ENEMY_STATE.IDLE;
        ThisAnimator.SetTrigger((int)AI_ENEMY_STATE.IDLE);
        ThisAgent.Stop();
        while (CurrentState == AI_ENEMY_STATE.IDLE)
        {
            if (CanSeePlayer)
            {
                StartCoroutine(State_Chase());
                yield break;
            }
            yield return null;
        }
    }
    public void OnIdleAnimCompleted()
    {
        StopAllCoroutines();
        StartCoroutine(State_Patrol());
    }
}
