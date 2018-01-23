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

    [Header("Dash parameters")]
    bool isDashing = false;
    public bool dashAvailable = true;
    
    Vector3 dashEnd;

    public float dashDuration;
    public float dashCooldown;
    public float dashImpulse;
    float dashRemainingDistance;
    public float dashDistance;


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

        //Debug.Log("Player state: " + moveStates);
        //Debug.Log("Distance from target: " + playerAgent.remainingDistance);

        if (!playerAgent.isStopped)
        {
            SetRotation();
        }

        Debug.Log(playerAgent.remainingDistance);
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
            SetIdle();
            DisableDash();
        }
    }

    //Chase update to chase enemies?

    void DeadUpdate()
    {
        return;
    }

    #endregion

    #region State Sets

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

    public void Dash(Vector3 destination)
    {
        if(!dashAvailable) return;
        else
        {
            Debug.Log("daash!");

            isDashing = true;
            dashAvailable = false;
        
            moveStates = MoveStates.Move;

            NavMeshPath path = new NavMeshPath();

            if(NavMesh.CalculatePath(playerTransform.position, destination, NavMesh.AllAreas, path))
            {
                dashEnd = path.corners[1];

                SetDestination(dashEnd);
            }

            dashRemainingDistance = playerAgent.remainingDistance - dashDistance;
            if(dashRemainingDistance < dashDistance) dashRemainingDistance = dashDistance;

            SetRotation();

            playerAgent.speed *= dashImpulse;
        }
    }

    void DisableDash()
    {
        playerAgent.speed = agentSpeed;
        playerAgent.SetDestination(playerTransform.position);

        isDashing = false;
        StartCoroutine(DashCD());
    }

    public void SetDestination (Vector3 destination)
    {
        if (!isDashing)
        {
            Debug.Log("heeey ddoug");

            targetPos = destination;
            playerAgent.SetDestination(targetPos);
        }
    }

    void SetRotation()
    {
        playerTransform.LookAt(playerAgent.steeringTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (playerAgent != null) Gizmos.DrawCube(playerAgent.steeringTarget, new Vector3(0.5f, 0.5f, 0.5f));
    }

    IEnumerator DashCD ()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashAvailable = true;
    }
    
    //HACER GODMODE CON playerAgent.Warp()!!!
}
