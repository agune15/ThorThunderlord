using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    Transform playerTransform;
    NavMeshAgent playerAgent;
    HammerBehaviour hammerBehaviour;
    Animator thorAnimator;

    //Move parameters
    enum MoveStates { Idle, Move, Dead }
    MoveStates moveStates = MoveStates.Idle;

    enum RotationTypes { Move, Dash, Throw }
    RotationTypes rotationType = RotationTypes.Move;

    Vector3 targetPos;

    float agentSpeed;

    bool canMove = true;

    //Animation related
    [SerializeField] List<AnimationClipName> animationsList = new List<AnimationClipName>();

    //Basic Attack parameters
    [Header("Basic Attack parameters")]
    bool isAttacking = false;
    bool attack = false;

    public float attackRange;

    Transform enemyTargetTransform;

    //Dash parameters
    [Header("Dash parameters")]
    bool isDashing = false;
    public bool dashAvailable = true;

    Vector3 dashEnd;

    public float dashDuration;
    public float dashCooldown;
    public float dashImpulse;
    float dashRemainingDistance;
    public float dashDistance;

    //Slowing Area parameters
    [Header("Slowing Area parameters")]
    public List<Transform> enemyTransformList = new List<Transform>();

    Vector3 slowAreaOrigin;

    bool isSlowingArea = false;
    bool slowAreaAvailable = true;

    public float slowAreaRange;
    public float slowAreaDuration;
    public float slowAreaDelay;
    public float slowAreaFadeTime;
    public float slowAreaCD;

    [SerializeField] float slowAreaCount = 0;

    //Hammer Throw parameters
    [Header("Hammer Throw parameters")]


    bool isThrowing = false;
    bool hasThrown = false;
    public bool throwAvailable = true;

    Vector3 throwDestination;
    Vector3 throwTurnOrigin;

    public float throwCD;
    public float throwDistance;
    float throwTime;
    public float throwDuration;
    public float throwTurnTime;
    public float throwHammerEventTime;


    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();
        hammerBehaviour = this.GetComponentInChildren<HammerBehaviour>();
        thorAnimator = this.GetComponentInChildren<Animator>();

        foreach (AnimationClip animation in thorAnimator.runtimeAnimatorController.animationClips)
        {
            animationsList.Add(new AnimationClipName(animation.name, animation));

            //if(animation.name == "Attack_E_edited") throwDuration = animation.length;
        }

        agentSpeed = playerAgent.speed;

        slowAreaFadeTime += slowAreaDuration;
    }

    private void Update()
    {
        //Estaria bien que se usara solo para el movimiento
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

        //Animations
        thorAnimator.SetBool("isThrowing", isThrowing);
    }

    #region State Updates

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

        if (!isThrowing && !isDashing) SetRotation(RotationTypes.Move);

        //play moving animation

        if (!isDashing) playerAgent.SetDestination(targetPos);
        else
        {
            playerAgent.SetDestination(dashEnd);

            /*
            if(playerAgent.remainingDistance < dashRemainingDistance)
            {
                DisableDash();
            }*/
        }

        if (playerAgent.remainingDistance <= 0)
        {
            DisableDash();
            SetIdle();
        }
    }

    //Chase update to chase enemies?

    void DeadUpdate()
    {
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
        moveStates = MoveStates.Dead;
    }

    #endregion

    #region Basic Attack behaviour

    void BasicAttackUpdate()
    {
        attack = (Vector3.Distance(playerTransform.position, enemyTargetTransform.position) < attackRange) ? true : false;

            /*attack = true;    Ejecuta animacion hammer

            En el script del hammer
            Si el trigger del hammer golpea al enemy, lo daña (y se desactiva por si acasao hasta la siguiente ronda de animacion)
            y desactiva el DamageDeal
            
            Animacion con 1 evento
                1 en el frame 0 que active el DamageDeal al enemigo
            */
        
        if (attack)
        {
            canMove = false;
            playerAgent.isStopped = true;
        }
        else
        {
            canMove = true;
            playerAgent.isStopped = false;
        }
    }

    #endregion

    #region Dash behaviour

    public void Dash(Vector3 destination)
    {
        if(dashAvailable)
        {
            isDashing = true;
            dashAvailable = false;
            canMove = false;

            NavMeshPath path = new NavMeshPath();

            if(NavMesh.CalculatePath(playerTransform.position, destination, NavMesh.AllAreas, path))
            {
                dashEnd = path.corners[1];

                playerAgent.SetDestination(dashEnd);
            }

            dashRemainingDistance = playerAgent.remainingDistance - dashDistance;
            if(dashRemainingDistance < dashDistance) dashRemainingDistance = dashDistance;

            playerAgent.speed *= dashImpulse;

            SetRotation(RotationTypes.Dash);
        }
    }

    void DisableDash()
    {
        isDashing = false;
        canMove = true;

        playerAgent.speed = agentSpeed;
        playerAgent.SetDestination(playerTransform.position);

        StartCoroutine(DashCD());
    }

    #endregion

    #region Slow Area behaviour

    public void SlowArea()
    {
        if(slowAreaAvailable && !isDashing)
        {
            isSlowingArea = true;
            slowAreaAvailable = false;
            
            slowAreaCount = 0;

            slowAreaOrigin = playerTransform.position;

            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if(Vector3.Distance(slowAreaOrigin, enemy.transform.position) < slowAreaRange * 6)
                {
                    enemyTransformList.Add(enemy.transform);
                }
            }
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
            playerAgent.isStopped = false;
            canMove = true;
        }

        if(slowAreaCount <= slowAreaDuration)
        {
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

                enemyTransformList.Clear();

                StartCoroutine(SlowAreaCD());
            }
        }

        foreach(Transform enemy in enemyTransformList)
        {
            if(Vector3.Distance(slowAreaOrigin, enemy.position) < slowAreaRange)
            {
                Debug.Log("eee");

                //Slowear al enemigo y hacerle algo de daño
            }
        }
    }

    #endregion

    #region Throw Hammer behaviour
    
    public void ThrowHammer (Vector3 destination)
    {
        if (throwAvailable)
        {
            isThrowing = true;
            throwAvailable = false;

            Vector3 throwDirection = destination - transform.position;

            Vector3 desiredDestination = transform.position + (throwDirection.normalized * throwDistance);
            throwDestination = desiredDestination;

            throwTurnOrigin = transform.position + (transform.forward * throwDistance);

            throwTime = 0;
        }
    }

    void ThrowUpdate ()
    {
        if (throwTime < throwDuration)
        {
            throwTime += Time.deltaTime;

            SetRotation(RotationTypes.Throw);

            playerAgent.isStopped = true;
            canMove = false;

            if (throwTime > throwHammerEventTime)
            {
                if(hasThrown) return;
                else
                {
                    hammerBehaviour.ThrowHammer(throwDestination);
                    hasThrown = true;
                }
            }
        }
        else
        {
            isThrowing = false;
            playerAgent.isStopped = false;
            canMove = true;
            hasThrown = false;

            StartCoroutine(ThrowHammerCD());
        }
    }

    #endregion

    #region Sets

    public void SetDestination (Vector3 destination)
    {
        if (canMove)
        {
            targetPos = destination;
            playerAgent.SetDestination(targetPos);
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
        Vector3 throwTurnDestination = throwDestination - throwTurnOrigin;

        Vector3 throwLookAt = (throwTime <= throwTurnTime) ? new Vector3(Easing.CubicEaseOut(throwTime, throwTurnOrigin.x, throwTurnDestination.x, throwTurnTime),
                                                            Easing.CubicEaseOut(throwTime, throwTurnOrigin.y, throwTurnDestination.y, throwTurnTime),
                                                            Easing.CubicEaseOut(throwTime, throwTurnOrigin.z, throwTurnDestination.z, throwTurnTime))
                                                            : throwDestination;

        playerTransform.LookAt(throwLookAt);
    }

    #endregion

    public void SetEnemyTransform (Transform enemyTransfrom, bool enemyWasHit)
    {
        enemyTargetTransform = enemyTransfrom;
        isAttacking = enemyWasHit;
    }

    #endregion

    #region Others

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(playerAgent != null)
        {
            Gizmos.DrawCube(playerAgent.steeringTarget, new Vector3(0.5f, 0.5f, 0.5f));
            if (isSlowingArea && slowAreaDelay < 0) Gizmos.DrawWireSphere(slowAreaOrigin, slowAreaRange);
        }
    }

    IEnumerator DashCD()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashAvailable = true;
    }

    IEnumerator SlowAreaCD()
    {
        yield return new WaitForSeconds(slowAreaCD);
        slowAreaAvailable = true;
    }

    IEnumerator ThrowHammerCD()
    {
        yield return new WaitForSeconds(throwCD);
        throwAvailable = true;
    }

    #endregion

    //HACER GODMODE CON playerAgent.Warp()!!!
}

public class AnimationClipName : MonoBehaviour
{
    public string animationName;
    public AnimationClip animationClip;

    public AnimationClipName(string name, AnimationClip clip)
    {
        animationName = name;
        animationClip = clip;
    }
}
