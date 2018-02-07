using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehaviour : MonoBehaviour {

    [SerializeField] Transform parentBone;
    [SerializeField] CharacterBehaviour playerBehaviour;
    [SerializeField] Transform hammerTransform;


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
        playerBehaviour = this.GetComponentInParent<CharacterBehaviour>();

        hammerTransform = GameObject.Find("J_Axe").GetComponent<Transform>();

        parentBone = GameObject.Find("CC_Hammer").GetComponent<Transform>();
        hammerTransform.parent = parentBone;
    }

    private void Update ()
    {
        ThrowUpdate();
	}

    #region Throw Hammer behaviour

    void ThrowUpdate ()
    {
        if (!isThrowing)
        {
            if (hammerTransform.localPosition != Vector3.zero) hammerTransform.localPosition = Vector3.zero;
            if (hammerTransform.rotation.eulerAngles != Vector3.zero) hammerTransform.localRotation = Quaternion.Euler(Vector3.zero);
            return;
        }
        else
        {
            TimeUpdate();

            currentPosition = new Vector3(Easing.QuartEaseOut(currentTime, startPosition.x, deltaPosition.x, durationTime),
                Easing.QuartEaseOut(currentTime, startPosition.y, deltaPosition.y, durationTime),
                Easing.QuartEaseOut(currentTime, startPosition.z, deltaPosition.z, durationTime));

            hammerTransform.position = currentPosition;
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
                currentTime = 0;
                comeBack = false;
                isThrowing = false;
                HammerIsBack();
            }
        }
    }

    public void ThrowHammer(Vector3 hammerDestination)
    {
        //Vector3 direction = playerBehaviour.HammerDestination();

        hammerTransform.rotation = parentBone.rotation;
        hammerTransform.parent = null;

        startPosition = parentBone.position;
        endPosition = new Vector3(hammerDestination.x, hammerTransform.position.y, hammerDestination.z);

        deltaPosition = endPosition - startPosition;
        currentTime = 0;

        isThrowing = true;
        goForward = true;
    }

    void HammerIsBack()
    {
        isThrowing = false;
        hammerTransform.parent = parentBone;
        hammerTransform.localPosition = Vector3.zero;
        hammerTransform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    #endregion
}
