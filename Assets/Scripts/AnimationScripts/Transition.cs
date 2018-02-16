using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject BackButton; 
    bool startTransition;

    public float iniValueY;
    public float finalValueY;
    public float currentTime = 0;
    public float durationTime;
    public float startDelay;
    public float startDelayBackGround;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, iniValueY, transform.position.z);
        startTransition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTransition)
        {
            startDelay -= Time.deltaTime;

            if(startDelay > 0) return;

            startDelayBackGround -= Time.deltaTime;

            if(currentTime <= durationTime)
            {
                currentTime += Time.deltaTime;

                float value = Easing.QuadEaseOut(currentTime, iniValueY, finalValueY - iniValueY, durationTime);

                transform.position = new Vector3(transform.position.x, value, transform.position.z);

                if (currentTime > durationTime ) ActiveMenu();
            }
            else
            {
                transform.position = new Vector3(transform.position.x, finalValueY, transform.position.z);

                startTransition = false;
            }
        }
    }

    public void TransitionStart()
    {
        startTransition = true;
    }

    void ActiveMenu()
    {
        MenuPanel.SetActive(true);
        BackButton.SetActive(true);
    }
}