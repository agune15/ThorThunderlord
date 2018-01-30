﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraRaycaster : MonoBehaviour {

    public enum RayPorpuse { Move, Dash }
    [HideInInspector] public RayPorpuse rayPorpuse;

    Camera playerCamera;
    public LayerMask moveLayerMask;
    public LayerMask dashLayerMask;
    float maxDistance = 100f;
    float maxRadius = 20f;

    CharacterBehaviour playerBehaviour;
    Vector3 destination;
    Vector3 hitPosition;

    bool enemyWasHit = false;
    Transform enemyTransform = null;

    Vector3 navMeshHitOrigin;

	void Start () {
        playerBehaviour = GameObject.Find("Player").GetComponent<CharacterBehaviour>();
        playerCamera = this.GetComponent<Camera>();

        destination = playerBehaviour.gameObject.transform.position;
	}

    public void CastRay (RayPorpuse rayInput)
    {
        rayPorpuse = rayInput;

        switch(rayPorpuse)
        {
            case RayPorpuse.Move:
                MoveRayInput();
                break;
            case RayPorpuse.Dash:
                DashRayInput();
                break;
            default:
                break;
        }
    }

    #region Ray Inputs

    void MoveRayInput ()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if(Physics.Raycast(ray, out hit, maxDistance, moveLayerMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                enemyWasHit = true;
                enemyTransform = hit.transform;
                hitPosition = Vector3.zero;
            }
            else if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                enemyWasHit = false;
                hitPosition = hit.point;
                enemyTransform = null;
            }
        }
        else
        {
            Vector3 rayPoint = ray.GetPoint(maxDistance / 5);
            NavMeshHit navHit;

            navMeshHitOrigin = rayPoint;

            if(NavMesh.SamplePosition(rayPoint, out navHit, maxRadius, NavMesh.AllAreas))
            {
                enemyWasHit = false;
                enemyTransform = null;
                hitPosition = navHit.position;
            }
        }

        MoveUpdate();
    }

    void DashRayInput ()
    {
        if(!playerBehaviour.dashAvailable) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if(Physics.Raycast(ray, out hit, maxDistance, moveLayerMask, QueryTriggerInteraction.Ignore))
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                enemyWasHit = false;
                hitPosition = hit.point;
                enemyTransform = null;
            }
        }
        else
        {
            Vector3 rayPoint = ray.GetPoint(maxDistance / 5);
            NavMeshHit navHit;

            navMeshHitOrigin = rayPoint;

            if(NavMesh.SamplePosition(rayPoint, out navHit, maxRadius, NavMesh.AllAreas))
            {
                enemyWasHit = false;
                enemyTransform = null;
                hitPosition = navHit.position;
            }
        }

        DashUpdate();
    }

    #endregion

    #region Ray Updates

    void MoveUpdate()
    {
        if(enemyWasHit)
        {
            if(destination != enemyTransform.position)
            {
                destination = enemyTransform.position;
                playerBehaviour.SetDestination(destination);
            }
        }
        else
        {
            if(destination != hitPosition)
            {
                destination = hitPosition;
                playerBehaviour.SetDestination(destination);
            }
        }
    }

    void DashUpdate()
    {
        destination = hitPosition;
        playerBehaviour.Dash(destination);
    }

    #endregion
}