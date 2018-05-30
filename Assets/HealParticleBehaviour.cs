using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealParticleBehaviour : MonoBehaviour {

    public Renderer circleRenderer;

    public float offsetSpeed;

    Vector2 inputOffset;
    float outputOffset;


    void Update () {
        inputOffset = circleRenderer.material.GetTextureOffset("_MainTex");

        outputOffset = inputOffset.x + (Time.deltaTime * offsetSpeed);

        if (outputOffset >= 1) outputOffset = outputOffset - 1;
        
        circleRenderer.material.SetTextureOffset("_MainTex", new Vector2(outputOffset, 0));
    }
}
