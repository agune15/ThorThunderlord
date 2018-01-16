using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour {

    enum PlayerStates { Idle, Move, Dead }
    PlayerStates playerStates = PlayerStates.Idle;

    Transform playerTransform;
    NavMeshAgent playerAgent;

    Vector3 targetPos;


    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Estaria bien que se usara solo para el movimiento
        switch(playerStates)
        {
            case PlayerStates.Idle:
                IdleUpdate();
                break;
            case PlayerStates.Move:
                MoveUpdate();
                break;
            case PlayerStates.Dead:
                DeadUpdate();
                break;
            default:
                break;
        }

        //Debug.Log("Player state: " + playerStates);
        //Debug.Log("Distance from target: " + playerAgent.remainingDistance);

        if (playerAgent.isStopped == false)
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

    #endregion

    #region State Sets

    void SetIdle()
    {
        playerStates = PlayerStates.Idle;
    }

    void SetMove()
    {
        playerStates = PlayerStates.Move;
    }

    void SetDead()
    {
        playerStates = PlayerStates.Dead;
    }

    #endregion

    public void SetDestination (Vector3 destination)
    {
        targetPos = destination;
        playerAgent.SetDestination(targetPos);
    }

    void SetRotation()
    {
        playerTransform.LookAt(playerAgent.steeringTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(playerAgent.steeringTarget, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
