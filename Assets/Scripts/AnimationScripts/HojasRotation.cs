using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HojasRotation : MonoBehaviour
{
    public float currentTime = 0;
    public Vector3 iniValue;
    public Vector3 finalValue;
    public float durationTime;

    public Vector3 deltaValue;
    // Use this for initialization
    void Start()
    {
        deltaValue = finalValue - iniValue;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentTime < durationTime)
        {
            //float easingValue = Easing.BackEaseOut(currentTime, inValue.x, deltaValue.x, durationTime);
            Vector3 easingValue = new Vector3(
                Easing.QuadEaseInOut(currentTime, iniValue.x, deltaValue.x, durationTime),
				Easing.QuadEaseInOut(currentTime, iniValue.y, deltaValue.y, durationTime),
				Easing.QuadEaseInOut(currentTime, iniValue.z, deltaValue.z, durationTime));

            //transform.position = easingValue;
            //transform.localScale = easingValue;
            transform.localRotation = Quaternion.Euler(easingValue.x, easingValue.y, easingValue.z);

            currentTime += Time.deltaTime;

            if (currentTime >= durationTime)
            {
                transform.localRotation = Quaternion.Euler(easingValue.x, easingValue.y, easingValue.z);


                Vector3 final = finalValue;
                finalValue = iniValue;
                iniValue = final;
                deltaValue = finalValue - iniValue;

                currentTime = 0;
            }

        }
    }
}