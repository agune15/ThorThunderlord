using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    Transform playerTransform;
    NavMeshAgent playerAgent;
    HammerBehaviour hammerBehaviour;
    BasicAttackTrigger basicAttackTrigger;
    Animator thorAnimator;

    //UI related
    PlayerHealthBar playerHealthBar;

    //Move parameters
    enum MoveStates { Idle, Move, Dead }
    MoveStates moveStates = MoveStates.Idle;

    enum RotationTypes { Move, Dash, Throw, BasicAttack }
    RotationTypes rotationType = RotationTypes.Move;

    Vector3 targetPos;

    float agentSpeed;

    bool canMove = true;

    public float life;
    float maxLife;

    //Animation related
    //[SerializeField] List<AnimationClipName> animationsList = new List<AnimationClipName>();

    //Particle related
    ParticleInstancer particleInstancer;

    //Basic Attack parameters
    [Header("Basic Attack parameters")]
    bool isAttacking = false;
    bool attack = false;
    bool alreadyAttacked = false;

    public float attackRange;
    float attackDuration;
    public float basicAttackDamage;

    public bool isBeingAttacked = false;
    [SerializeField] List<string> enemiesWhoAttacked = new List<string>();

    Transform enemyTargetTransform;
    EnemyStats enemyTargetStats;

    //Dash parameters
    [Header("Dash parameters")]
    bool isDashing = false;
    public bool dashAvailable = true;
    bool hasDashedOut = false;

    Vector3 dashOrigin;
    Vector3 dashEnd;

    public float dashDuration;
    public float dashCooldown;
    public float dashImpulse;
    public float dashDistance;
    float dashCurrentDistance;

    //Slowing Area parameters
    [Header("Slowing Area parameters")]
    public List<EnemyStatsTransform> enemyList = new List<EnemyStatsTransform>();

    Vector3 slowAreaOrigin;

    bool isSlowingArea = false;
    public bool slowAreaAvailable = true;
    bool isCastingArea = false;
    bool castedAreaFX = false;

    public float slowAreaRange;
    public float slowAreaDuration;
    float slowAreaInitDelay;
    [SerializeField] float slowAreaDelay;
    public float slowAreaFadeTime;
    public float slowAreaCD;

    [SerializeField] float slowAreaCount = 0;

    //Hammer Throw parameters
    [Header("Hammer Throw parameters")]
    bool isThrowing = false;
    bool hasThrown = false;
    public bool throwAvailable = true;
    bool throwOnCooldown = false;

    Vector3 throwDestination;
    Vector3 throwTurnOrigin;
    Vector3 throwTurnDestination;

    public float throwHammerDamage;
    public float throwCD;
    float throwCurrentCD;
    public float throwDistance;
    float throwTime;
    float throwDuration;
    public float throwTurnTime;

    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerAgent = GetComponent<NavMeshAgent>();
        hammerBehaviour = GetComponentInChildren<HammerBehaviour>();
        basicAttackTrigger = GetComponentInChildren<BasicAttackTrigger>();
        thorAnimator = GetComponentInChildren<Animator>();

        foreach (AnimationClip animation in thorAnimator.runtimeAnimatorController.animationClips)
        {
            //animationsList.Add(new AnimationClipName(animation.name, animation));

            if (animation.name == "throwHammer") throwDuration = animation.length / 1.4f;
            if (animation.name == "hit_01") attackDuration = animation.length;
            if (animation.name == "slowArea") slowAreaInitDelay = animation.length / 1.3f;
        }

        agentSpeed = playerAgent.speed;

        slowAreaFadeTime += slowAreaDuration;
        slowAreaDelay = slowAreaInitDelay;

        maxLife = life;
        playerHealthBar = GameObject.Find("GameplayUI").GetComponent<PlayerHealthBar>();
        StartCoroutine(SetInitLife());

        particleInstancer = playerHealthBar.gameObject.GetComponent<ParticleInstancer>();
    }

    private void Update()
    {
        if(life <= 0 && moveStates != MoveStates.Dead) SetDead();

        //Move Behaviour
        switch (moveStates)
        {
            case MoveStates.Idle:
                IdleUpdate();
                break;
            case MoveStates.Move:
                MoveUpdate();
                break;
            case MoveStates.Dead:
                DeadUpdate();
                break;
            default:
                break;
        }

        //Basic Attack Update
        if (isAttacking)
        {
            BasicAttackUpdate();
        }

        //Slow Area Update
        if (isSlowingArea)
        {
            SlowAreaUpdate();
        }

        //Throw Hammer Update
        if (isThrowing)
        {
            ThrowUpdate();
        }

        //cooldown Updates
        CooldownUpdate();

        //Animations
        thorAnimator.SetBool("isThrowing", isThrowing);
        thorAnimator.SetBool("isStopped", playerAgent.isStopped);
        thorAnimator.SetBool("isAttacking", isAttacking);
        thorAnimator.SetBool("isCastingArea", isCastingArea);
        thorAnimator.SetBool("isDashing", isDashing);
    }

    #region Movement State Updates

    void IdleUpdate()
    {
        playerAgent.isStopped = true;

        //play idle animation

        if (playerAgent.remainingDistance > 0)
        {
            SetMove();
        }
    }

    void MoveUpdate()
    {
        playerAgent.isStopped = false;

        if (!isDashing)
        {
            SetRotation(RotationTypes.Move);

            if (isAttacking)
            {
                if(playerAgent.destination != enemyTargetTransform.position) playerAgent.SetDestination(enemyTargetTransform.position);
            }         
        }
        else
        {
            SetRotation(RotationTypes.Dash);

            if (Vector3.Distance(dashOrigin, playerTransform.position) / dashCurrentDistance > 0.6f)
            {
                float remainingDistanceFactor = Vector3.Distance(dashOrigin, playerTransform.position) - dashCurrentDistance * 0.6f;
                float maxDistanceFactor = dashCurrentDistance * 0.4f;
                playerAgent.speed = Mathf.SmoothStep(agentSpeed * dashImpulse, 0, remainingDistanceFactor / maxDistanceFactor);

                if (!hasDashedOut)
                {
                    thorAnimator.SetTrigger("dashOut");
                    hasDashedOut = true;
                }
            }

            if (Vector3.Distance(dashOrigin, playerTransform.position) > dashCurrentDistance) DisableDash();
        }

        if (playerAgent.remainingDistance <= 0)
        {
            if (isDashing) DisableDash();
            SetIdle();
        }
    }

    void DeadUpdate()
    {
        if(playerAgent.enabled)
        {
            if(!playerAgent.isStopped)
            {
                playerAgent.isStopped = true;
                playerAgent.enabled = false;
                EnemyStats.SetFrozen();
                GetComponent<CharacterBehaviour>().enabled = false;
            }
        }
        return;
    }

    #endregion

    #region Move State Sets

    void SetIdle()
    {
        moveStates = MoveStates.Idle;
    }

    void SetMove()
    {
        moveStates = MoveStates.Move;
    }

    void SetDead()
    {
        canMove = false;
        playerAgent.isStopped = true;
        CameraBehaviour.playerCanMove = false;

        thorAnimator.SetTrigger("die");
        //PlayingEndMessage.PlayDefeat();
        moveStates = MoveStates.Dead;
    }

    #endregion

    #region Basic Attack behaviour

    void BasicAttackUpdate()
    {
        if(isDashing || isThrowing || isCastingArea || moveStates == MoveStates.Dead) return;

        SetRotation(RotationTypes.BasicAttack);

        if (Vector3.Distance(playerTransform.position, enemyTargetTransform.position) < attackRange)
        {
            if(!attack)
            {
                thorAnimator.SetTrigger("hit");
            }
            attack = true;
        }
        else
        {
            attack = false;
        }

        if(attack)
        {
            canMove = false;
            playerAgent.isStopped = true;

            if(thorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.03f)
            {
                alreadyAttacked = false;
            }

            if(!alreadyAttacked)
            {
                if (thorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 0.35f && thorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.45f)
                {
                    DealBasicAttackDamage();
                }
            }
        }
        else
        {
            canMove = true;
            playerAgent.isStopped = false;

            thorAnimator.ResetTrigger("hit");
            alreadyAttacked = false;
        }
    }

    void DealBasicAttackDamage()
    {
        if (!alreadyAttacked && basicAttackTrigger.TargetIsInRange(enemyTargetStats.name))
        {
            Vector3 directionToTarget = playerTransform.position - enemyTargetTransform.position;
            float desiredAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

            enemyTargetStats.SetDamage(basicAttackDamage, Quaternion.Euler(0, desiredAngle, 0));
            
            alreadyAttacked = true;
        }
    }

    #endregion

    #region Dash behaviour

    public void Dash(Vector3 destination)
    {
        if(dashAvailable && !isThrowing && !isCastingArea && moveStates != MoveStates.Dead)
        {
            isDashing = true;
            dashAvailable = false;
            hasDashedOut = false;
            canMove = false;

            NavMeshPath path = new NavMeshPath();

            if(NavMesh.CalculatePath(playerTransform.position, destination, NavMesh.AllAreas, path))
            {
                dashEnd = path.corners[1];

                playerAgent.SetDestination(dashEnd);
            }

            dashOrigin = playerTransform.position;

            dashCurrentDistance = (Vector3.Distance(dashOrigin, dashEnd) > dashDistance) ? dashDistance : Vector3.Distance(dashOrigin, dashEnd);

            playerAgent.speed *= dashImpulse;

            thorAnimator.SetTrigger("dash");

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.E, 1);
        }
    }

    void DisableDash()
    {
        isDashing = false;
        canMove = true;

        playerAgent.speed = agentSpeed;
        SetDestination(playerTransform.position);

        thorAnimator.ResetTrigger("dash");
        thorAnimator.ResetTrigger("dashOut");

        StartCoroutine(DashCD());
    }

    #endregion

    #region Slow Area behaviour

    public void SlowArea()
    {
        if(slowAreaAvailable && !isDashing && !isThrowing && moveStates != MoveStates.Dead)
        {
            isSlowingArea = true;
            slowAreaAvailable = false;
            isCastingArea = true;
            castedAreaFX = false;
            slowAreaDelay = slowAreaInitDelay;

            thorAnimator.SetTrigger("castArea");
            thorAnimator.ResetTrigger("hit");

            slowAreaCount = 0;

            slowAreaOrigin = playerTransform.position;

            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if(Vector3.Distance(slowAreaOrigin, enemy.transform.position) < slowAreaRange * 8)  //Ajustar!
                {
                    enemyList.Add(new EnemyStatsTransform(enemy.transform, enemy.GetComponent<EnemyStats>()));
                }
            }

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.W, 1);
        }
    }

    void SlowAreaUpdate()
    {
        if(slowAreaDelay > 0)
        {
            slowAreaDelay -= Time.deltaTime;
            playerAgent.isStopped = true;
            canMove = false;
            return;
        }
        else
        {
            if (playerAgent.destination != transform.position) playerAgent.isStopped = false;
            canMove = true;
            if (isCastingArea && isAttacking) thorAnimator.SetTrigger("hit");
            isCastingArea = false;
        }

        if(slowAreaCount <= slowAreaDuration)
        {
            if (!castedAreaFX)
            {
                particleInstancer.InstanciateParticleSystem("Area_Thor", playerTransform.position, Quaternion.identity);
                castedAreaFX = true;
            }

            slowAreaCount += Time.deltaTime;
        }
        else
        {
            if (slowAreaCount <= slowAreaFadeTime)
            {
                slowAreaCount += Time.deltaTime;

                //easing desvanecimiento del area (shader)

            }
            else
            {
                //alpha del shader a 0
                isSlowingArea = false;
                thorAnimator.ResetTrigger("castArea");

                foreach(EnemyStatsTransform enemy in enemyList)
                {
                    if(enemy.stats.isSlowed) enemy.stats.SetInitSpeed();
                }

                enemyList.Clear();

                StartCoroutine(SlowAreaCD());
            }
        }

        foreach(EnemyStatsTransform enemy in enemyList)
        {
            if(Vector3.Distance(slowAreaOrigin, enemy.transform.position) < slowAreaRange)
            {
                enemy.stats.SetDamage(0.02f);
                if (!enemy.stats.isSlowed) enemy.stats.SetSlowSpeed();
            }
            else
            {
                if(enemy.stats.isSlowed) enemy.stats.SetInitSpeed();
            }
        }
    }

    #endregion

    #region Throw Hammer behaviour
    
    public void ThrowHammer (Vector3 destination)
    {
        if (throwAvailable && !isDashing && !isCastingArea && moveStates != MoveStates.Dead)
        {
            isAttacking = false;
            isThrowing = true;
            throwAvailable = false;
            hasThrown = false;

            Vector3 throwDirection = destination - transform.position;

            throwDestination = transform.position + (throwDirection.normalized * throwDistance);
            throwTurnDestination = throwDestination;

            throwTurnOrigin = transform.position + (transform.forward * throwDistance);

            throwTime = 0;

            thorAnimator.SetTrigger("throwHammer");

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.Q, 1);
        }
    }

    public void CatchHammer ()
    {
        thorAnimator.SetTrigger("catchHammer");
    }

    public void HammerWasCaught ()
    {
        thorAnimator.ResetTrigger("throwHammer");
        thorAnimator.ResetTrigger("catchHammer");

        isThrowing = false;
        ThrowHammerCD();
    }

    void ThrowUpdate ()
    {
        if (!hasThrown)
        {
            if (throwTime <= throwDuration)
            {
                throwTime += Time.deltaTime;

                playerAgent.isStopped = true;
                canMove = false;

                SetRotation(RotationTypes.Throw);

                if (throwTime >= throwDuration)
                {
                    if (hasThrown) return;
                    else
                    {
                        hammerBehaviour.ThrowHammer(throwDestination);
                        hasThrown = true;

                        canMove = true;
                    }
                }
            }
        }
    }

    #endregion

    #region Sets

    public void SetDestination (Vector3 destination)
    {
        targetPos = destination;

        if (canMove)
        {
            if(targetPos != transform.position) playerAgent.SetDestination(targetPos);
            else
            {
                playerAgent.SetDestination(targetPos);
                SetIdle();
            }
        }
    }

    void SetRotation(RotationTypes rotationInput)
    {
        rotationType = rotationInput;

        switch(rotationType)
        {
            case RotationTypes.Move:
                MoveRotation();
                break;
            case RotationTypes.Dash:
                DashRotation();
                break;
            case RotationTypes.Throw:
                ThrowRotation();
                break;
            case RotationTypes.BasicAttack:
                BasicAttackRotation();
                break;
            default:
                break;
        }

        playerTransform.rotation = Quaternion.Euler(0, playerTransform.rotation.eulerAngles.y, 0);
    }

    #region Rotation types

    void MoveRotation()
    {
        playerTransform.LookAt(playerAgent.steeringTarget);
    }

    void DashRotation()
    {
        playerTransform.LookAt(dashEnd);
    }

    void ThrowRotation()
    {
        /*Vector3 throwLookAt = (throwTime <= throwTurnTime) ? new Vector3(Easing.CubicEaseOut(throwTime, throwTurnOrigin.x, throwTurnDestination.x, throwTurnTime),
                                                            Easing.CubicEaseOut(throwTime, throwTurnOrigin.y, throwTurnDestination.y, throwTurnTime),
                                                            Easing.CubicEaseOut(throwTime, throwTurnOrigin.z, throwTurnDestination.z, throwTurnTime))
                                                            : throwDestination;*/

        Vector3 throwLookAt = (throwTime <= throwTurnTime) ? Vector3.Slerp(throwTurnOrigin, throwTurnDestination, Mathf.SmoothStep(0, 1, throwTime / throwTurnTime)) : throwDestination;

        playerTransform.LookAt(throwLookAt);
    }

    void BasicAttackRotation()
    {
        playerTransform.LookAt(enemyTargetTransform);
    }

    #endregion

    public void SetBasicAttackTransform (Transform enemyTransform, bool enemyWasHit)
    {
        if(enemyTargetTransform == enemyTransform && attack) return;

        if(isCastingArea || isThrowing || isDashing) return;

        enemyTargetTransform = enemyTransform;
        enemyTargetStats = (enemyTransform != null) ? enemyTargetTransform.GetComponent<EnemyStats>() : null;
        isAttacking = enemyWasHit;

        attack = false;
        thorAnimator.ResetTrigger("hit");

        if (enemyTargetTransform == null && !isAttacking)
        {
            canMove = true;
        }
    }

    public void SetDamage (float damage)
    {
        life -= damage;
        playerHealthBar.SetCurrentPlayerHealth(maxLife, life);

        if (life <= 0 && moveStates != MoveStates.Dead)
        {
            SetDead();
            PlayingEndMessage.PlayVictory();
        }
    }

    //Particle instancing overload
    public void SetDamage (float damage, Quaternion particleAngle)
    {
        particleInstancer.InstanciateParticleSystem("Blood", playerTransform.position + new Vector3(0, 1, 0), particleAngle);

        SetDamage(damage);
    }

    public void SetBeingAttacked(string enemyName, bool enemyIsAttacking)
    {
        if (enemyIsAttacking)
        {
            if (!enemiesWhoAttacked.Contains(enemyName)) enemiesWhoAttacked.Add(enemyName);
            if (isBeingAttacked != enemyIsAttacking) isBeingAttacked = enemyIsAttacking;
        }
        else
        {
            if (enemiesWhoAttacked.Contains(enemyName))
            {
                enemiesWhoAttacked.Remove(enemyName);
            }
        }

        if (enemiesWhoAttacked.Count == 0) isBeingAttacked = false;
    }

    #endregion

    #region Gets

    public void IsPlayerAttacking (out bool playerIsAttacking, out bool playerIsMoving)
    {
        playerIsAttacking = isAttacking;
        playerIsMoving = !playerAgent.isStopped;
    }

    public bool IsPlayerDashing()
    {
        return isDashing;
    }

    //Value storage
    public float GetLife ()
    {
        return life;
    }

    public void GetAbilityCooldowns (out float qCooldown, out float wCooldown, out float eCooldown)
    {
        qCooldown = throwCD;
        wCooldown = slowAreaCD;
        eCooldown = dashCooldown;
    }

    #endregion

    #region Others

    void CooldownUpdate()
    {
        if (throwOnCooldown)
        {
            ThrowHammerCooldownUpdate();
        }
    }

    IEnumerator DashCD()
    {
        playerHealthBar.EmptyGreyIcon(PlayerHealthBar.Icons.E);
        yield return new WaitForSeconds(dashCooldown);
        dashAvailable = true;
    }

    IEnumerator SlowAreaCD()
    {
        playerHealthBar.EmptyGreyIcon(PlayerHealthBar.Icons.W);
        yield return new WaitForSeconds(slowAreaCD);
        slowAreaAvailable = true;
    }

    /*IEnumerator ThrowHammerCD()
    {
        playerHealthBar.EmptyGreyIcon(PlayerHealthBar.Icons.Q);
        yield return new WaitForSeconds(throwCD);
        throwAvailable = true;
    }*/

    IEnumerator SetInitLife()
    {
        yield return new WaitForSeconds(0.1f);
        playerHealthBar.SetCurrentPlayerHealth(maxLife, life);
    }

    void ThrowHammerCD()
    {
        throwOnCooldown = true;
        throwCurrentCD = 0;
        playerHealthBar.EmptyGreyIcon(PlayerHealthBar.Icons.Q);
    }

    void ThrowHammerCooldownUpdate()
    {
        if (throwCurrentCD < throwCD)
        {
            throwCurrentCD += Time.deltaTime;
        }
        else
        {
            throwAvailable = true;
            throwOnCooldown = false;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (playerAgent != null)
        {
            Gizmos.DrawCube(playerAgent.destination, new Vector3(0.5f, 0.5f, 0.5f));
            if (isSlowingArea && slowAreaDelay < 0) Gizmos.DrawWireSphere(slowAreaOrigin, slowAreaRange);
        }
        if (isThrowing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(throwTurnOrigin, new Vector3(0.5f, 0.5f, 0.5f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(throwTurnDestination, new Vector3(0.5f, 0.5f, 0.5f));
        }
        

    }

    #endregion

    //HACER GODMODE CON playerAgent.Warp()!!!
}

[System.Serializable]
public class AnimationClipName 
{
    public string animationName;
    public AnimationClip animationClip;

    public AnimationClipName(string name, AnimationClip clip)
    {
        animationName = name;
        animationClip = clip;
    }
}

[System.Serializable]
public class EnemyStatsTransform
{
    public Transform transform;
    public EnemyStats stats;

    public EnemyStatsTransform(Transform enemyTransform, EnemyStats enemyStats)
    {
        transform = enemyTransform;
        stats = enemyStats;
    }
}