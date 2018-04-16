using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPressButtons : MonoBehaviour {

    public Button startButton;
    public Button exitButton;

    [SerializeField] CameraTransition cameraTransition;

    private void Start()
    {
        cameraTransition = GameObject.FindWithTag("MainCamera").GetComponent<CameraTransition>();
    }
    
    void Update () {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(startButton.enabled && !cameraTransition.IsEasing()) startButton.onClick.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(exitButton.enabled && !cameraTransition.IsEasing()) exitButton.onClick.Invoke();
        }
    }
}
