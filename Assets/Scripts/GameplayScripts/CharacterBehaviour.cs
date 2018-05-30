using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    Transform playerTransform;
    NavMeshAgent playerAgent;
    HammerBehaviour hammerBehaviour;
    AttackTrigger basicAttackTrigger;
    CameraBehaviour cameraBehaviour;
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

    //Health Bar related
    public float life;
    float maxLife;

    //Animation related
    //[SerializeField] List<AnimationClipName> animationsList = new List<AnimationClipName>();

    //Particle related
    ParticleInstancer particleInstancer;

    //Passive parameters
    [Header("Passive parameters")]
    bool isUsingPassive = false;
    bool passiveLoadable = true;

    float passiveCharge = 0;
    public float passivePerChargePercentage;
    float passiveInitDuration;
    public float passiveUnloadTime;
    public float passiveDuration;
    public float passiveCD;
    public float passiveMultiplierFactor;
    float passiveMultiplier = 1;

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
    public List<EnemyStatsTransform> enemySlowAreaList = new List<EnemyStatsTransform>();

    Vector3 slowAreaOrigin;

    bool isSlowingArea = false;
    public bool slowAreaAvailable = true;
    bool isCastingArea = false;
    bool slowAreaDealedDamage = false;
    bool slowAreaCastedCameraShake = false;

    public float slowAreaDamage;
    public float slowAreaRange;
    float slowAreaInitDelay;
    [SerializeField] float slowAreaDelay;
    public float slowAreaFXTime;    //Tiene que ser mayor que slowAreaInitDelay
    public float slowAreaCD;

    [SerializeField] float slowAreaTimer = 0;

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

    //Lightning Rain parameters
    //[Header("Lightning Rain parameters")]
    bool isLightRaining = false;
    [SerializeField] bool lRainAvailable = true;
    bool isCastingLightRain = false;
    bool hasLightRained = false;
    bool castedHammerLightBolt = false;

    Vector3 lightRainOrigin;

    public float lightRainDamage;
    public float lightRainRange;
    public float lightRainCD;
    float lightRainCastDuration;
    float lightRainCastInitDuration;
    public float lightRainLightningFallTime;    //Time until light rain falls (anim time + x)
    float lightRainTimer;
    

    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerAgent = GetComponent<NavMeshAgent>();
        hammerBehaviour = GetComponentInChildren<HammerBehaviour>();
        basicAttackTrigger = GetComponentInChildren<AttackTrigger>();
        cameraBehaviour = GameObject.FindWithTag("CameraController").GetComponent<CameraBehaviour>();
        thorAnimator = GetComponentInChildren<Animator>();

        foreach (AnimationClip animation in thorAnimator.runtimeAnimatorController.animationClips)
        {
            //animationsList.Add(new AnimationClipName(animation.name, animation));

            if (animation.name == "throwHammer") throwDuration = animation.length / 1.4f;
            if (animation.name == "hit_01") attackDuration = animation.length;
            if (animation.name == "slowArea") slowAreaInitDelay = animation.length / 1.3f;
            if (animation.name == "lightboltRain") lightRainCastInitDuration = animation.length;
        }

        agentSpeed = playerAgent.speed;

        maxLife = life;
        playerHealthBar = GameObject.Find("GameplayUI").GetComponent<PlayerHealthBar>();
        StartCoroutine(SetInitLife());

        passiveInitDuration = passiveDuration;
        
        particleInstancer = GameObject.FindWithTag("ParticleInstancer").GetComponent<ParticleInstancer>();
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

        //Light Rain Area Update
        if (isLightRaining)
        {
            LightRainUpdate();
        }

        //Passive Update
        PassiveUpdate();

        //Cooldown Updates
        CooldownUpdate();

        //Animations
        thorAnimator.SetBool("isThrowing", isThrowing);
        thorAnimator.SetBool("isStopped", playerAgent.isStopped);
        thorAnimator.SetBool("isAttacking", isAttacking);
        thorAnimator.SetBool("isCastingArea", isCastingArea);
        thorAnimator.SetBool("isCastingRain", isCastingLightRain);
        thorAnimator.SetBool("isDashing", isDashing);
        thorAnimator.SetFloat("passiveMultiplier", 1 * passiveMultiplier);
    }

    #region Movement State Updates

    void IdleUpdate()
    {
        playerAgent.isStopped = true;

        if (playerAgent.remainingDistance > 0 || isDashing)
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

            playerAgent.SetDestination(dashEnd);

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

                if (playerAgent.speed <= 0.1f) DisableDash();
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

    #region Passive behaviour
    
    void PassiveUpdate ()
    {
        if (isUsingPassive)
        {
            if (passiveInitDuration >= 0)
            {
                passiveInitDuration -= Time.deltaTime;
            }
            else DisablePassive();
        }
        else
        {
            if (passiveCharge <= 0) return;

            passiveCharge -= 100 * Time.deltaTime / passiveUnloadTime;
            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.Passive, passiveCharge / 100);
        }
    }

    void ChargePassive (float chargeAmount)
    {
        if (passiveLoadable)
        {
            passiveCharge += chargeAmount;
            if (passiveCharge >= 100)
            {
                passiveCharge = 0;

                passiveDuration = passiveInitDuration;
                passiveMultiplier = passiveMultiplierFactor;
                isUsingPassive = true;
                passiveLoadable = false;

                playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.Passive, 1);
                
                return;
            }

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.Passive, passiveCharge / 100);
        }
    }

    void DisablePassive ()
    {
        passiveMultiplier = 1;
        isUsingPassive = false;

        StartCoroutine(PassiveCD());
    }
    
    #endregion

    #region Basic Attack behaviour

    void BasicAttackUpdate()
    {
        if(isDashing || isThrowing || isCastingArea || isCastingLightRain || moveStates == MoveStates.Dead) return;

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

            if(thorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 <= 0.1f)
            {
				if (alreadyAttacked) alreadyAttacked = false;
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
            ChargePassive(passivePerChargePercentage);
            
            alreadyAttacked = true;
        }
    }

    #endregion

    #region Dash behaviour

    public void Dash(Vector3 destination)
    {
        if(dashAvailable && !isThrowing && !isCastingArea && !isCastingLightRain && moveStates != MoveStates.Dead)
        {
            isDashing = true;
            dashAvailable = false;
            hasDashedOut = false;
            canMove = false;

            NavMeshPath path = new NavMeshPath();
            Vector3 lastDashEnd = dashEnd;

            if(NavMesh.CalculatePath(playerTransform.position, destination, NavMesh.AllAreas, path))
            {
                for (int i = 0; i < path.corners.Length; i++)
                {
                    if (Vector3.Distance(path.corners[i], playerTransform.position) > 0 && dashEnd != path.corners[i])
                    {
                        dashEnd = path.corners[i];
                        break;
                    }
                }

                playerAgent.SetDestination(dashEnd);
            }

            if (lastDashEnd == dashEnd) dashEnd = destination;   //Da problemas?

            dashOrigin = playerTransform.position;

            dashCurrentDistance = (Vector3.Distance(dashOrigin, dashEnd) > dashDistance) ? dashDistance : Vector3.Distance(dashOrigin, dashEnd);

            playerAgent.speed *= dashImpulse;

            thorAnimator.SetTrigger("dash");

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.E, 1);
            particleInstancer.InstanciateParticleSystem("Dash", playerTransform, new Vector3(0, 1, 2f), Quaternion.Euler(0, 180, 0));
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

        particleInstancer.DestroyParticleSystem("Dash(Clone)");

        StartCoroutine(DashCD());
    }

    #endregion

    #region Slow Area behaviour

    public void SlowArea()
    {
        if(slowAreaAvailable && !isDashing && !isThrowing && !isCastingLightRain && moveStates != MoveStates.Dead)
        {
            isSlowingArea = true;
            slowAreaAvailable = false;
            isCastingArea = true;
            slowAreaDealedDamage = false;
            slowAreaCastedCameraShake = false;

            thorAnimator.SetTrigger("castArea");
            thorAnimator.ResetTrigger("hit");

            slowAreaDelay = slowAreaInitDelay;
            slowAreaTimer = 0;

            slowAreaOrigin = playerTransform.position;

            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if(Vector3.Distance(slowAreaOrigin, enemy.transform.position) < slowAreaRange * 6)  //Ajustar!
                {
                    enemySlowAreaList.Add(new EnemyStatsTransform(enemy.transform, enemy.GetComponent<EnemyStats>()));
                }
            }

            particleInstancer.InstanciateParticleSystem("Area_W_v2", slowAreaOrigin + new Vector3(0, 0.1f, 0), Quaternion.identity);

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.W, 1);
        }
    }

    void CastSlowAreaUpdate()
    {
        if(slowAreaDelay > 0)
        {
            slowAreaDelay -= Time.deltaTime;
            playerAgent.isStopped = true;
            canMove = false;
        }
        else
        {
            if (playerAgent.destination != transform.position) playerAgent.isStopped = false;
            canMove = true;
            if (isCastingArea && isAttacking) thorAnimator.SetTrigger("hit");
            isCastingArea = false;
        }
    }

    void SlowAreaUpdate()
    {
        CastSlowAreaUpdate();

        if (slowAreaTimer <= slowAreaFXTime)
        {
            slowAreaTimer += Time.deltaTime;

            if (slowAreaTimer >= 0.4f)
            {
                foreach (EnemyStatsTransform enemy in enemySlowAreaList)    //Apply / Remove slow speed on enemies
                {
                    if (Vector3.Distance(enemy.transform.position, slowAreaOrigin) <= slowAreaRange)
                    {
                        if (!enemy.stats.isSlowed) enemy.stats.SetSlowSpeed();
                    }
                    else
                    {
                        if (enemy.stats.isSlowed) enemy.stats.SetInitSpeed();
                    }
                }

                if (slowAreaTimer >= 0.6f)
                {
                    if (!slowAreaDealedDamage)
                    {
                        foreach (EnemyStatsTransform enemy in enemySlowAreaList)    //Apply slowAreaDamage on enemies
                        {
                            if (Vector3.Distance(enemy.transform.position, slowAreaOrigin) <= slowAreaRange) enemy.stats.SetDamage(slowAreaDamage);
                        }
                        slowAreaDealedDamage = true;
                    }

                    if (!slowAreaCastedCameraShake)     //Apply camera shake
                    {
                        cameraBehaviour.CameraShake(6, 0.5f);
                        slowAreaCastedCameraShake = true;
                    }
                }
            }
        }
        else
        {
            isSlowingArea = false;
            thorAnimator.ResetTrigger("castArea");

            foreach (EnemyStatsTransform enemy in enemySlowAreaList)
            {
                if (enemy.stats.isSlowed) enemy.stats.SetInitSpeed();
            }

            enemySlowAreaList.Clear();

            StartCoroutine(SlowAreaCD());
        }
    }

    #endregion

    #region Throw Hammer behaviour
    
    public void ThrowHammer (Vector3 destination)
    {
        if (throwAvailable && !isDashing && !isCastingArea && !isCastingLightRain && moveStates != MoveStates.Dead)
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

    #region Light Rain behaviour

    public void LightRain ()
    {
        if (lRainAvailable && !isDashing && !isCastingArea && !isThrowing && moveStates != MoveStates.Dead)
        {
            lRainAvailable = false;
            isLightRaining = true;
            isCastingLightRain = true;
            hasLightRained = false;
            castedHammerLightBolt = false;

            thorAnimator.SetTrigger("castRain");
            thorAnimator.ResetTrigger("hit");

            lightRainCastDuration = lightRainCastInitDuration;
            lightRainTimer = 0;

            lightRainOrigin = playerTransform.position;

            playerHealthBar.SetIconFillAmount(PlayerHealthBar.Icons.R, 1);
        }
    }

    void LightRainUpdate ()
    {
        if (lightRainCastDuration > 0)
        {
            lightRainCastDuration -= Time.deltaTime;
            playerAgent.isStopped = true;
            canMove = false;

            if (lightRainCastDuration / lightRainCastInitDuration < 0.5f & !castedHammerLightBolt)
            {
                particleInstancer.InstanciateParticleSystem("LightBolt_fromHAMMER", playerTransform, new Vector3(0.25f, 2.75f, 0), Quaternion.identity);
                castedHammerLightBolt = true;
            }

            return;
        }
        else
        {
            if (playerAgent.destination != playerTransform.position) playerAgent.isStopped = false;
            canMove = true;
            if (isCastingArea && isAttacking) thorAnimator.SetTrigger("hit");
            isCastingLightRain = false;
        }

        lightRainTimer += Time.deltaTime;
        
        if (lightRainTimer >= lightRainLightningFallTime)
        {
            if (!hasLightRained)
            {
                //instanciar particulas (deberia ser object pooling)
                for (int i = 0; i < 30; i++)
                {
                    float rangeFromOrigin = 0;
                    float angleFromOrigin = (float)i / 6 * 360 + (int)i / 6 * 30;
                    
                    if (i < 30 / 5) rangeFromOrigin = lightRainRange / 5;
                    else if (i >= 30 / 5 && i < 30 / 5 * 2) rangeFromOrigin = lightRainRange / 5 * 2;
                    else if (i >= 30 / 5 * 2 && i < 30 / 5 * 3) rangeFromOrigin = lightRainRange / 5 * 3;
                    else if (i >= 30 / 5 * 3 && i < 30 / 5 * 4) rangeFromOrigin = lightRainRange / 5 * 4;
                    else if (i >= 30 / 5 * 4) rangeFromOrigin = lightRainRange / 5 * 5;


                    float xPosition = lightRainOrigin.x + rangeFromOrigin * Mathf.Cos(angleFromOrigin * Mathf.Deg2Rad);
                    float zPosition = lightRainOrigin.z + rangeFromOrigin * Mathf.Sin(angleFromOrigin * Mathf.Deg2Rad);

                    Vector3 particlePosition = new Vector3(xPosition, lightRainOrigin.y, zPosition);

                    particleInstancer.PoolParticleSystem("LightBolt_fromSKY_noLight", particlePosition, Quaternion.identity);
                }
                particleInstancer.InstanciateParticleSystem("LightBolt_light", lightRainOrigin, Quaternion.identity);

                cameraBehaviour.CameraShake(7, 1f);
                hasLightRained = true;
            }

            if (lightRainTimer >= lightRainLightningFallTime + 0.1f)
            {
                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))    //Damage to enemies within the lightRainRange
                {
                    if (Vector3.Distance(enemy.transform.position, lightRainOrigin) <= lightRainRange)
                    {
                        enemy.GetComponent<EnemyStats>().SetDamage(lightRainDamage);
                    }
                }

                thorAnimator.ResetTrigger("castRain");

                isLightRaining = false;
                StartCoroutine(LightRainCD());
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

        if(isThrowing || isDashing) return;   //isCastingArea || isCastingLightRain

        enemyTargetTransform = enemyTransform;
        enemyTargetStats = (enemyTransform != null) ? enemyTargetTransform.GetComponent<EnemyStats>() : null;

        //if (!isAttacking && enemyWasHit) particleInstancer.InstanciateParticleSystem("Thor_basicAttack", GameObject.FindWithTag("hammerParent").transform, new Vector3(-0.023f, -0.024f, -0.624f), Quaternion.identity);
        //if (isAttacking && !enemyWasHit) particleInstancer.DestroyParticleSystem("Thor_basicAttack(Clone)");

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

    public void GetAbilityCooldowns (out float passiveCooldown, out float qCooldown, out float wCooldown, out float eCooldown, out float rCooldown)
    {
        passiveCooldown = passiveCD;
        qCooldown = throwCD;
        wCooldown = slowAreaCD;
        eCooldown = dashCooldown;
        rCooldown = lightRainCD;
    }

    public void GetAbilityRanges (out float qRange, out float wRange, out float eRange, out float rRange)
    {
        qRange = throwDistance;
        wRange = slowAreaRange;
        eRange = dashDistance;
        rRange = lightRainRange;
    }

    #endregion

    #region Others

    public IEnumerator HealHealOverTime (int percentage, float time)
    {
        float healTimer = time;
        float healDuration = time;
        float healPercentage = maxLife * ((float)percentage / 100);

        while (healTimer >= 0)
        {
            life += healPercentage * Time.deltaTime / healDuration;
            healTimer -= Time.deltaTime;

            if (life >= maxLife)
            {
                life = maxLife;
                //yield break;    si se quiere salir de la coroutina
            }

            playerHealthBar.SetCurrentPlayerHealth(maxLife, life);

            yield return null;
        }
    }

    void CooldownUpdate()   //Update CDs that aren't coroutines
    {
        if (throwOnCooldown)
        {
            ThrowHammerCooldownUpdate();
        }
    }

    IEnumerator PassiveCD()
    {
        playerHealthBar.EmptyGreyIcon(PlayerHealthBar.Icons.Passive);
        yield return new WaitForSeconds(passiveCD);
        passiveLoadable = true;
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

    IEnumerator LightRainCD()
    {
        playerHealthBar.EmptyGreyIcon(PlayerHealthBar.Icons.R);
        yield return new WaitForSeconds(lightRainCD);
        lRainAvailable = true;
    }

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
            if (isSlowingArea && slowAreaTimer >= 0.4f) Gizmos.DrawWireSphere(slowAreaOrigin, slowAreaRange);
        }
        if (isThrowing)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(throwTurnOrigin, new Vector3(0.5f, 0.5f, 0.5f));
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(throwTurnDestination, new Vector3(0.5f, 0.5f, 0.5f));
        }
        if (isDashing)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(dashEnd, new Vector3(0.5f, 0.5f, 0.5f));
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(playerTransform.position, dashDistance * 2);
        }
    }

    #endregion

    //HACER GODMODE CON playerAgent.Warp()!!!
}

[System.Serializable]
public class AnimationClipName      //Store AnimationName (string) + AnimationClip
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
public class EnemyStatsTransform    //Store Transform + EnemyStats
{
    public Transform transform;
    public EnemyStats stats;

    public EnemyStatsTransform(Transform enemyTransform, EnemyStats enemyStats)
    {
        transform = enemyTransform;
        stats = enemyStats;
    }
}