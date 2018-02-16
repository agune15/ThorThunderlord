using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject startButton;

    public Animator startButtonAnim;

    bool isEasing;
    bool goUp;
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

        goUp = false;
        isEasing = false;
        hasStarted = false;
    }

    // Update is called once per frame
    void Update()

    {
        
        if (startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        else
        {
            if (!hasStarted)
            {
                OnStartOptionsSet();
                hasStarted = !hasStarted;
            }
        }

       
        if (isEasing)
        {
            TimeUpdate();

            EasingUpdate();

            transform.position = new Vector3(transform.position.x, currentValue, transform.position.z);

            if (goUp)
            {
                if (currentTime >= durationTime) EasingEnd();
            }
            else
            {
                if (currentTime <= 0) EasingEnd();
            }
        }

    }

    public void EasingStart()
    {
        goUp = !goUp;
        isEasing = true;

        if (goUp)
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
        if (goUp)
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
        transform.position = (goUp) ? new Vector3(transform.position.x, finalValueY, transform.position.z) :
                                    new Vector3(transform.position.x, iniValueY, transform.position.z);

        OnEndOptionsSet();

        startDelay = startDelayTime;

        isEasing = false;
        hasStarted = false;
    }

    void OnStartOptionsSet()
    {
        if (goUp)
        {
            startButton.SetActive(false);
        }
        else
        {
            menuButtons.SetActive(false);
        }
    }

    void OnEndOptionsSet()
    {
        if (goUp)
        {
            menuButtons.SetActive(true);
        }
        else
        {
            startButton.SetActive(true);
            startButtonAnim.SetTrigger("Close");
        }
    }
}
