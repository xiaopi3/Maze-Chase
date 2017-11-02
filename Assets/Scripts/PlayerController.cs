using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float PlayerSpeed=4;
    float LR;
    float LeftRight;

    Animator PlayerAnim;
    Rigidbody rigd;
    Transform ThisTransform;
    
    void Awake()
    {
        PlayerAnim = GetComponent<Animator>();
        rigd = GetComponent<Rigidbody>();
        ThisTransform = transform;
        
    }
	// Use this for initialization
	void Start () {

	}
    void Update()
    {
        LeftRight = Input.GetAxisRaw("Mouse X");
        LR = Input.GetAxis("Mouse X");
        
        if (Input.GetKey(KeyCode.W))
        {
            //print("开始跑");
            PlayerAnim.SetBool("Left", false);
            PlayerAnim.SetBool("Right", false);
            PlayerAnim.SetBool("Idle", false);
            PlayerAnim.SetBool("Run", true);
            rigd.velocity = transform.forward * PlayerSpeed;
          
            if (LeftRight != 0)
            {
                ThisTransform.Rotate(Vector3.up, Time.deltaTime * 120 * LR);
                if (LeftRight > 0)
                {
                    PlayerAnim.SetBool("Right", true);
                    PlayerAnim.SetBool("Run", false);
                }
                else
                {
                    PlayerAnim.SetBool("Left", true);
                    PlayerAnim.SetBool("Run", false);
                }
            }
        }
        else
        {
            //print("停止");
            PlayerAnim.SetBool("Idle",true);
            PlayerAnim.SetBool("Left", false);
            PlayerAnim.SetBool("Right", false);
            PlayerAnim.SetBool("Run", false);
            rigd.velocity = Vector3.zero;
            ThisTransform.Rotate(Vector3.up, Time.deltaTime * 120 * LR);
        }        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

	}
    public void OnIdleAnimCompleted()
    {
        //print("Idle over");
        
    }
}
