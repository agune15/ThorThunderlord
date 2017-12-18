using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

     

    public LayerMask Ground;

    public Interactable focus;

    Camera cam;
    PlayerMotor motor;

	// Use this for initialization
	void Start ()
    {
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (Input.GetMouseButtonDown(1) || (Input.GetMouseButton(1)))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("we hit" + hit.collider.name + " " + hit.point);

                motor.MoveToPoint(hit.point); //Move Our player to what we hit

                RemoveFocus(); //Stop focusing any objects
            }

            
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("we hit" + hit.collider.name + " " + hit.point);

                Interactable interactable = hit.collider.GetComponent<Interactable>(); //Check if we hit an interacteble

                if (interactable != null)
                {
                    SetFocus(interactable);
                }

                //Stop focusing any objects
            }
        }
    }

    // Set our focus to a new focus
    void SetFocus(Interactable newFocus)
    {
        // If our focus has changed
        if (newFocus != focus)
        {
            // Defocus the old one
            if (focus != null)
                focus.OnDefocused();

            
            focus = newFocus;   // Set our new focus
            motor.FollowTarget(newFocus);   // Follow the new focus
        }


        newFocus.OnFocused(transform);
    }

    // Remove our current focus
    void RemoveFocus()
    {
        if (focus != null)
            focus.OnDefocused();


        focus = null;
        motor.StopFollowingTarget();
    }


   
}

