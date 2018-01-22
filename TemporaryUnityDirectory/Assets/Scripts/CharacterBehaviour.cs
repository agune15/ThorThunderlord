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
    bool dashAvailable = true;
    float dashCounter = 0;

    Vector3 dashDirection;

    public float dashDuration;
    public float dashCooldown;
    public float dashDistance;
    public float dashImpulse;



    private void Start()
    {
        playerTransform = this.GetComponent<Transform>();
        playerAgent = this.GetComponent<NavMeshAgent>();

        agentSpeed = playerAgent.speed;
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
        if (dashCounter <= dashDuration && playerAgent.remainingDistance > 0)
        {
            dashCounter += Time.deltaTime;
            
            SetRotation();
        }
        else
        {
            //SetDestination(playerTransform.position);

            dashCounter = 0;
            playerAgent.speed = agentSpeed;

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
            isDashing = true;
            dashAvailable = false;

            Vector3 rayDirection = direction - playerTransform.position;
            Ray ray = new Ray(playerTransform.position + new Vector3(0, 1, 0), rayDirection);
            RaycastHit hit;

            LayerMask layerMask = new LayerMask();
            layerMask |= LayerMask.NameToLayer("Ground");
            layerMask |= LayerMask.NameToLayer("Enemy");

            if(Physics.Raycast(ray, out hit, dashDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 hitPosition = hit.point;
                NavMeshHit navHit;

                if(NavMesh.SamplePosition(hitPosition, out navHit, 1.5f, NavMesh.AllAreas))
                {
                    RaycastHit lineHit;

                    if(Physics.Linecast(navHit.position, playerTransform.position + new Vector3(0, 1, 0), out lineHit, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        if(lineHit.collider.gameObject.name == GameObject.Find("Player").name)
                        {
                            dashDirection = navHit.position;
                        }
                    }

                    Debug.DrawLine(ray.origin, navHit.position);
                }

                Debug.DrawLine(ray.origin, hit.point);
            }
            else
            {
                NavMeshHit navHit;

                if(NavMesh.SamplePosition(ray.GetPoint(dashDistance), out navHit, 1.5f, NavMesh.AllAreas))
                {
                    RaycastHit lineHit;

                    if(Physics.Linecast(navHit.position, playerTransform.position + new Vector3(0, 1, 0), out lineHit, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        if (lineHit.collider.gameObject.name == GameObject.Find("Player").name)
                        {
                            dashDirection = navHit.position;
                        }
                    }
                    Debug.DrawLine(ray.origin, navHit.position);
                }
            }

            playerAgent.speed *= dashImpulse;
            playerAgent.isStopped = false;
            playerAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            if(Vector3.Distance(playerTransform.position, dashDirection) <= dashDistance)
            {
                playerAgent.SetDestination(dashDirection);

                Debug.Log("heey");
            }
        }
        else return;
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

    public void SetSteeringTarget (Vector3 targetPos)
    {
        //navAgent.steeringTarget;
        
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
