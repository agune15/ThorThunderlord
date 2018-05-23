using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBehaviour : MonoBehaviour {

    CharacterBehaviour playerBehaviour;

    Transform playerTransform;
    [SerializeField] Transform parentBone;
    [SerializeField] Transform hammerTransform;

    Vector3 currentPosition;
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    Vector3 deltaPosition;

    Vector3 hammerLPosition;
    Quaternion hammerLRotation;

    float forwardTimer = 0;
    float backwardTimer = 0;
    float durationTime = 0.3f;

    float forwardSpeed;
    float backwardSpeed;
    //float backwardMaxSpeed;

    Vector3 rotationVelocity = Vector3.zero;

    bool isThrowing = false;
    bool goForward = true;

    bool hammerCameBack = false;

    float throwHammerDamage;

    private void Start()
    {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerTransform = playerBehaviour.gameObject.GetComponent<Transform>();

        hammerTransform = GameObject.Find("J_Axe").GetComponent<Transform>();
        hammerLPosition = hammerTransform.localPosition;
        hammerLRotation = hammerTransform.localRotation;

        parentBone = GameObject.FindWithTag("hammerParent").GetComponent<Transform>();
        hammerTransform.SetParent(parentBone, true);

        throwHammerDamage = playerBehaviour.throwHammerDamage;

        forwardSpeed = playerBehaviour.throwDistance / durationTime;
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
        }
        else
        {
            if (goForward)
            {
                GoForwardUpdate();
            }
            else
            {
                ComeBackUpdate();
            }
        }
    }

    void GoForwardUpdate ()
    {
        if (forwardTimer <= durationTime)
        {
            forwardTimer += Time.deltaTime;

            if (forwardTimer >= durationTime)
            {
                forwardTimer = durationTime;
                goForward = false;
            }
        }

        //hammerTransform.rotation = Quaternion.LerpUnclamped(hammerTransform.rotation, Quaternion.Euler(0, hammerTransform.rotation.eulerAngles.y, 77), forwardTimer / durationTime/2);
        if (forwardTimer % durationTime < durationTime * 0.75f)
        {
            //Hammer Forward Rotation
            hammerTransform.rotation = Quaternion.RotateTowards(hammerTransform.rotation, Quaternion.Euler(0, hammerTransform.rotation.eulerAngles.y, 77), 5);
        }
        else
        {
            //Hammer Flip Rotation
            hammerTransform.rotation = Quaternion.RotateTowards(hammerTransform.rotation, Quaternion.Euler(180, hammerTransform.rotation.eulerAngles.y, 77), 5);
        }

        //Hammer Position
        currentPosition = new Vector3(Easing.QuartEaseOut(forwardTimer, startPosition.x, deltaPosition.x, durationTime),
            Easing.QuartEaseOut(forwardTimer, startPosition.y, deltaPosition.y, durationTime),
            Easing.QuartEaseOut(forwardTimer, startPosition.z, deltaPosition.z, durationTime));

        hammerTransform.position = currentPosition;
    }

    void ComeBackUpdate ()
    {
        if (backwardTimer <= durationTime)
        {
            backwardTimer += Time.deltaTime;

            backwardSpeed = Easing.QuartEaseIn(backwardTimer, 0, forwardSpeed, durationTime);
        }
        else backwardSpeed = forwardSpeed;

        float distanceFromPlayer = Vector3.Distance(hammerTransform.position, playerTransform.position);

        if (distanceFromPlayer > 4f)
        {
            //Hammer Position coming back
            Vector3 targetPosition = new Vector3(playerTransform.position.x, hammerTransform.position.y, playerTransform.position.z);
            hammerTransform.position = Vector3.MoveTowards(hammerTransform.position, targetPosition, backwardSpeed * Time.deltaTime);

            //Hammer Flip Rotation
            Vector3 directionFromPlayer = hammerTransform.position - playerTransform.position;
            float desiredYAngle = Mathf.Atan2(directionFromPlayer.x, directionFromPlayer.z) * Mathf.Rad2Deg;

            hammerTransform.rotation = Quaternion.SlerpUnclamped(hammerTransform.rotation, Quaternion.Euler(180, desiredYAngle, 77), backwardTimer / 0.3f);
        }
        else if (distanceFromPlayer <= 4f && distanceFromPlayer >= 1f)
        {
            //Hammer Position towards parent transform
            hammerTransform.position = Vector3.MoveTowards(hammerTransform.position, parentBone.position, backwardSpeed * Time.deltaTime);

            if (!hammerCameBack)
            {
                playerBehaviour.CatchHammer();
                hammerCameBack = true;
                StartCoroutine(HammerBackAssurance(0.3f));
            }
            else
            {
                //Hammer Hand Grab Rotation
                Vector3 directionFromPlayer = hammerTransform.position - playerTransform.position;
                float desiredYAngle = Mathf.Atan2(directionFromPlayer.x, directionFromPlayer.z) * Mathf.Rad2Deg;

                Vector3 initRotation = hammerTransform.rotation.eulerAngles;
                hammerTransform.rotation = Quaternion.RotateTowards(hammerTransform.rotation, parentBone.rotation * Quaternion.Inverse(hammerLRotation), 20);

                if (Vector3.Distance(hammerTransform.position, parentBone.position) < 0.25f) HammerIsBack();
            }
        }
        else
        {
            HammerIsBack();
        }
    }

    void HammerIsBack()
    {
        if (!isThrowing && !hammerCameBack) return;
        playerBehaviour.HammerWasCaught();

        isThrowing = false;
        hammerCameBack = false;

        hammerTransform.SetParent(parentBone, true);
        hammerTransform.localPosition = hammerLPosition;
        hammerTransform.localRotation = hammerLRotation;
    }

    public void ThrowHammer(Vector3 hammerDestination)
    {
        hammerTransform.rotation = hammerTransform.rotation;
        startPosition = hammerTransform.position;

        hammerTransform.SetParent(null, true);

        hammerTransform.position = startPosition;
        endPosition = new Vector3(hammerDestination.x, hammerTransform.position.y, hammerDestination.z);

        deltaPosition = endPosition - startPosition;

        forwardTimer = 0;
        backwardTimer = 0;

        isThrowing = true;
        goForward = true;
    }

    IEnumerator HammerBackAssurance(float cdTime)
    {
        if (isThrowing)
        {
            yield return new WaitForSeconds(cdTime);
            HammerIsBack();
        }
        else yield return null;
    }

    #endregion

    #region Deal Damage behaviour

    public float ThrowHammerDamage()
    {
        float damageToDeal;

        if(isThrowing)
        {
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
