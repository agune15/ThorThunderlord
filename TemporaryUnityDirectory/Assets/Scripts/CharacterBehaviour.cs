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

    [Header("Dash parameters")]
    bool isDashing = false;
    bool dashAvailable = true;
    float dashCounter = 0;

    Vector3 dashDirection;
    Quaternion desiredRotation;

    Vector3 dashBegin;
    Vector3 dashEnd;
    Vector3 dashDelta;

    public float dashDuration;
    public float dashCooldown;
    public float dashDistance;



    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Estaria bien que se usara solo para el movimiento
        if(isDashing)
        {
            DashUpdate();
        }
        else
        {
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
        }

        //Debug.Log("Player state: " + moveStates);
        //Debug.Log("Distance from target: " + playerAgent.remainingDistance);

        if (!playerAgent.isStopped && !isDashing)
        {
            SetRotation();
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
        
        playerAgent.SetDestination(targetPos);

        if (playerAgent.remainingDistance <= 0)
        {
            SetIdle();
        }
    }

    //Chase update to chase enemies?

    void DeadUpdate()
    {
        return;
    }

    void DashUpdate()
    {
        if (dashCounter <= dashDuration)
        {
            dashCounter += Time.deltaTime;

            Vector3 dashEasing = new Vector3(
                Easing.QuadEaseOut(dashCounter, dashBegin.x, dashDelta.x, dashDuration),
                Easing.QuadEaseOut(dashCounter, dashBegin.y, dashDelta.y, dashDuration),
                Easing.QuadEaseOut(dashCounter, dashBegin.z, dashDelta.z, dashDuration));

            //playerAgent.Warp(new Vector3(dashEasing.x, playerTransform.position.y, dashEasing.z));
            playerAgent.Warp(dashEasing);

            //Debug.Log("dashEasing" + dashEasing);
            //Debug.Log("dashDirection" + dashDirection);
        }
        else
        {
            moveStates = MoveStates.Idle;
            isDashing = false;

            StartCoroutine(DashCD());
        }
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

    public void Dash(Vector3 direction)
    {
        if(dashAvailable)
        {
            dashCounter = 0;

            isDashing = true;
            dashAvailable = false;

            /*
            NavMeshPath path = new NavMeshPath();
            playerAgent.CalculatePath(direction, path);

            dashDirection =  path.corners[0];
            */
            
            dashDirection = direction - playerTransform.position;

            desiredRotation = Quaternion.LookRotation(dashDirection);
            playerTransform.rotation = Quaternion.Euler(0, desiredRotation.eulerAngles.y, 0);

            Debug.Log("normalized direction" + dashDirection.normalized);
            
            dashBegin = playerTransform.position;
            dashEnd = dashDirection.normalized * dashDistance;
            dashDelta = dashEnd - dashBegin;
        }
    }

    public void SetDestination (Vector3 destination)
    {
        targetPos = destination;
        playerAgent.SetDestination(targetPos);
    }

    void SetRotation()
    {
        playerTransform.LookAt(playerAgent.steeringTarget);
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(playerAgent.steeringTarget, new Vector3(0.5f, 0.5f, 0.5f));
    }*/

    IEnumerator DashCD ()
    {
        yield return new WaitForSeconds(dashCooldown - dashDuration);
        dashAvailable = true;
    }


    //HACER GODMODE CON playerAgent.Warp()!!!

}
