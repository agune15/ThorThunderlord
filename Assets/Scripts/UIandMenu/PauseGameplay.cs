using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameplay : MonoBehaviour {

    public GameObject canvas;
    public Image pauseBackground;

    public GameObject options;
    public GameObject video;
    public GameObject sound;
    public GameObject controls;
    public GameObject languages;

    [HideInInspector] public bool isGamePaused = false;
    float pauseTimeScale = 0;
    float runningTimeScale = 1;
    
    public float pauseBackgroundFadeTime;
    

    public void Pause()
    {
        isGamePaused = !isGamePaused;

        if(isGamePaused)
        {
            if(canvas.activeSelf == false)
            {
                canvas.SetActive(true);
                SetPauseChildCanvasesState(true);
                StartCoroutine(PauseWaitTime());
                Time.timeScale = pauseTimeScale;
            }
        }
        else
        {
            StartCoroutine(PauseWaitTime());
            SetPauseChildCanvasesState(false);
            canvas.SetActive(false);
            Time.timeScale = runningTimeScale;
        }
    }

    IEnumerator PauseWaitTime ()
    {
        yield return new WaitForSeconds(pauseBackgroundFadeTime);
    }

    void SetPauseChildCanvasesState (bool areActive)
    {
        options.SetActive(areActive);
        video.SetActive(areActive);
        sound.SetActive(areActive);
        controls.SetActive(areActive);
        languages.SetActive(areActive);
    }
}
