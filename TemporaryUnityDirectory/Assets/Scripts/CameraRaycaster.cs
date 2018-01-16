using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraRaycaster : MonoBehaviour {

    Camera playerCamera;
    public LayerMask layerMask;
    float maxDistance = 100f;
    float maxRadius = 200f;

    CharacterBehaviour playerBehaviour;
    Vector3 destination;
    Vector3 hitPosition;

    bool enemyWasHit = false;
    Transform enemyTransform = null;

	void Start () {
        playerBehaviour = GameObject.Find("Player").GetComponent<CharacterBehaviour>();
        playerCamera = this.GetComponent<Camera>();

        destination = playerBehaviour.gameObject.transform.position;
	}

    private void Update()
    {
        if (enemyWasHit)
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

    public void CastRay()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if(Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.Log(hit.transform.name);

                enemyWasHit = true;
                enemyTransform = hit.transform;
                hitPosition = Vector3.zero;
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log(hit.transform.name);

                enemyWasHit = false;
                hitPosition = hit.point;
                enemyTransform = null;
            }
            else
            {
                enemyWasHit = false;
                enemyTransform = null;
            }

            Debug.DrawRay(ray.origin, ray.direction, Color.red);
        }
        else
        {
            Ray alternativeRay = playerCamera.ScreenPointToRay(Input.mousePosition);

            Ray alternativePosition = new Ray(transform.position, playerCamera.ScreenToWorldPoint(Input.mousePosition));

            if (Physics.Raycast(alternativePosition, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 rayPoint = alternativePosition.GetPoint(maxDistance);
                rayPoint.y = playerBehaviour.transform.position.y;
                NavMeshHit navHit;

                if (NavMesh.SamplePosition(rayPoint, out navHit, maxRadius, NavMesh.AllAreas))
                {
                    hitPosition = navHit.position;
                }

                Debug.DrawRay(alternativeRay.origin, ray.direction, Color.red);
            }
        }
    }
}