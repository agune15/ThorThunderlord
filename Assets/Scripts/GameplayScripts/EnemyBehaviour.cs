using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {

    public EnemyStats.EnemyType enemyType;

    enum EnemyStates { Frozen, Chase, Attack, Dead }
    [SerializeField] EnemyStates enemyState = EnemyStates.Frozen;

    enum SkullAttacks { SpinAttack, BasicAttack }
    [SerializeField] SkullAttacks skullAttack = SkullAttacks.BasicAttack;

    enum FenrirAttacks { JumpAttack, BasicAttack }
    [SerializeField] FenrirAttacks fenrirAttack = FenrirAttacks.BasicAttack;

    NavMeshAgent enemyAgent;
    Transform targetTransform;
    CharacterBehaviour targetBehaviour;
    Animator enemyAnimator;
    
    float life;

    float enemySpeed;
    float enemyAngularSpeed;

    bool isFrozen = true;

    bool isSlowed = false;

    bool playerIsDead;  //Necesario?

    //Chase parameters
    public float unfreezeTime;
    public float chaseRange;

    bool isChasing = false;

    //Attack parameters
    public float attackRange;
    float basicAttackDuration;

    bool isAttacking;
    bool alreadyAttacked = false;
    bool hasAttacked = false;

        //Skull Only
        bool isSpinning = false;

        //Fenrir Only
        int basicAttackIndex;
        int baAnimationLength;
        int baTemporaryIndex;



    void Start()
    {
        enemyType = this.GetComponent<EnemyStats>().GetEnemyType();

        enemyAgent = this.GetComponent<NavMeshAgent>();

        targetTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        targetBehaviour = targetTransform.gameObject.GetComponent<CharacterBehaviour>();
        enemyAnimator = this.GetComponentInChildren<Animator>();

        foreach(AnimationClip clip in enemyAnimator.runtimeAnimatorController.animationClips)
        {
            switch(enemyType)
            {
                case EnemyStats.EnemyType.Skull:
                    if(clip.name == "hit") basicAttackDuration = clip.length;

                    break;
                case EnemyStats.EnemyType.Fenrir:
                    break;
                default:
                    break;
            }
        }

        enemyAgent.SetDestination(transform.position);

        enemySpeed = enemyAgent.speed;
        enemyAngularSpeed = enemyAgent.angularSpeed;

        if(enemyType == EnemyStats.EnemyType.Fenrir) baAnimationLength = 3;
    }


    void Update()
    {
        BehaviourUpdate();

        AnimatorUpdate();
    }

    void BehaviourUpdate()
    {
        if(life <= 0 && enemyState != EnemyStates.Dead) SetDead();

        switch(enemyState)
        {
            case EnemyStates.Frozen:
                FronzenUpdate();
                break;
            case EnemyStates.Chase:
                ChaseUpdate();
                break;
            case EnemyStates.Attack:
                AttackUpdate();
                break;
            case EnemyStates.Dead:
                DeadUpdate();
                break;
            default:
                break;
        }
    }

    #region State Updates

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

        if(enemyAgent.remainingDistance > chaseRange && enemyType == EnemyStats.EnemyType.Skull)
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

        if(Vector3.Distance(transform.position, targetTransform.position) > attackRange)
        {
            SetChase();
            InputManager.playerBeingAttacked = true;
        }

        switch(enemyType)
        {
            case EnemyStats.EnemyType.Skull:
                SkullAttacksUpdate();
                break;
            case EnemyStats.EnemyType.Fenrir:
                FenrirAttacksUpdate();
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

    #region State Sets

    void SetChase()
    {
        if(enemyState == EnemyStates.Frozen) isFrozen = false;

        enemyAgent.SetDestination(targetTransform.position);

        enemyAgent.angularSpeed = enemyAngularSpeed;

        if (enemyType == EnemyStats.EnemyType.Skull) isSpinning = false;
        alreadyAttacked = false;
        hasAttacked = false;

        SetSpeed();

        enemyState = EnemyStates.Chase;

        targetBehaviour.SetBeingAttacked(this.gameObject.name, true);
    }

    void SetAttack()
    {
        SetPlayerAttack();

        switch(enemyType)
        {
            case EnemyStats.EnemyType.Skull:
                if(!isSpinning) skullAttack = SkullAttacks.BasicAttack;
                else skullAttack = SkullAttacks.SpinAttack;
                break;
            case EnemyStats.EnemyType.Fenrir:
                basicAttackIndex = Random.Range(0, baAnimationLength - 1);
                baTemporaryIndex = 0;
                break;
            default:
                break;
        }

        enemyState = EnemyStates.Attack;
    }

    void SetDead()
    {
        enemyAgent.isStopped = true;
        enemyAnimator.SetTrigger("die");

        targetBehaviour.SetBasicAttackTransform(null, false);
        targetBehaviour.SetDestination(targetTransform.position);

        targetBehaviour.SetBeingAttacked(this.gameObject.name, false);

        enemyState = EnemyStates.Dead;
    }

    #endregion

    #region Attack Updates

    void SkullAttacksUpdate()
    {
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

    #region Skull Attacks

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
            if(enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.25f && enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.4f)
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
            if(enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.03f)
            {
                alreadyAttacked = false;
            }
        }
    }

    #endregion

    void FenrirAttacksUpdate()
    {
        switch(fenrirAttack)
        {
            case FenrirAttacks.JumpAttack:
                JumpAttackUpdate();
                break;
            case FenrirAttacks.BasicAttack:
                BasicAttackUpdateF();
                break;
            default:
                break;
        }
    }

    #region Fenrir Attacks

    void JumpAttackUpdate()
    {
        return; //Do nothing, de momento
    }

    void BasicAttackUpdateF()
    {
        if(baTemporaryIndex >= 3)
        {
            int baLastIndex = basicAttackIndex;

            baTemporaryIndex = 0;
            basicAttackIndex = Random.Range(0, baAnimationLength - 1);
            while(baLastIndex == basicAttackIndex) basicAttackIndex = Random.Range(0, baAnimationLength - 1);
        }
        if(enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.01) baTemporaryIndex++;

        if(!alreadyAttacked)
        {
            if(hasAttacked)
            {
                targetBehaviour.SetDamage(9);
                alreadyAttacked = true;
                hasAttacked = false;
            }
        }
        else
        {
            if(enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.03f)
            {
                alreadyAttacked = false;
            }
        }
    }

    #endregion

    #endregion

    #region Public Methods and Others

    public void SetLife(float currentLife)
    {
        life = currentLife;
    }

    void SetRotation()
    {
        transform.LookAt(targetTransform);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void SetPlayerAttack()
    {
        bool playerIsAttacking;
        bool playerIsMoving;
        targetBehaviour.IsPlayerAttacking(out playerIsAttacking, out playerIsMoving);

        if(!playerIsAttacking) //&& !playerIsMoving por si solo queremos hacerlo cuando el player este quieto
        {
            targetBehaviour.SetBasicAttackTransform(this.transform, true);
        }

        InputManager.playerBeingAttacked = false;
    }

    public void EnemyHasAttacked()
    {
        if(enemyState == EnemyStates.Attack) hasAttacked = true;
        else hasAttacked = false;
    }

    public void SetSlow(bool slowed)
    {
        isSlowed = slowed;

        SetSpeed();
    }

    void SetSpeed()
    {
        switch(enemyType)
        {
            case EnemyStats.EnemyType.Skull:
                SkullSpeedSet();
                break;
            case EnemyStats.EnemyType.Fenrir:
                FenrirSpeedSet();
                break;
            default:
                break;
        }
    }

    void SkullSpeedSet()
    {
        if(isSpinning) enemyAgent.speed = (isSlowed) ? enemySpeed : enemySpeed * 2;
        else enemyAgent.speed = (isSlowed) ? enemySpeed / 2 : enemySpeed;
    }

    void FenrirSpeedSet()
    {
        enemyAgent.speed = (isSlowed) ? enemySpeed / 2 : enemySpeed;
    }

    #endregion

    #region Animations

    void AnimatorUpdate()
    {
        switch(enemyType)
        {
            case EnemyStats.EnemyType.Skull:
                SkullAnimatorUpdate();
                break;
            case EnemyStats.EnemyType.Fenrir:
                FenrirAnimatorUpdate();
                break;
            default:
                break;
        }
    }

    void SkullAnimatorUpdate()
    {
        if(enemyState == EnemyStates.Chase) isChasing = true;
        else isChasing = false;

        if(isChasing) //!isSpinning para hacerlo solo cuando corra
        {
            enemyAnimator.speed = enemyAgent.velocity.magnitude / enemyAgent.speed;
        }
        else enemyAnimator.speed = 1;

        if(enemyState == EnemyStates.Attack) isAttacking = true;
        else isAttacking = false;

        enemyAnimator.SetBool("isFrozen", isFrozen);
        enemyAnimator.SetBool("isSpinning", isSpinning);
        enemyAnimator.SetBool("isChasing", isChasing);
        enemyAnimator.SetBool("isAttacking", isAttacking);
    }

    void FenrirAnimatorUpdate()
    {
        if(enemyState == EnemyStates.Chase) isChasing = true;
        else isChasing = false;

        if(isChasing)
        {
            enemyAnimator.speed = enemyAgent.velocity.magnitude / enemyAgent.speed;
        }
        else enemyAnimator.speed = 1;

        if(enemyState == EnemyStates.Attack) isAttacking = true;
        else isAttacking = false;

        enemyAnimator.SetBool("isFrozen", isFrozen);
        enemyAnimator.SetBool("isChasing", isChasing);
        enemyAnimator.SetBool("isAttacking", isAttacking);
        enemyAnimator.SetInteger("basicAttackIndex", basicAttackIndex);
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
