using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPressButtons : MonoBehaviour {

    [Header("Buttons")]
    public Button startButton;
    public Button mainExitButton;
    public Button optionsExitButton;
    public Button videoExitButton;
    public Button soundExitButton;
    public Button controlsExitButton;
    public Button languageExitButton;

    [Header("Animators")]
    public Animator optionsPanelAnimator;
    public Animator videoPanelAnimator;
    public Animator soundPanelAnimator;
    public Animator controlsPanelAnimator;
    public Animator languagePanelAnimator;

    [SerializeField] CameraTransition cameraTransition;

    private void Start()
    {
        cameraTransition = GameObject.FindWithTag("MainCamera").GetComponent<CameraTransition>();
    }
    
    void Update () {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (startButton.isActiveAndEnabled && !cameraTransition.IsEasing() && startButton.interactable) startButton.onClick.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainExitButton.isActiveAndEnabled && !cameraTransition.IsEasing() && mainExitButton.interactable) mainExitButton.onClick.Invoke();
            else if (optionsPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("OpenOptionsPanels") && optionsExitButton.interactable) optionsExitButton.onClick.Invoke();
            else if (videoPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("OpenVideoPanels") && videoExitButton.interactable) videoExitButton.onClick.Invoke();
            else if (soundPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("OpenSoundOptions") && soundExitButton.interactable) soundExitButton.onClick.Invoke();
            else if (controlsPanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("openControlsOption") && controlsExitButton.interactable) controlsExitButton.onClick.Invoke();
            else if (languagePanelAnimator.GetCurrentAnimatorStateInfo(0).IsName("OpenLanguageOptions") && languageExitButton.interactable) languageExitButton.onClick.Invoke();
        }
    }
}
