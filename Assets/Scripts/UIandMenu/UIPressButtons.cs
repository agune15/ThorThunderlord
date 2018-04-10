using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPressButtons : MonoBehaviour {

    public Button startButton;
    public Button exitButton;

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(startButton.enabled) startButton.onClick.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(exitButton.enabled) exitButton.onClick.Invoke();
        }
    }
}
