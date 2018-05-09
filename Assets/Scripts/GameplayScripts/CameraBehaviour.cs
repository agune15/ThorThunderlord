using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    Transform playerTransform;
    Transform cameraTransform;

    Vector3 startPos;
    Vector3 relativePos;
    public Vector3 offsetPos;
    Vector3 velocity = Vector3.zero;

    public float heightOffset;

    public static bool playerCanMove;

    float wheelAxis = 0;
    
    float yZoomAddition;
    float zZoomAddition;

    float zoomMinLimit = 9f;
    float zoomMaxLimit = 0;

    Vector3 minOffset;
    Vector3 maxOffset;

    float wheelAxisStorage = 0;
    float lastWheelAxisStorage;

    public float smoothTime;
    float timeCounter = 0;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cameraTransform = this.transform;

        startPos = cameraTransform.position;
        relativePos = playerTransform.position + offsetPos;

        playerCanMove = false;

        bool yOffsetIsBigger = (Mathf.Abs(offsetPos.y) > Mathf.Abs(offsetPos.z)) ? true : false;
        float smallAdditionPercentage;

        if (yOffsetIsBigger)
        {
            smallAdditionPercentage = Mathf.Abs(offsetPos.z) / Mathf.Abs(offsetPos.y);
            yZoomAddition = offsetPos.y / 10;
            zZoomAddition = -Mathf.Abs(yZoomAddition) * smallAdditionPercentage;
        }
        else
        {
            smallAdditionPercentage = Mathf.Abs(offsetPos.y) / Mathf.Abs(offsetPos.z);
            zZoomAddition = offsetPos.z / 10;
            yZoomAddition = Mathf.Abs(zZoomAddition) * smallAdditionPercentage;
        }

        minOffset = new Vector3(0, yZoomAddition * 2, zZoomAddition * 2);
        maxOffset = offsetPos;

        Debug.Log("minOffset " + minOffset);
        Debug.Log("maxOffset " + maxOffset);
    }

    private void LateUpdate()
    {
        if(!playerTransform) return;

        Debug.Log("wheelAxisStorage " + wheelAxisStorage);

        //Initial Transition
        if(timeCounter <= smoothTime)
        {
            timeCounter += Time.deltaTime;

            //Camera Offset Position
            relativePos = playerTransform.position + offsetPos + new Vector3(0, heightOffset, 0);

            //Camera Start Position Set
            cameraTransform.position = Vector3.Lerp(startPos, relativePos, Mathf.SmoothStep(0, 1, timeCounter / smoothTime));

            //Camera Start Rotation Set
            Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - cameraTransform.position + new Vector3(0, heightOffset, 0));

            cameraTransform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, 0, 0);
        }
        else
        {
            if (smoothTime != 0.25f) smoothTime = 0.25f;
            if (!playerCanMove) playerCanMove = true;   //Permitir mover el player

            //Camera Zoom
            if (wheelAxisStorage <= zoomMinLimit && wheelAxisStorage >= zoomMaxLimit && lastWheelAxisStorage != wheelAxisStorage)
            {
                offsetPos.y -= wheelAxis * yZoomAddition;
                offsetPos.z -= wheelAxis * zZoomAddition;

                if (offsetPos.sqrMagnitude < minOffset.sqrMagnitude)
                {
                    offsetPos = minOffset;
                }
                else if (offsetPos.sqrMagnitude > maxOffset.sqrMagnitude)
                {
                    offsetPos = maxOffset;
                }
            }

            //Camera Offset Position
            relativePos = playerTransform.position + offsetPos + new Vector3(0, heightOffset, 0);

            //Camera Position Set
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, relativePos, ref velocity, smoothTime);

            //Camera Rotation Set
            Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - relativePos + new Vector3(0, heightOffset, 0));

            cameraTransform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, 0, 0);
        }
    }

    public void SetMouseWheel(float mouseWheel)
    {
        lastWheelAxisStorage = wheelAxisStorage;
        wheelAxisStorage += Mathf.Round(mouseWheel * 10);
        wheelAxis = Mathf.Round(mouseWheel * 10);

        if (wheelAxisStorage > zoomMinLimit)
        {
            wheelAxisStorage = zoomMinLimit;
        }
        else if (wheelAxisStorage < zoomMaxLimit)
        {
            wheelAxisStorage = zoomMaxLimit;
        }
    }
}
