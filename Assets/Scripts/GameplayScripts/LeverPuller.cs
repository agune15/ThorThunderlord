using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPuller : MonoBehaviour {

    CharacterBehaviour playerBehaviour;
    public LeverDoorBehaviour doorBehaviour;
    
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
        if (!playerBehaviour.isBeingAttacked && leverState == LeverStates.Unused)
        {
            Cursor.SetCursor(CursorManager.GetCursorTexture("interact"), Vector2.zero, CursorMode.Auto);

            if (Input.GetButtonDown("Interact"))
            {
                if (Vector3.Distance(playerBehaviour.transform.position, transform.position) < 10)
                {
                    doorBehaviour.OpenDoor();
                    leverState = LeverStates.Used;
                    leverAnimator.SetInteger("leverState", (int)leverState);
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                }
            }
        }
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
