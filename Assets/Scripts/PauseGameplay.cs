using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameplay : MonoBehaviour {

    public bool isGamePaused = false;
    float pauseTimeScale = 0;
    float runningTimeScale = 1;

    public GameObject canvas;

    public void Pause()
    {
        isGamePaused = !isGamePaused;

        if(isGamePaused)
        {
            if(canvas.activeSelf == false)
            {
                canvas.SetActive(true);
                Time.timeScale = pauseTimeScale;
            }
        }
        else
        {
            canvas.SetActive(false);
            Time.timeScale = runningTimeScale;
        }
    }
}
