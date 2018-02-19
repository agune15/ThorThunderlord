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

    bool isSlowed = false;

    bool playerIsDead;

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
    bool hasAttacked = false;


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

                enemyAgent.angularSpeed = enemyAngularSpeed / 2;

                //Trigger ataque giratorio

                isSpinning = true;
                SetSpeed();
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
        if(enemyAgent.enabled)
        {
            if(!enemyAgent.isStopped)
            {
                enemyAgent.isStopped = true;
            }
            enemyAgent.enabled = false;
        }
        return;
    }

    #endregion

    #region Skull State Sets

    void SetChase()
    {
        if(skullState == SkullStates.Frozen) isFrozen = false;

        enemyAgent.SetDestination(targetTransform.position);

        enemyAgent.angularSpeed = enemyAngularSpeed;

        isSpinning = false;
        alreadyAttacked = false;
        hasAttacked = false;

        SetSpeed();

        skullState = SkullStates.Chase;
    }

    void SetAttack()
    {
        SetPlayerAttack();

        if(!isSpinning) skullAttack = SkullAttacks.BasicAttack;
        else skullAttack = SkullAttacks.SpinAttack;

        skullState = SkullStates.Attack;
    }

    void SetDead()
    {
        enemyAgent.isStopped = true;
        skullAnimator.SetTrigger("die");

        targetBehaviour.SetBasicAttackTransform(null, false);
        targetBehaviour.SetDestination(targetTransform.position);

        skullState = SkullStates.Dead;
    }

    #endregion

    #region Attack Updates

    void SpinAttackUpdate()
    {
        if(isSpinning)
        {
            if(hasAttacked)
            {
                targetBehaviour.SetDamage(10);

                hasAttacked = false;
                isSpinning = false;

                enemyAgent.angularSpeed = enemyAngularSpeed;

                skullAttack = SkullAttacks.BasicAttack;

                SetSpeed();
            }
        }
    }

    void BasicAttackUpdate()
    {
        if(!alreadyAttacked)
        {
            if(skullAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.25f && skullAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.4f)
            {
                if(hasAttacked)
                {
                    targetBehaviour.SetDamage(4);
                    alreadyAttacked = true;
                    hasAttacked = false;
                }
            }
        }
        else
        {
            if (skullAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.03f)
            {
                alreadyAttacked = false;
            }
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
        }
    }

    public void SkullHasAttacked ()
    {
        if(skullState == SkullStates.Attack) hasAttacked = true;
        else hasAttacked = false;
    }

    public void SetSlow (bool slowed)
    {
        isSlowed = slowed;

        SetSpeed();
    }

    void SetSpeed()
    {
        if(isSpinning) enemyAgent.speed = (isSlowed) ? enemySpeed : enemySpeed * 2;
        else enemyAgent.speed = (isSlowed) ? enemySpeed / 2 : enemySpeed;
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
