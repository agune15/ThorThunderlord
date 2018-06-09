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
    }

    void PlayerInput()
    {
        if (Input.GetButtonDown("Pause")) pauseGameplay.Pause();

        if (!pauseGameplay.isGamePaused)
        {
            playerCamBehaviour.SetMouseWheel(Input.GetAxisRaw("Mouse ScrollWheel"));

            if(CameraBehaviour.playerCanMove)
            {
                if(Input.GetButtonDown("Move") || Input.GetButton("Move")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.Move);
                if(Input.GetButtonDown("Dash")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.Dash);
                if(Input.GetButtonDown("SlowArea")) playerBehaviour.SlowArea();
                if(Input.GetButtonDown("ThrowHammer")) playerCamRaycaster.CastRay(CameraRaycaster.RayPorpuse.ThrowHammer);
                if(Input.GetButtonDown("LightRain")) playerBehaviour.LightRain();
            }
        }
    }

    /*
    void ControllerInputs()
    {

    }*/

    #region Public Methods

    public void UseController (bool controllerChosen)
    {
        useController = controllerChosen;
    }

    public void SetScripts ()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player");
            playerBehaviour = player.GetComponent<CharacterBehaviour>();
            playerCamBehaviour = GameObject.FindGameObjectWithTag("CameraController").GetComponentInChildren<CameraBehaviour>();
            playerCamRaycaster = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CameraRaycaster>();
            pauseGameplay = GameObject.Find("GameplayUI").GetComponent<PauseGameplay>();
        }
        else
        {
            player = null;
            playerBehaviour = null;
            playerCamBehaviour = null;
            playerCamRaycaster = null;
            pauseGameplay = null;
        }
    }

    public void ResetStaticVariables ()
    {

    }

    #endregion
}