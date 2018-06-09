using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PostProcessing;

public class GameManager : MonoBehaviour {

    public PostProcessingProfile[] postProcessingProfiles;

    private void Start()
    {
        AudioManager.Initialize();
        ResetPostProcessingProfiles();
    }

    void ResetPostProcessingProfiles()
    {
        foreach (PostProcessingProfile profile in postProcessingProfiles)
        {
            ColorGradingModel.Settings gradingSettings = profile.colorGrading.settings;
            gradingSettings.basic.postExposure = 0;
            profile.colorGrading.settings = gradingSettings;
        }
    }
}
