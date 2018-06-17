using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.PostProcessing;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public PostProcessingProfile[] postProcessingProfiles;
    public Slider basicExposure;
    public GameSettings gameSettings;

    void Start()
    {
        basicExposure.value = GameObject.FindWithTag("MainCamera").GetComponent<PostProcessingBehaviour>().profile.colorGrading.settings.basic.postExposure;
        OnFullScreenOn();
    }

    void OnEnable()
    {
        gameSettings = new GameSettings();
    }

    public void OnFullScreenOn()
    {
        gameSettings.fullScreen =  Screen.fullScreen = true;
    }

    public void OnFullScreenOff()
    {
        gameSettings.fullScreen = Screen.fullScreen = false;
    }

    public void FullHDRes()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen);
    }

    public void MidFullRes()
    {
        Screen.SetResolution(1600, 1200, Screen.fullScreen);
    }

    public void HDRes()
    {
        Screen.SetResolution(1280, 720, Screen.fullScreen);
    }

    public void ChangeQualityFast()
    {
        QualitySettings.SetQualityLevel(1, true);
    }

    public void ChangeQualityGood()
    {
        QualitySettings.SetQualityLevel(2, true);
    }

    public void ChangeQualityHigh()
    {
        QualitySettings.SetQualityLevel(3, true);
    }

    public void ChangeQualityUltra()
    {
        QualitySettings.SetQualityLevel(4, true);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void SetAmbientVolume(float volume)
    {
        audioMixer.SetFloat("AmbientVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetGamma()
    {
        foreach(PostProcessingProfile profile in postProcessingProfiles)
        {
            ColorGradingModel.Settings gradingSettings = profile.colorGrading.settings;
            gradingSettings.basic.postExposure = basicExposure.value;
            profile.colorGrading.settings = gradingSettings;
        }
    }
}
