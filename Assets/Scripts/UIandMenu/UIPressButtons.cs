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
            if(startButton.isActiveAndEnabled && !cameraTransition.IsEasing() && startButton.interactable) startButton.onClick.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(exitButton.isActiveAndEnabled && !cameraTransition.IsEasing() && exitButton.interactable) exitButton.onClick.Invoke();
        }
    }
}
