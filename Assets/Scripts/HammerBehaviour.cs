using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehaviour : MonoBehaviour {

    [SerializeField] Transform parentBone;
    [SerializeField] Transform hammerTransform;


    Vector3 currentPosition;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    Vector3 deltaPosition;

    Vector3 hammerLPosition;
    Quaternion hammerLRotation;

    float currentTime = 0;
    float durationTime = 0.5f;

    bool isThrowing = false;
    bool goForward = true;
    bool comeBack = false;

    //Basic Attack parameters
    bool isAttacking = false;

    private void Start()
    {
        hammerTransform = GameObject.Find("J_Axe").GetComponent<Transform>();
        hammerLPosition = hammerTransform.localPosition;
        hammerLRotation = hammerTransform.localRotation;

        parentBone = GameObject.Find("transform9").GetComponent<Transform>();
        hammerTransform.SetParent(parentBone, true);
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
            if(hammerTransform.localPosition !=  hammerLPosition) hammerTransform.localPosition = hammerLPosition;
            if(hammerTransform.localRotation != hammerLRotation) hammerTransform.localRotation = hammerLRotation;

            /*
            if (hammerTransform.localPosition != Vector3.zero) hammerTransform.localPosition = Vector3.zero;
            if (hammerTransform.rotation.eulerAngles != Vector3.zero) hammerTransform.localRotation = Quaternion.Euler(Vector3.zero);*/
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
        hammerTransform.rotation = hammerTransform.rotation;
        startPosition = hammerTransform.position;

        hammerTransform.SetParent(null, true);

        hammerTransform.position = startPosition;
        endPosition = new Vector3(hammerDestination.x, hammerTransform.position.y, hammerDestination.z);

        deltaPosition = endPosition - startPosition;
        currentTime = 0;

        isThrowing = true;
        goForward = true;
    }

    void HammerIsBack()
    {
        isThrowing = false;
        hammerTransform.SetParent(parentBone, true);
        hammerTransform.localPosition = hammerLPosition;
        hammerTransform.localRotation = hammerLRotation;
    }

    #endregion

    #region Basic Attack behaviour

    public void BasicAttack(bool isAvailable)
    {
        isAttacking = isAvailable;
    }

    public void DealDamage()
    {
        if(isAttacking)
        {
            Debug.Log("daño enemy BasicAttack");
            isAttacking = false;
        }
        else if(isThrowing)
        {
            Debug.Log("daño enemy Throw");
            //damage / 60, ya que va a ser daño por segundo
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawCube(endPosition, new Vector3(0.2f, 0.2f, 0.2f));
    }
}
