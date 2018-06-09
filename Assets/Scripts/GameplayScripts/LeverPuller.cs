using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPuller : MonoBehaviour {

    CharacterBehaviour playerBehaviour;
    public LeverDoorBehaviour doorBehaviour;
    PauseGameplay pauseGameplay;
    
    enum LeverStates { Unused = 0, Used }
    LeverStates leverState = LeverStates.Unused;
    Animator leverAnimator;

    //Shader related
    [Header("Shader related parameters")]
    [SerializeField] Renderer leverRenderer;

    [SerializeField] Shader outlineShader;
    [SerializeField] Shader standardShader;

    private void Start()
    {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        leverAnimator = GetComponentInChildren<Animator>();
        leverAnimator.SetInteger("leverState", (int)leverState);
        pauseGameplay = GameObject.FindWithTag("GameplayUI").GetComponent<PauseGameplay>();
 
        outlineShader = GetComponentInChildren<Renderer>().material.shader;
        standardShader = Shader.Find("Standard");
    }

    private void OnMouseOver()
    {
        if (pauseGameplay.isGamePaused || leverState == LeverStates.Used) return;

        if (!playerBehaviour.isBeingAttacked && leverState == LeverStates.Unused)
        {
            if (Vector3.Distance(playerBehaviour.transform.position, transform.position) < 7)
            {
                if (CursorManager.GetCurrentCursorTextureName() != "interact")
                {
                    CursorManager.SetAndStoreCursor("interact", Vector2.zero, CursorMode.Auto);
                }

                if (Input.GetButtonDown("Interact"))
                {

                    doorBehaviour.OpenDoor();
                    leverState = LeverStates.Used;
                    leverAnimator.SetInteger("leverState", (int)leverState);
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

                    foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                    {
                        renderer.material.shader = standardShader;
                    }
                }
            }
        }
    }

    private void OnMouseExit()
    {
        CursorManager.SetAndStoreCursor("default", Vector2.zero, CursorMode.Auto);
    }
}
