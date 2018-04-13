using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDoorBehaviour : MonoBehaviour {

    Animator doorAnimator;
    enum DoorStates { Closed = 0, Opened }
    DoorStates doorState = DoorStates.Closed;

    private void Start()
    {
        doorAnimator = GetComponentInChildren<Animator>();
        doorAnimator.SetInteger("doorStates", (int)doorState);
    }

    public void OpenDoor()
    {
        doorState = DoorStates.Opened;
        doorAnimator.SetInteger("doorStates", (int)doorState);
    }
}
