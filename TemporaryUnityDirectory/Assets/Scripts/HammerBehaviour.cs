using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehaviour : MonoBehaviour {

    public GameObject parentBone;

    Vector3 currentPosition;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    Vector3 deltaPosition;

    float currentTime = 0;
    float durationTime = 0.5f;

    bool isThrowing = false;
    bool goForward = true;
    bool comeBack = false;

    private void Start()
    {
        parentBone = GameObject.Find("CC_Hammer");
        transform.parent = parentBone.transform;
    }

    private void Update ()
    {
        if(!isThrowing)
        {
            if (transform.localPosition != Vector3.zero) transform.localPosition = Vector3.zero;
            if(transform.rotation.eulerAngles != Vector3.zero) transform.localRotation = Quaternion.Euler(Vector3.zero);
            return;
        }
        else
        {
            TimeUpdate();

            currentPosition = new Vector3(Easing.QuadEaseOut(currentTime, startPosition.x, deltaPosition.x, durationTime),
                Easing.QuadEaseOut(currentTime, startPosition.y, deltaPosition.y, durationTime),
                Easing.QuadEaseOut(currentTime, startPosition.z, deltaPosition.z, durationTime));

            transform.position = currentPosition;

            //Debug.Log("currentPosition" + currentPosition);
        }
	}

    void TimeUpdate ()
    {
        if(goForward)
        {
            if(currentTime <= durationTime)
            {
                currentTime += Time.deltaTime;

                if(currentTime >= durationTime)
                {
                    currentTime = durationTime;
                    goForward = false;
                    comeBack = true;
                }
            }
        }
        else if(comeBack)
        {
            currentTime -= Time.deltaTime;

            if(currentTime <= 0)
            {
                comeBack = false;
                isThrowing = false;
                HammerIsBack();
            }
        }
    }

    public void ThrowHammer(Vector3 direction)
    {
        transform.rotation = parentBone.transform.rotation;
        transform.parent = null;

        startPosition = parentBone.transform.position;
        endPosition = new Vector3(direction.x, transform.position.y, direction.z);

        deltaPosition = endPosition - startPosition;
        currentTime = 0;

        isThrowing = true;
        goForward = true;
    }

    void HammerIsBack()
    {
        isThrowing = false;
        transform.parent = parentBone.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
