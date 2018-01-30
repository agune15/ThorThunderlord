using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    enum MoveStates { Idle, Move, Dead }
    MoveStates moveStates = MoveStates.Idle;

    Transform playerTransform;
    NavMeshAgent playerAgent;

    Vector3 targetPos;

    float agentSpeed;

    [Header("Dash Parameters")]
    bool isDashing = false;
    public bool dashAvailable = true;
    
    Vector3 dashEnd;

    public float dashDuration;
    public float dashCooldown;
    public float dashImpulse;
    float dashRemainingDistance;
    public float dashDistance;

    [Header("Slowing Area parameters")]
    public List<Transform> enemyTransformList = new List<Transform>();
    public LayerMask slowAreaLayers;

    Vector3 slowAreaOrigin;

    bool isSlowingArea = false;
    bool slowAreaAvailable = true;

    public float slowAreaRange;
    public float slowAreaDuration;
    public float slowAreaCD;

    float slowAreaCount = 0;
    float slowAreaCurrentRange;

   

    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();

        agentSpeed = playerAgent.speed;
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

        playerAgent.speed = agentSpeed;
        playerAgent.SetDestination(playerTransform.position);

        StartCoroutine(DashCD());
    }

    #endregion

    #region Slow Area behaviour

    public void SlowArea()
    {
        if(slowAreaAvailable)
        {
            isSlowingArea = true;
            slowAreaAvailable = false;

            slowAreaCurrentRange = 0;
            slowAreaCount = 0;

            slowAreaOrigin = playerTransform.position;

            foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) enemyTransformList.Add(enemy.transform);
        }
    }

    void SlowAreaUpdate()
    {
        if(slowAreaCount <= slowAreaDuration)
        {
            slowAreaCount += Time.deltaTime;

            slowAreaCurrentRange = Easing.QuartEaseInOut(slowAreaCount, 0, slowAreaRange, slowAreaDuration);

            if (slowAreaCount >= slowAreaDuration)
            {
                slowAreaCurrentRange = slowAreaRange;
            }
        }
        else
        {
            isSlowingArea = false;

            enemyTransformList.Clear();

            StartCoroutine(SlowAreaCD());
        }

        foreach(Transform enemy in enemyTransformList)
        {
            if(Vector3.Distance(slowAreaOrigin, enemy.position) < slowAreaCurrentRange)
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
        if (!isDashing)
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
            Gizmos.DrawWireSphere(slowAreaOrigin, slowAreaCurrentRange);
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
