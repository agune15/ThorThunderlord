using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayEnding : MonoBehaviour {

    public enum EndingTypes { Victory, Defeat }
    EndingTypes endingType;

    CameraBehaviour cameraBehaviour;
    TimeManager timeManager;
    ChangeScene sceneChanger;
    public AudioPlayer endingAudioPlayer;
    MusicAmbientController musicAmbientController;

    public GameObject endingCanvas;
    public Text endMessage;

    public Animator[] endingAnimators;
    public EventTrigger[] uiEventTriggers;
    public Image[] uiPopUps;

    float delayTime = 0;
    float changeSceneDelayTime = 0;

    bool playGameEnding = false;
    bool hasPlayedMessage = false;
    bool buttonHasBeenClicked = false;

    int nextScene;


    private void Start()
    {
        cameraBehaviour = GameObject.FindWithTag("CameraController").GetComponent<CameraBehaviour>();
        timeManager = GameObject.FindWithTag("manager").GetComponent<TimeManager>();
        sceneChanger = GameObject.FindWithTag("GameplayUI").GetComponent<ChangeScene>();
        musicAmbientController = GameObject.FindWithTag("MusicAmbientController").GetComponent<MusicAmbientController>();

        //Seteadas des del editor
        /*endingCanvas = GameObject.Find("Ending");
        endMessage = GameObject.Find("EndMessage").GetComponent<Text>();

        endingAnimators[0] = GameObject.Find("Credits").GetComponent<Animator>();
        endingAnimators[1] = GameObject.Find("ExitToMainMenu").GetComponent<Animator>();
        endingAnimators[2] = GameObject.Find("EndMessage").GetComponent<Animator>();
        endingAnimators[3] = GameObject.Find("BottomBG").GetComponent<Animator>();
        endingAnimators[4] = GameObject.Find("TopBG").GetComponent<Animator>();*/
    }

    void Update () {
        if (playGameEnding)
        {
            if (delayTime >= 0)
            {
                delayTime -= Time.unscaledDeltaTime;
                return;
            }

            if (!hasPlayedMessage)
            {
                PlayEndingMessage();
                hasPlayedMessage = true;
            }

            if (buttonHasBeenClicked)
            {
                if (changeSceneDelayTime >= 0)
                {
                    changeSceneDelayTime -= Time.unscaledDeltaTime;
                    return;
                }
                else
                {
                    sceneChanger.SetScene(nextScene);
                    buttonHasBeenClicked = false;
                }
            }
        }
	}

    public void PlayGameEnding (EndingTypes ending, Transform targetTransitionTransform, float cameraTransitionTime, float timeScale, Vector3 cameraEndPosition)
    {
        delayTime = cameraTransitionTime;

        endingType = ending;

        playGameEnding = true;

        cameraBehaviour.CameraEndTransition(targetTransitionTransform, cameraTransitionTime, cameraEndPosition);
        timeManager.SetTimeScaleAndDuration(timeScale, cameraTransitionTime, TimeManager.ScaleTimeTypes.Flat);

        switch (endingType)
        {
            case EndingTypes.Victory:
                //musicAmbientController.SetMusicType(MusicAmbientController.MusicTypes.Victory, 0, 0);
                break;
            case EndingTypes.Defeat:
                //musicAmbientController.SetMusicType(MusicAmbientController.MusicTypes.Defeat, 0, 0);
                break;
            default:
                break;
        }

        //if (endingType == EndingTypes.Defeat) screen en blanco i negro (afectara al texto??)
    }

    void PlayEndingMessage ()
    {
        endingCanvas.SetActive(true);

        foreach (EventTrigger eventTrigger in uiEventTriggers)
        {
            if (eventTrigger.enabled) eventTrigger.enabled = false;
        }
        foreach (Image popUpImage in uiPopUps)
        {
            if (popUpImage.gameObject.activeInHierarchy) popUpImage.enabled = false;
        }
        if (GameObject.FindWithTag("AOEIndicator") != null) GameObject.FindWithTag("AOEIndicator").SetActive(false);

        switch (endingType)
        {
            case EndingTypes.Victory:
                endMessage.text = "VICTORY";
                endingAudioPlayer.Play2DSFX(0, 0.5f, 1f);
                endMessage.color = new Color(0, 0.95f, 0.13f, 1);
                
                break;
            case EndingTypes.Defeat:
                endMessage.text = "DEFEAT";
                endingAudioPlayer.Play2DSFX(1, 0.5f, 1f);
                endMessage.color = new Color(0.95f, 0, 0, 1);

                break;
            default:
                break;
        }
    }

    public void PlayOnMouseClicked (int sceneToGoTo)
    {
        foreach(Animator animator in endingAnimators)
        {
            animator.SetTrigger("buttonClicked");
        }

        changeSceneDelayTime = 1f;
        buttonHasBeenClicked = true;
        nextScene = sceneToGoTo;
    }

    public bool IsGameEnding ()
    {
        return playGameEnding;
    }
}
