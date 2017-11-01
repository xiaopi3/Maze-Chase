using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEditor.Animations;

public enum AI_ENEMY_STATE
{
    IDLE=2081823275,
    PATROL=207038023,
    CHASE=1463555229,
    ATTACK=1080829965,
    SEEKHEALTH=-833380208
};
public class AI_Enemy : MonoBehaviour {

    Animator ThisAnimator;
    NavMeshAgent ThisAgent;
    Transform ThisTransform, PlayerTransform,PlayerView;
    BoxCollider ThisCollider;
    public AI_ENEMY_STATE CurrentState = AI_ENEMY_STATE.IDLE;
    public float DistEps = 1;
    public float FieldOfView=60;
    public float ChaseTimeOut = 4f;
    public float AttackDelay = 1f;
    public float AttackDamage = 10;
    private bool CanSeePlayer=false;
    private EnemyHealth EnemyHealthScript;
    private AnimatorController ac;

    //

    //

    Transform[] Waypoints;
    void Awake()
    {
        ThisAnimator = GetComponent<Animator>();
        ThisAgent = GetComponent<NavMeshAgent>();
        ThisTransform = transform;
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerView = PlayerTransform.transform.Find("flag").transform;
        ThisCollider = GetComponent<BoxCollider>();
        EnemyHealthScript = GetComponent<EnemyHealth>();
        Waypoints = (from GameObject GO in GameObject.FindGameObjectsWithTag("Waypoint") select GO.transform).ToArray();
        ac = GetComponent<AnimatorController>();
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //CanSeePlayer = false;
        if (!ThisCollider.bounds.Contains(PlayerView.position))
            return;
        
        CanSeePlayer = HaveLineSightToPlayer(PlayerTransform);
        
	}
    public IEnumerator State_Idle()
    {
        print("Idle");
        CurrentState = AI_ENEMY_STATE.IDLE;
        ThisAnimator.SetTrigger((int)AI_ENEMY_STATE.IDLE);
        ThisAgent.Stop();
        ThisAgent.ResetPath();

        while (CurrentState == AI_ENEMY_STATE.IDLE)
        {
            print("Idle loop");
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
        //print("Idle over");
        StopAllCoroutines();
        StartCoroutine(State_Patrol());
    }
    public IEnumerator State_Patrol()
    {
        print("巡逻");
        CurrentState = AI_ENEMY_STATE.PATROL;
        ThisAnimator.SetTrigger((int)AI_ENEMY_STATE.PATROL);
        Transform RandomDest = Waypoints[Random.Range(0, Waypoints.Length)];
        ThisAgent.SetDestination(RandomDest.position);
        while (CurrentState == AI_ENEMY_STATE.PATROL)
        {
            //print("正在前往目的地");
            if (CanSeePlayer)
            {
                
                StartCoroutine(State_Chase());
                yield break;
            }
            if (Vector3.Distance(ThisTransform.position, RandomDest.position) <= DistEps)
            {
                //print("到达目的地" + ThisTransform.position);
                StartCoroutine(State_Idle());
                yield break;
            }
            yield return null;
        }
    }

    private bool HaveLineSightToPlayer(Transform Player)
    {
        //角度过大或者直线无遮挡
        float angle = Mathf.Abs(Vector3.Angle(ThisTransform.forward, (PlayerTransform.position - ThisTransform.position).normalized));
        if (angle > FieldOfView)
            return false;
        RaycastHit hitInfo;
        Physics.Linecast(ThisTransform.position, Player.position, out hitInfo);
        //print(hitInfo.collider);
        if (Physics.Linecast(ThisTransform.position, Player.position,out hitInfo))
            return false;
        return true;
    }
    public IEnumerator State_Chase()
    {
        print("追逐");
        CurrentState = AI_ENEMY_STATE.CHASE;
        ThisAnimator.SetTrigger((int)AI_ENEMY_STATE.CHASE);
        while (CurrentState == AI_ENEMY_STATE.CHASE)
        {
            ThisAgent.SetDestination(PlayerTransform.position);
            if (!CanSeePlayer)
            {
                float ElapsedTime = 0f;

                while (true)
                {
                    ElapsedTime += Time.deltaTime;
                    ThisAgent.SetDestination(PlayerTransform.position);
                    yield return null;

                    if (ElapsedTime >= ChaseTimeOut)
                    {
                        if (!CanSeePlayer)
                        {
                            StartCoroutine(State_Idle());
                            yield break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (CanSeePlayer&&Vector3.Distance(ThisTransform.position, PlayerTransform.position) <= DistEps)
                    {
                        StartCoroutine(State_Attack());
                        yield break;
                    }
                }
            }
            if (Vector3.Distance(ThisTransform.position, PlayerTransform.position) <= DistEps)
            {
                StartCoroutine(State_Attack());
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator State_Attack()//此处是否能控制动画和伤害同步显示？还未解决
    {
        print("攻击");
        CurrentState = AI_ENEMY_STATE.ATTACK;
        ThisAnimator.SetTrigger((int)AI_ENEMY_STATE.ATTACK);
        //想动态控制敌人攻击速度，目前未找到实现方法。
        PlayerTransform.SendMessage("ChangeHealth", -AttackDamage, SendMessageOptions.DontRequireReceiver);
        ThisAgent.Stop();
        ThisAgent.ResetPath();
        float ElapsedTime = 0f;
        while (CurrentState == AI_ENEMY_STATE.ATTACK)
        {
            ElapsedTime += Time.deltaTime;
            if (!CanSeePlayer || Vector3.Distance(ThisTransform.position, PlayerTransform.position) > DistEps)
            {
                StartCoroutine(State_Chase());
                yield break;
            }
            if (ElapsedTime >= AttackDelay)
            {
                
                print("继续攻击");
                ElapsedTime = 0f;
                PlayerTransform.SendMessage("ChangeHealth", -AttackDamage, SendMessageOptions.DontRequireReceiver);
            }
            yield return null;
        }
    }
    public IEnumerator State_SeekHealth()
    {
        CurrentState = AI_ENEMY_STATE.SEEKHEALTH;
        ThisAnimator.SetTrigger((int)AI_ENEMY_STATE.SEEKHEALTH);
        HealthRestore HR = null;
        while (CurrentState == AI_ENEMY_STATE.SEEKHEALTH)
        {
            if (HR == null)
            {
                HR = GetNearestHealthRestore(ThisTransform);
                if (HR != null)
                    ThisAgent.SetDestination(HR.transform.position);
            }
            if (HR == null || EnemyHealthScript.Health > EnemyHealthScript.HealthDangerLevel)
            {
                StartCoroutine(State_Idle());
                yield break;
            }
            yield return null;
        }
    }
    private HealthRestore GetNearestHealthRestore(Transform Target)
    {
        HealthRestore[] Restores = Object.FindObjectsOfType<HealthRestore>();
        float DistanceToNearest = Mathf.Infinity;
        HealthRestore Nearest = null;
        foreach (HealthRestore HR in Restores)
        {
            float CurrentDistance = Vector3.Distance(Target.position, HR.transform.position);
            if (CurrentDistance <= DistanceToNearest)
            {
                Nearest = HR;
                DistanceToNearest = CurrentDistance;
            }
        }
        return Nearest;
    }
}
