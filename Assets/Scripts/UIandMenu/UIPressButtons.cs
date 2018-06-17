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

        SetButtonHighlights();
    }
    
    void Update () {
        if(Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
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

    void SetButtonHighlights ()
    {
        //Resolution button highlighting
        if (Screen.currentResolution.width == 1280)
        {
            GameObject.Find("1280x720").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (Screen.currentResolution.width == 1600)
        {
            GameObject.Find("1600x1200").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (Screen.currentResolution.width == 1920)
        {
            GameObject.Find("1920x1080").GetComponent<Animator>().SetTrigger("Highlighted");
        }

        //Quality button highlighting
        if (QualitySettings.GetQualityLevel() == 1)
        {
            GameObject.Find("Fast").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (QualitySettings.GetQualityLevel() == 2)
        {
            GameObject.Find("Good").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (QualitySettings.GetQualityLevel() == 3)
        {
            GameObject.Find("High").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (QualitySettings.GetQualityLevel() == 4)
        {
            GameObject.Find("Ultra").GetComponent<Animator>().SetTrigger("Highlighted");
        }

        //Full screen button highlighting
        if (Screen.fullScreen)
        {
            GameObject.Find("FullScreen").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else
        {
            GameObject.Find("Windowed").GetComponent<Animator>().SetTrigger("Highlighted");
        }

        //Language button highlighting
        GameObject.Find("English").GetComponent<Animator>().SetTrigger("Highlighted");
    }
}
