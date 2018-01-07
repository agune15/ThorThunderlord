using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycaster : MonoBehaviour {

    //NECESITO HACER RAYCAST DE LA POSICION DEL MOUSE Y DETECTAR LA LAYER.
    //POSICION DEL MOUSE A POSICION DEL MUNDO

    Camera playerCamera;
    public LayerMask layerMask;
    float maxDistance = 100f;

	void Start () {
        playerCamera = this.GetComponent<Camera>();
	}

    public void CastRay ()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if(Physics.Raycast(ray, out hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                //player target pos = hit.transform.gameobject.pos;
                //It has to follow the enemy

                //Player should have a Target, if the ray hits enemy, follow enemy, if it hits the ground, go to the ground point.
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                //player target pos = hit.point;
            }
            
        }
    }
}
