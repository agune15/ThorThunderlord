using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    float scrollWheel;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void PlayerInputs()
    {
        scrollWheel = Input.GetAxis("Mouse ScrollWheel");
    }
}
