
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    
    // The target we are following
    public Transform target;
    // The height we want the camera to be above the target
    public float height = 5.0f;
    public float back = 5.0f;
    // How much we want the Smooth of the camera
    public float cameraSmooth;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (!target) return;        
        
        Vector3 targetPos = target.position;
        targetPos.y += height;
        targetPos.z -= back;

        transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref velocity, cameraSmooth);

        //transform.LookAt(target);        
    }
}