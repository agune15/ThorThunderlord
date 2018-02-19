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
    [SerializeField] private PauseGameplay pauseGameplay = null;

    [Header("Other Scripts")]
    [SerializeField] private LevelLogic levelLogic;

    [Header("Gamepad")]
    [SerializeField] bool controllerAvailable;
    [SerializeField] bool useController = false;


    void Start()
    {
        levelLogic = GetComponent<LevelLogic>();

        //Debug.Log(Input.GetJoystickNames()[0]);

        if(Input.GetJoystickNames().Length > 0)
        {
            if(Input.GetJoystickNames()[0] != "") controllerAvailable = true;
        }
        else controllerAvailable = false;

        useController = false;

        SetScripts();
    }

    void Update()
    {
        //Player Inputs
        if(player != null)
        {
            if(!useController)
            {
                PlayerInput();
            }
            //else ControllerInputs();
        }

        //Scene Logic
        SceneLogicInput();
    }

    void PlayerInput()
    {
        if (Input.GetButtonDown("Pause")) pauseGameplay.Pause();

        if (!pauseGameplay.isGamePaused)
        {
            playerCamBehaviour.SetMouseWheel(Input.GetAxis("Mouse ScrollWheel"));

            if(CameraBehaviour.playerCanMove)
            {
                if(Input.GetButtonDown("Move") || Input.GetButton("Move")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.Move);
                if(Input.GetButtonDown("Dash")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.Dash);
                if(Input.GetButtonDown("SlowArea")) playerBehaviour.SlowArea();
                if(Input.GetButtonDown("ThrowHammer")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.ThrowHammer);
            }
        }
    }

    /*
    void ControllerInputs()
    {

    }*/

    void SceneLogicInput()
    {
        if(Input.GetKey(KeyCode.AltGr))
        {
            if(Input.GetKeyDown(KeyCode.N)) levelLogic.StartLoad(levelLogic.nextScene);
            if(Input.GetKeyDown(KeyCode.B)) levelLogic.StartLoad(levelLogic.backScene);
            if(Input.GetKeyDown(KeyCode.R)) levelLogic.StartLoad(levelLogic.currentScene);
        }
    }

    #region Public Methods

    public void UseController (bool controllerChosen)
    {
        useController = controllerChosen;
    }

    public void SetScripts ()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            if(player == null)
            {
                player = GameObject.Find("Player");
                playerBehaviour = player.GetComponent<CharacterBehaviour>();
                pauseGameplay = player.GetComponent<PauseGameplay>();
                playerCamBehaviour = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraBehaviour>();
                playerCamRaycaster = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraRaycaster>();
            }
        }
        else
        {
            if(player != null)
            {
                player = null;
                playerBehaviour = null;
                pauseGameplay = null;
                playerCamBehaviour = null;
                playerCamRaycaster = null;
            }
        }
    }

    #endregion
}