using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTransition : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject startButton;

    public Button backButton;
    public Button pressStartButton;

    public Animator startButtonAnim;
    public Animator backButtonAnim;

    AudioPlayer audioPlayer;

    [SerializeField] bool isEasing;
    [SerializeField] bool goDown;
    bool hasStarted;

    float currentValue;
    public float iniValueY;
    public float finalValueY;
    public float currentTime = 0;
    public float durationTime;
    public float startDelay;
    float startDelayTime;

    // Use this for initialization
    void Start()
    {
        currentValue = iniValueY;
        transform.position = new Vector3(transform.position.x, currentValue, transform.position.z);

        startDelayTime = startDelay;

        goDown = false;
        isEasing = false;
        hasStarted = false;

        audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioPlayer>();
    }

    void Update()
    {
        if (!hasStarted)
        {
            BeforeStartOptionsSet();
            hasStarted = !hasStarted;
        }

        if (isEasing)
        {
            TimeUpdate();

            EasingUpdate();

            transform.position = new Vector3(transform.position.x, currentValue, transform.position.z);

            if (goDown)
            {
                if (currentTime >= durationTime) EasingEnd();
            }
            else
            {
                if (currentTime <= 0) EasingEnd();
            }
        }

        SetButtonsInteractionState();
    }

    public void EasingStart()
    {
        if (isEasing) return;

        goDown = !goDown;
        OnStartOptionsSet();

        isEasing = true;

        if (goDown)
        {
            currentValue = iniValueY;
            currentTime = 0;
        }
        else
        {
            currentValue = finalValueY;
            currentTime = durationTime;
        }
    }

    void TimeUpdate()
    {
        if (goDown)
        {
            currentTime += Time.deltaTime;
        }
        else currentTime -= Time.deltaTime;
    }

    void EasingUpdate()
    {
        currentValue = Easing.QuartEaseInOut(currentTime, iniValueY, finalValueY - iniValueY, durationTime);
    }

    void EasingEnd()
    {
        transform.position = (goDown) ? new Vector3(transform.position.x, finalValueY, transform.position.z) :
                                    new Vector3(transform.position.x, iniValueY, transform.position.z);

        OnEndOptionsSet();

        startDelay = startDelayTime;

        isEasing = false;
        hasStarted = false;
    }

    void BeforeStartOptionsSet()
    {
        if (goDown)
        {
            startButton.SetActive(false);
        }
        else
        {
            menuButtons.SetActive(false);
        }
    }

    void OnStartOptionsSet()
    {
        if (goDown)
        {
            pressStartButton.interactable = false;
        }
        else
        {
            backButton.interactable = false;
        }
    }

    void OnEndOptionsSet()
    {
        if (goDown)
        {
            menuButtons.SetActive(true);
            backButton.interactable = true;
        }
        else
        {
            startButton.SetActive(true);
            startButtonAnim.SetTrigger("Close");
            pressStartButton.interactable = true;
        }
    }

    void SetButtonsInteractionState ()   //Sets whether the Press "Key" To Start button is interactable or not
    {
        if (startButton.activeSelf)
        {
            if (startButtonAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenStartButton"))
            {
                if (startButtonAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                {
                    if (pressStartButton.interactable) pressStartButton.interactable = false;
                }
                else pressStartButton.interactable = true;
            }
            else pressStartButton.interactable = false;
        }
        
        if (menuButtons.activeSelf)
        {
            if (backButtonAnim.GetCurrentAnimatorStateInfo(0).IsName("OpenPanels"))
            {
                if (backButtonAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                {
                    if (backButton.interactable) backButton.interactable = false;
                }
                else backButton.interactable = true;
            }
            else backButton.interactable = false;
        }
    }



    public bool IsEasing()
    {
        return isEasing;
    }
}
