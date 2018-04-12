using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPuller : MonoBehaviour {

    CharacterBehaviour playerBehaviour;
    //public LeverDoorBehaviour doorBehaviour;
    
    enum LeverStates { Unused = 0, Used }
    LeverStates leverState = LeverStates.Unused;
    Animator leverAnimator;

    private void Start()
    {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        leverAnimator = GetComponentInChildren<Animator>();
        leverAnimator.SetInteger("leverState", (int)leverState);
    }

    private void OnMouseOver()
    {
        if (Input.GetButtonDown("Interact") && !playerBehaviour.isBeingAttacked && leverState == LeverStates.Unused)
        {
            if (Vector3.Distance(playerBehaviour.transform.position, transform.position) < 10)
            {
                //doorBehaviour.OpenDoor();
                leverState = LeverStates.Used;
                leverAnimator.SetInteger("leverState", (int)leverState);
            }
        }
    }
}
