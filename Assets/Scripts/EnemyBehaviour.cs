using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    //enum EnemyType { Skull, Fenrir }
    //public EnemyType enemyType;

    enum SkullStates { Frozen, Chase, Attack, Dead }
    [SerializeField] SkullStates skullState = SkullStates.Frozen;

    enum SkullAttacks { SpinAttack, BasicAttack }
    [SerializeField] SkullAttacks skullAttack = SkullAttacks.BasicAttack;


    NavMeshAgent enemyAgent;
    Transform targetTransform;
    CharacterBehaviour targetBehaviour;
    //Animator skullAnimator;

    public float life;

    float enemySpeed;
    float enemyAngularSpeed;

    //Chase parameters
    public float unfreezeTime;
    public float chaseRange;

    //Attack parameters
    public float attackRange;

    bool isSpinning = false;

    bool alreadyAttacked;



	void Start () {
        enemyAgent = this.GetComponent<NavMeshAgent>();

        targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        targetBehaviour = targetTransform.gameObject.GetComponent<CharacterBehaviour>();

        enemyAgent.SetDestination(transform.position);

        enemySpeed = enemyAgent.speed;
        enemyAngularSpeed = enemyAgent.angularSpeed;
    }


	void Update () {
        SkullBehaviour();
	}

    void SkullBehaviour()
    {
        if(life <= 0) SetDead();

        switch(skullState)
        {
            case SkullStates.Frozen:
                FronzenUpdate();
                break;
            case SkullStates.Chase:
                ChaseUpdate();
                break;
            case SkullStates.Attack:
                ChaseUpdate();
                break;
            case SkullStates.Dead:
                DeadUpdate();
                break;
            default:
                break;
        }
    }

    #region Skull State Updates

    void FronzenUpdate ()
    {
        enemyAgent.isStopped = true;

        if (Vector3.Distance(transform.position, targetTransform.position) < chaseRange)
        {
            //Un sonido de chase
            SetChase();         
        }
    }

    void ChaseUpdate ()
    {
        if (unfreezeTime > 0)
        {
            unfreezeTime -= Time.deltaTime;
            return;
        }

        enemyAgent.isStopped = false;

        if (enemyAgent.remainingDistance > chaseRange)
        {
            //Ataque giratorio!!
            if(!isSpinning)
            {
                skullAttack = SkullAttacks.SpinAttack;
                enemyAgent.speed = enemySpeed * 2;
                enemyAgent.angularSpeed = enemyAngularSpeed / 2;
                
                //Trigger ataque giratorio

                isSpinning = true;
            }
        }

        if(enemyAgent.remainingDistance < attackRange)
        {
            enemyAgent.isStopped = true;
            SetAttack();
        }
        else
        {
            enemyAgent.SetDestination(targetTransform.position);
        }
    }

    void AttackUpdate ()
    {
        enemyAgent.isStopped = true;

        if (enemyAgent.remainingDistance > attackRange)
        {
            SetChase();
        }

        switch(skullAttack)
        {
            case SkullAttacks.SpinAttack:
                SpinAttackUpdate();
                break;
            case SkullAttacks.BasicAttack:
                BasicAttackUpdate();
                break;
            default:
                break;
        }
    }

    void DeadUpdate ()
    {
        if (!enemyAgent.isStopped) enemyAgent.isStopped = true;
        return;
    }

    #endregion

    #region Skull State Sets

    void SetChase ()
    {
        enemyAgent.SetDestination(targetTransform.position);
        enemyAgent.speed = enemySpeed;
        enemyAgent.angularSpeed = enemyAngularSpeed;

        isSpinning = false;
        alreadyAttacked = false;

        skullState = SkullStates.Chase;
    }

    void SetAttack ()
    {
        skullState = SkullStates.Attack;
    }

    void SetDead ()
    {
        enemyAgent.isStopped = true;

        skullState = SkullStates.Dead;
    }

    #endregion

    #region Attack Updates

    void SpinAttackUpdate()
    {
        if (isSpinning)
        {
            //haz daño si el trigger le da

            enemyAgent.speed = enemySpeed;
            enemyAgent.angularSpeed = enemyAngularSpeed;

            skullAttack = SkullAttacks.BasicAttack;
            isSpinning = false;
        }
    }

    void BasicAttackUpdate()
    {
        if (!alreadyAttacked)
        {
            //Llama al script del enemy para atacar

            //Si va a haber mas de un script de SkullTrigger, hacemos un array?

            alreadyAttacked = true;
        }
        else
        {
            /*
            if (skullAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.03f)
            {
                alreadyAttacked = false;

                //resetar ataque en el script de ataque del enemigo
            }*/
        }
    }

    #endregion

    #region Public Methods

    public void SetDamage (float damage)
    {
        life -= damage;

        if(life <= 0) SetDead();
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
