using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodMode : MonoBehaviour
{
    public bool isGodModeEnabled;
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            isGodModeEnabled = !isGodModeEnabled;
            Debug.Log("GOD Mode enabled");
        }
    }
}
