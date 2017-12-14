/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public LayerMask ground;
    public float speed;
    public CharacterController controller;
    private Vector3 position;
    public Physics gravity;

    // Use this for initialization
    void Start ()
    {
        position = transform.position;
        Physics.gravity = new Vector3(0, -100.0F, 0);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButton(1))
        {
            locatePosition(); //Locate where the player clicked on the terrain. 
        }

        moveToPosition(); //Move the player to the position. 


	}

    void locatePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, ground))
        {
            position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            //Debug.Log(position);
        }
    }

    void moveToPosition()   
    { 
        if(Vector3.Distance(transform.position, position)>2)
        {
            Quaternion newRotation = Quaternion.LookRotation(position - transform.position, Vector3.forward);

            newRotation.x = 0f;
            newRotation.z = 0f;

            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 10);
            controller.SimpleMove(transform.forward * speed);
        }
        //Game Object is not moving
        else 
        {

        }    
    }
}*/
