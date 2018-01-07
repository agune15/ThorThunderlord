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

    public bool playerCanMove;

    float wheelAxis = 0;
    float zoomSpeed = 4;

    float zoomMinLimit = 0.8f;
    float zoomMaxLimit = 0;

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
                offsetPos.z += wheelAxis * zoomSpeed * 75 * Time.deltaTime;
                offsetPos.y -= wheelAxis * zoomSpeed * 100 * Time.deltaTime;
            }
            else if (wheelAxisStorage >= zoomMinLimit)
            {
                wheelAxisStorage = zoomMinLimit;
            }
            else if (wheelAxisStorage <= zoomMaxLimit)
            {
                wheelAxisStorage = zoomMaxLimit;
            }
        }
    }

    public void SetMouseWheel(float mouseWheel)
    {
        wheelAxis = mouseWheel;
    }
}
