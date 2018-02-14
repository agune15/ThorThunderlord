using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkullBehaviour : MonoBehaviour {

    enum SkullStates { Frozen, Chase, Attack, Dead }
    [SerializeField] SkullStates skullState = SkullStates.Frozen;

    enum SkullAttacks { SpinAttack, BasicAttack }
    [SerializeField] SkullAttacks skullAttack = SkullAttacks.BasicAttack;


    NavMeshAgent enemyAgent;
    Transform targetTransform;
    CharacterBehaviour targetBehaviour;
    Animator skullAnimator;

    float life;

    float enemySpeed;
    float enemyAngularSpeed;

    bool isFrozen = true;

    //Chase parameters
    public float unfreezeTime;
    public float chaseRange;

    bool isChasing = false;

    //Attack parameters
    public float attackRange;
    float basicAttackDuration;

    bool isSpinning = false;

    bool isAttacking;
    bool alreadyAttacked = false;



    void Start() {
        enemyAgent = this.GetComponent<NavMeshAgent>();

        targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        targetBehaviour = targetTransform.gameObject.GetComponent<CharacterBehaviour>();
        skullAnimator = this.GetComponentInChildren<Animator>();

        foreach(AnimationClip clip in skullAnimator.runtimeAnimatorController.animationClips)
        {
            if(clip.name == "hit") basicAttackDuration = clip.length;
        }

        enemyAgent.SetDestination(transform.position);

        enemySpeed = enemyAgent.speed;
        enemyAngularSpeed = enemyAgent.angularSpeed;
    }


    void Update() {
        SkullBehaviourUpdate();

        AnimatorUpdate();
    }

    void SkullBehaviourUpdate()
    {
        if(life <= 0 && skullState != SkullStates.Dead) SetDead();

        switch(skullState)
        {
            case SkullStates.Frozen:
                FronzenUpdate();
                break;
            case SkullStates.Chase:
                ChaseUpdate();
                break;
            case SkullStates.Attack:
                AttackUpdate();
                break;
            case SkullStates.Dead:
                DeadUpdate();
                break;
            default:
                break;
        }
    }

    #region Skull State Updates

    void FronzenUpdate()
    {
        enemyAgent.isStopped = true;

        if(Vector3.Distance(transform.position, targetTransform.position) < chaseRange)
        {
            //Un sonido de chase
            SetChase();
        }
    }

    void ChaseUpdate()
    {
        if(unfreezeTime > 0)
        {
            unfreezeTime -= Time.deltaTime;
            return;
        }

        enemyAgent.isStopped = false;
        enemyAgent.SetDestination(targetTransform.position);

        if(enemyAgent.remainingDistance > chaseRange)
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
            SetAttack();
        }
    }

    void AttackUpdate()
    {
        enemyAgent.isStopped = true;
        SetRotation();

        //SetPlayerAttack();

        if(Vector3.Distance(transform.position, targetTransform.position) > attackRange)
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

    void DeadUpdate()
    {
        if(!enemyAgent.isStopped) enemyAgent.isStopped = true;
        return;
    }

    #endregion

    #region Skull State Sets

    void SetChase()
    {
        if(skullState == SkullStates.Frozen) isFrozen = false;

        enemyAgent.SetDestination(targetTransform.position);
        enemyAgent.speed = enemySpeed;
        enemyAgent.angularSpeed = enemyAngularSpeed;

        isSpinning = false;
        alreadyAttacked = false;

        skullState = SkullStates.Chase;
    }

    void SetAttack()
    {
        SetPlayerAttack();

        skullState = SkullStates.Attack;
    }

    void SetDead()
    {
        enemyAgent.isStopped = true;
        skullAnimator.SetTrigger("die");

        skullState = SkullStates.Dead;
    }

    #endregion

    #region Attack Updates

    void SpinAttackUpdate()
    {
        if(isSpinning)
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
        if(!alreadyAttacked)
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

    #region Public Methods and Others

    public void SetLife(float currentLife)
    {
        life = currentLife;
    }

    void SetRotation ()
    {
        transform.LookAt(targetTransform);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void SetPlayerAttack ()
    {
        bool playerIsAttacking;
        bool playerIsMoving;
        targetBehaviour.IsPlayerAttacking(out playerIsAttacking, out playerIsMoving);

        if(!playerIsAttacking) //&& !playerIsMoving por si solo queremos hacerlo cuando el player este quieto
        {
            targetBehaviour.SetBasicAttackTransform(this.transform, true);
            Debug.Log("heeey");
        }

        Debug.Log("playerATtack Set!");
        Debug.Log("playerisAttacking: " + playerIsAttacking);
        Debug.Log("playerisMoving: " + playerIsMoving);
    }

    void AnimatorUpdate()
    {
        if(skullState == SkullStates.Chase) isChasing = true;
        else isChasing = false;

        if(isChasing) //!isSpinning para hacerlo solo cuando corra
        {
            skullAnimator.speed = enemyAgent.velocity.magnitude / enemyAgent.speed;
        }
        else skullAnimator.speed = 1;

        if(skullState == SkullStates.Attack) isAttacking = true;
        else isAttacking = false;

        skullAnimator.SetBool("isFrozen", isFrozen);
        skullAnimator.SetBool("isSpinning", isSpinning);
        skullAnimator.SetBool("isChasing", isChasing);
        skullAnimator.SetBool("isAttacking", isAttacking);
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
