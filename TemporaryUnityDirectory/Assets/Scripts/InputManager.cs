using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    [Header("GameObjects")]
    [SerializeField] private GameObject player = null;
    
    [Header("Player Related Scripts")]
    [SerializeField] private CameraBehaviour playerCamBehaviour = null;

    [Header("Other Scripts")]
    [SerializeField] private LevelLogic levelLogic;

    //float mouseWheelAxis;

    //[SerializeField] private bool aim = false;    //Ejemplo

    void Start()
    {
        levelLogic = GetComponent<LevelLogic>();
    }

    void Update()
    {
        //Player Inputs
        if(player != null)
        {
            PlayerInput();
        }

        //Scene Logic
        SceneLogicInput();

        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            if (player == null) player = GameObject.Find("Player");
            if (playerCamBehaviour == null) playerCamBehaviour = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraBehaviour>();
            
            //switchCamera = player.GetComponent<SwitchCamera>();   //Ejemplo
        }
        else
        {
            if (player != null) player = null;
            if (playerCamBehaviour != null) playerCamBehaviour = null;
        }
    }

    void PlayerInput()
    {
        playerCamBehaviour.SetMouseWheel(Input.GetAxis("Mouse ScrollWheel"));
    }

    void SceneLogicInput()
    {
        if(Input.GetKey(KeyCode.AltGr))
        {
            if(Input.GetKeyDown(KeyCode.N)) levelLogic.StartLoad(levelLogic.nextScene);
            if(Input.GetKeyDown(KeyCode.B)) levelLogic.StartLoad(levelLogic.backScene);
            if(Input.GetKeyDown(KeyCode.R)) levelLogic.StartLoad(levelLogic.currentScene);
        }
    }
}