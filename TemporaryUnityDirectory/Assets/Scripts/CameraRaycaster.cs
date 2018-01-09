using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraRaycaster : MonoBehaviour {

    //NECESITO HACER RAYCAST DE LA POSICION DEL MOUSE Y DETECTAR LA LAYER.
    //POSICION DEL MOUSE A POSICION DEL MUNDO

    Camera playerCamera;
    public LayerMask layerMask;
    float maxDistance = 100f;

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

        if(Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))  //, maxDistance, layerMask, QueryTriggerInteraction.Ignore
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
            /*
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("RayArea"))
            {
                enemyWasHit = false;
                enemyTransform = null;

                NavMeshHit navHit;
                if(NavMesh.SamplePosition(hit.point, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    hitPosition = navHit.position;
                }     
            }*/
        }
        else
        {
            enemyWasHit = false;
            enemyTransform = null;

            Vector3 mouseScreenToWorld = playerCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, playerCamera.nearClipPlane));

            //hitPosition = Hold Tight Asznee
        }
    }
}