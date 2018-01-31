using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    Transform playerTransform;
    NavMeshAgent playerAgent;

    //Move parameters
    enum MoveStates { Idle, Move, Dead }
    MoveStates moveStates = MoveStates.Idle;

    Vector3 targetPos;

    float agentSpeed;

    bool canMove = true;

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

   

    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();

        agentSpeed = playerAgent.speed;

        slowAreaFadeTime += slowAreaDuration;
    }

    private void Update()
    {
        //Estaria bien que se usara solo para el movimiento
        switch(moveStates)
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

        //Rotation Update
        if (!playerAgent.isStopped)
        {
            SetRotation();
        }

        //Slow Area Update
        if (isSlowingArea)
        {
            SlowAreaUpdate();
        }
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

        //play moving animation

        if(!isDashing) playerAgent.SetDestination(targetPos);
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

            SetRotation();

            playerAgent.speed *= dashImpulse;
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

    #region Sets

    public void SetDestination (Vector3 destination)
    {
        if (canMove)
        {
            targetPos = destination;
            playerAgent.SetDestination(targetPos);
        }
    }

    void SetRotation()
    {
        playerTransform.LookAt(playerAgent.steeringTarget);
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

    #endregion

    //HACER GODMODE CON playerAgent.Warp()!!!
}
