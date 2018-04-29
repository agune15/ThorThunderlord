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

    float throwHammerDamage;

    private void Start()
    {
        hammerTransform = GameObject.Find("J_Axe").GetComponent<Transform>();
        hammerLPosition = hammerTransform.localPosition;
        hammerLRotation = hammerTransform.localRotation;

        parentBone = GameObject.FindWithTag("hammerParent").GetComponent<Transform>();
        hammerTransform.SetParent(parentBone, true);

        throwHammerDamage = GetComponentInParent<CharacterBehaviour>().throwHammerDamage;
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
        currentTime = 0;
        comeBack = false;
        isThrowing = false;
        hammerTransform.SetParent(parentBone, true);
        hammerTransform.localPosition = hammerLPosition;
        hammerTransform.localRotation = hammerLRotation;
    }

    #endregion

    #region Basic Attack behaviour

    public float ThrowHammerDamage()
    {
        float damageToDeal;

        if(isThrowing)
        {
            Debug.Log("daño enemy Throw");
            damageToDeal = throwHammerDamage / 10;

            return damageToDeal;
        }
        else return 0;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawCube(endPosition, new Vector3(0.2f, 0.2f, 0.2f));
    }
}
