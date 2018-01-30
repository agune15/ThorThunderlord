using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    [Header("GameObjects")]
    [SerializeField] private GameObject player = null;

    [Header("Player Related Scripts")]
    [SerializeField] private CharacterBehaviour playerBehaviour = null;
    [SerializeField] private CameraBehaviour playerCamBehaviour = null;
    [SerializeField] private CameraRaycaster playerCamRaycaster = null;

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
            if (player == null)
            {
                player = GameObject.Find("Player");
                playerBehaviour = player.GetComponent<CharacterBehaviour>();
                playerCamBehaviour = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraBehaviour>();
                playerCamRaycaster = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraRaycaster>();
            }
        }
        else
        {
            if (player != null)
            {
                player = null;
                playerBehaviour = null;
                playerCamBehaviour = null;
                playerCamRaycaster = null;
            }
        }
    }

    void PlayerInput()
    {
        playerCamBehaviour.SetMouseWheel(Input.GetAxis("Mouse ScrollWheel"));

        if(playerCamBehaviour.playerCanMove)
        {
            if(Input.GetButtonDown("Move")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.Move);
            if(Input.GetButtonDown("Dash")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.Dash);
            if(Input.GetButtonDown("SlowArea")) playerBehaviour.SlowArea();
        }
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