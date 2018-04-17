using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTransition : MonoBehaviour
{
    bool isEasing;
    bool goUp;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(startDelay > 0)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        if(isEasing)
        {
            TimeUpdate();

            EasingUpdate();

            transform.position = new Vector3(transform.position.x, currentValue, transform.position.z);

            if(goUp)
            {
                if(currentTime >= durationTime) EasingEnd();
            }
            else
            {
                if(currentTime <= 0) EasingEnd();
            }
        }

    }

    public void EasingStart()
    {
        if (isEasing) return;

        goUp = !goUp;
        isEasing = true;

        if(goUp)
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
        if(goUp)
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

        startDelay = startDelayTime;

        isEasing = false;
    }
}