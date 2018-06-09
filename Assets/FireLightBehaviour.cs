using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLightBehaviour : MonoBehaviour {

    Animator fireLightAnimator;


	void Start () {
        fireLightAnimator = GetComponent<Animator>();
        int intensityVariant = Random.Range(1, 4);

        fireLightAnimator.SetInteger("desiredIntensity", intensityVariant);
	}
}
