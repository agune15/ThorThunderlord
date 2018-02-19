using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraRaycaster : MonoBehaviour {

    public enum RayPorpuse { Move, Dash, ThrowHammer }
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

    void Update()
    {
        if(rayPorpuse == RayPorpuse.Move && enemyWasHit && destination != enemyTransform.position)
        {
            destination = enemyTransform.position;
            playerBehaviour.SetDestination(destination);
        }
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
            case RayPorpuse.ThrowHammer:
                HammerThrowInput();
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

    void HammerThrowInput()
    {
        if(!playerBehaviour.throwAvailable) return;

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

        ThrowHammerUpdate();
    }

    #endregion

    #region Ray Updates

    void MoveUpdate()
    {
        if(enemyWasHit)
        {
            playerBehaviour.SetBasicAttackTransform(enemyTransform, enemyWasHit);

            destination = enemyTransform.position;
            playerBehaviour.SetDestination(destination);
        }
        else
        {
            playerBehaviour.SetBasicAttackTransform(null, enemyWasHit);

            destination = hitPosition;
            playerBehaviour.SetDestination(destination);
        }
    }

    void DashUpdate()
    {
        playerBehaviour.SetBasicAttackTransform(null, false);

        destination = hitPosition;
        playerBehaviour.Dash(destination);
    }

    void ThrowHammerUpdate()
    {
        destination = hitPosition;
        playerBehaviour.ThrowHammer(destination);
    }

    #endregion

    #region Others

    public Vector2 GetCameraPixelSize ()
    {
        return new Vector2(playerCamera.pixelWidth, playerCamera.pixelHeight);
    }

    #endregion
}