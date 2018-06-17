using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameplay : MonoBehaviour {

    TimeManager timeManager;

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

    private void Start()
    {
        timeManager = GameObject.FindWithTag("manager").GetComponent<TimeManager>();
    }

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
                timeManager.SetGameState(true);
                CursorManager.SetAndStoreCursor("default", Vector2.zero, CursorMode.Auto);
                Time.timeScale = pauseTimeScale;

               SetButtonHighlights();

                foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    enemy.GetComponent<EnemyStats>().SetDefaultShader();
                }
            }
        }
        else
        {
            StartCoroutine(PauseWaitTime());
            SetPauseChildCanvasesState(false);
            canvas.SetActive(false);
            Time.timeScale = runningTimeScale;
            timeManager.SetGameState(false);
            timeManager.SetTimeScaleAndDuration(0.1f, 0.3f, 0.5f, TimeManager.ScaleTimeTypes.FlatAndScaled);
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

    void SetButtonHighlights()
    {
        //Resolution button highlighting
        if (Screen.currentResolution.width == 1280)
        {
            GameObject.Find("1280 x 720").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (Screen.currentResolution.width == 1600)
        {
            GameObject.Find("1600 x 1200").GetComponent<Animator>().SetTrigger("Highlighted");
        }
        else if (Screen.currentResolution.width == 1920)
        {
            GameObject.Find("1920 x 1080").GetComponent<Animator>().SetTrigger("Highlighted");
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
