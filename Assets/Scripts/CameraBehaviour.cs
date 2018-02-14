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

    float zoomAddition = 0.64f;

    float zoomMinLimit = 0.8f;
    float zoomMaxLimit = 0;

    Vector3 minOffset;
    Vector3 maxOffset;

    float wheelAxisStorage = 0;

    public float smoothTime;
    float timeCounter = 0;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cameraTransform = this.transform;

        startPos = cameraTransform.position;
        relativePos = playerTransform.position + offsetPos;

        playerCanMove = false;

        minOffset = offsetPos + new Vector3(0, -zoomAddition * 8, zoomAddition * 8 * 0.75f);
        maxOffset = offsetPos;
    }

    private void LateUpdate()
    {
        if(!playerTransform) return;

        //Camera Offset Position
        relativePos = playerTransform.position + offsetPos + new Vector3(0, heightOffset, 0);

        //Initial Transition
        if(timeCounter <= smoothTime)
        {
            timeCounter += Time.deltaTime;

            //Camera Start Position
            cameraTransform.position = Vector3.Lerp(startPos, relativePos, Mathf.SmoothStep(0, 1, timeCounter / smoothTime));

            //Camera Start Rotation
            Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - cameraTransform.position + new Vector3(0, heightOffset, 0));

            cameraTransform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, 0, 0);
        }
        else
        {
            smoothTime = 0.25f;
            playerCanMove = true;   //Permitir mover el player

            //Camera Position
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, relativePos, ref velocity, smoothTime);

            //Camera Rotation
            Quaternion lookRotation = Quaternion.LookRotation(playerTransform.position - relativePos + new Vector3(0, heightOffset, 0));

            cameraTransform.rotation = Quaternion.Euler(lookRotation.eulerAngles.x, 0, 0);

            //Camera Zoom
            wheelAxisStorage += wheelAxis;
            
            if (wheelAxisStorage <= zoomMinLimit && wheelAxisStorage >= zoomMaxLimit)
            {
                offsetPos.y -= wheelAxis * 10 * zoomAddition;
                offsetPos.z += wheelAxis * 10 * zoomAddition * 0.75f;
            }
            else if (wheelAxisStorage >= zoomMinLimit)
            {
                if (offsetPos.y < minOffset.y || offsetPos.z > minOffset.z)
                {
                    offsetPos = minOffset;
                }

                wheelAxisStorage = zoomMinLimit;
            }
            else if (wheelAxisStorage <= zoomMaxLimit)
            {
                if(offsetPos.y > maxOffset.y || offsetPos.z > maxOffset.z)
                {
                    offsetPos = maxOffset;
                }

                wheelAxisStorage = zoomMaxLimit;
            }
        }
    }

    public void SetMouseWheel(float mouseWheel)
    {
        wheelAxis = mouseWheel;
    }
}
