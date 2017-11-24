using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    enum EnemyState { Idle, Patrol, Chase, Attack, Stun, Dead }
    [SerializeField] EnemyState state;


    private UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] public Transform targetTransform;

    [Header("Path")]
    public Transform[] path;
    private int pathIndex = 0;

    [Header("Distances")]
    public float chaseRange;
    public float attackRange;
    [SerializeField] private float distanceFromTarget = Mathf.Infinity;

    [Header("Timers")]
    public float idleTime = 1;
    private float timeCounter;
    public float cooldownAttack = 2;
    public float stunTime = 1;

    [Header("stats")]
    private bool canAttack = false;

    [Header("Properties")]
    public int hitDamage;
    public int life = 30;


    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SetIdle();
    }

    void Update()
    {
        CalculateDistanceFromTarget();
        switch(state)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
            case EnemyState.Patrol:
                PatrolUpdate();
                break;
            case EnemyState.Chase:
                ChaseUpdate();
                break;
            case EnemyState.Attack:
                AttackUpdate();
                break;
            case EnemyState.Stun:
                StunUpdate();
                break;
            case EnemyState.Dead:
                //DeadUpdate();
                break;
            default:
                break;
        }
    }
    #region Updates
    void IdleUpdate()
    {
        if(timeCounter >= idleTime)
        {
            SetPatrol();
        }
        else timeCounter += Time.deltaTime;
    }
    void PatrolUpdate()
    {
        if(distanceFromTarget < chaseRange)
        {
            SetChase();
            return;
        }

        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            pathIndex++;
            if(pathIndex >= path.Length) pathIndex = 0;
            SetPatrol();
        }
    }
    void ChaseUpdate()
    {
        agent.SetDestination(targetTransform.position);

        if(distanceFromTarget > chaseRange)
        {
            SetPatrol();
            return;
        }

        if(distanceFromTarget < attackRange)
        {
            SetAttack();
            return;
        }

    }
    void AttackUpdate()
    {
        agent.SetDestination(targetTransform.position);

        if(canAttack)
        {
            //damage player
            //5.6 agent.isStoped = true;
            agent.Stop();

            //targetTransform.GetComponent<PlayerNavigation>().SetDamage(hitDamage);

            idleTime = cooldownAttack;
            SetIdle();
            return;
        }

        if(distanceFromTarget > attackRange)
        {
            SetChase();
            return;
        }

    }
    void StunUpdate()
    {
        if(timeCounter >= stunTime)
        {
            idleTime = 0;
            SetIdle();
        }
        else timeCounter += Time.deltaTime;
    }
    void DeadUpdate() { }
    #endregion
    #region sets
    void SetIdle()
    {
        //Animacion de idle
        //Estate quieto

        timeCounter = 0;
        state = EnemyState.Idle;
    }
    void SetPatrol()
    {
        //agent.isStoped = false;
        agent.Resume();
        agent.SetDestination(path[pathIndex].position);
        state = EnemyState.Patrol;
    }
    void SetChase()
    {
        state = EnemyState.Chase;
    }
    void SetAttack()
    {
        state = EnemyState.Attack;
    }
    void SetStun()
    {
        agent.Stop();
        timeCounter = 0;
        state = EnemyState.Stun;
    }
    void SetDead()
    {
        agent.Stop();
        state = EnemyState.Dead;

        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }
    #endregion
    #region Public Functions
    public void SetDamage(int hit)
    {
        life -= hit;
        if(life <= 0)
        {
            SetDead();
            return;
        }
        SetStun();
    }
    #endregion
    void CalculateDistanceFromTarget()
    {
        distanceFromTarget = Vector3.Distance(transform.position, targetTransform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            canAttack = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            canAttack = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);


        Color newColor = Color.red;
        Gizmos.color = newColor;
        newColor.a = 0.2f;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
