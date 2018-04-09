using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenDoorBehaviour : MonoBehaviour {
    
    enum WoodenDoorStates { Closed = 0, OpenFront, OpenBack }
    WoodenDoorStates woodenDoorStates = WoodenDoorStates.Closed;

    Transform playerTransform;
    Transform doorTransform;

    Animator doorAnimator;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        doorTransform = this.transform;

        doorAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        doorAnimator.SetInteger("doorStates", (int)woodenDoorStates);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 targetDirection = playerTransform.position - doorTransform.position;

            if (woodenDoorStates == WoodenDoorStates.Closed) woodenDoorStates = (Vector3.Dot(doorTransform.forward, targetDirection) > 0) ? WoodenDoorStates.OpenBack : WoodenDoorStates.OpenFront;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (woodenDoorStates == WoodenDoorStates.OpenFront || woodenDoorStates == WoodenDoorStates.OpenBack) woodenDoorStates = WoodenDoorStates.Closed;
        }
    }
}
