using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    enum MoveStates { Idle, Move, Dead }
    MoveStates moveStates = MoveStates.Idle;

    Transform playerTransform;
    NavMeshAgent playerAgent;
    Rigidbody playerRB;

    Vector3 targetPos;

    [Header("Dash parameters")]
    bool isDashing = false;
    bool dashAvailable = true;
    public float dashTime;
    public float dashCooldown;
    public float dashImpulse;
    Vector3 dashDirection;

    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();
        playerRB = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Estaria bien que se usara solo para el movimiento
        if(!isDashing)
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

            Debug.Log("Player state: " + moveStates);
        //Debug.Log("Distance from target: " + playerAgent.remainingDistance);

        if (!playerAgent.isStopped && !isDashing)
        {
            SetRotation();
        }
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            DashUpdate();
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
        playerRB.WakeUp();
        playerRB.isKinematic = false;

        Vector3 desiredDirection = dashDirection - playerTransform.transform.position;

        Quaternion rotation = Quaternion.LookRotation(desiredDirection);
        playerTransform.rotation = rotation;

        //playerRB.AddForce(playerTransform.forward * dashImpulse);
        playerRB.velocity = playerTransform.forward * dashImpulse;

        playerRB.interpolation = RigidbodyInterpolation.Extrapolate;


        dashAvailable = false;
        isDashing = false;

        StartCoroutine(DashCD());
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
            dashDirection = direction;
            isDashing = true;
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
        yield return new WaitForSeconds(dashTime);
        //isDashing = false;
        //playerAgent.SetDestination(playerTransform.transform.position);
        moveStates = MoveStates.Idle;

        playerRB.isKinematic = true;
        playerRB.Sleep();


        yield return new WaitForSeconds(dashCooldown);
        dashAvailable = true;
    }
}
