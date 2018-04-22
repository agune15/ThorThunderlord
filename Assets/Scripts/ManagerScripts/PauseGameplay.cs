using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameplay : MonoBehaviour {

    public GameObject canvas;
    Image pauseBackground;

    [HideInInspector] public bool isGamePaused = false;
    float pauseTimeScale = 0;
    float runningTimeScale = 1;

    public float pauseBackgroundMaxAlpha;
    float pauseBackgroundFadeTime;
    float pauseBackgroundFadeTimer;

    private void Start()
    {
        pauseBackground = GameObject.Find("PauseBackground").GetComponent<Image>();
    }
    
    public void Pause()
    {
        isGamePaused = !isGamePaused;

        if(isGamePaused)
        {
            if(canvas.activeSelf == false)
            {
                canvas.SetActive(true);
                Time.timeScale = pauseTimeScale;
                StartCoroutine(PauseBackground());
            }
        }
        else
        {
            pauseBackground.color = new Color(pauseBackground.color.r, pauseBackground.color.g, pauseBackground.color.b, 0);
            canvas.SetActive(false);
            Time.timeScale = runningTimeScale;
        }
    }

    IEnumerator PauseBackground ()
    {
        pauseBackground.color = new Color(pauseBackground.color.r, pauseBackground.color.g, pauseBackground.color.b, 0);

        while (pauseBackgroundFadeTimer <= pauseBackgroundFadeTime)
        {
            //
        }

        yield return 0;
    }
}
