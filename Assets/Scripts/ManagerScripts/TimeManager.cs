using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public enum ScaleTimeTypes { Flat, Scaled, FlatAndScaled };
    ScaleTimeTypes scaleTimeType = ScaleTimeTypes.Flat;

    float currentTimeScale;
    float normalTimeScale = 1;
    float normalFixedDeltaTime;
    float scaledTimeDuration = 0;
    float flatTimeDuration = 0;

    bool gameIsPaused = false;
    bool timeWasScaled = false;


    void Start () {
        Time.timeScale = normalTimeScale;
        normalFixedDeltaTime = Time.fixedDeltaTime;
	}
	
	void Update () {
        if (timeWasScaled && !gameIsPaused)
        {
            switch (scaleTimeType)
            {
                case ScaleTimeTypes.Flat:
                    if (flatTimeDuration >= 0)
                    {
                        flatTimeDuration -= Time.unscaledDeltaTime;
                    }
                    else DisableTimeScale();
                    break;
                case ScaleTimeTypes.Scaled:
                    if (Time.timeScale < normalTimeScale)
                    {
                        Time.fixedDeltaTime += (1f / scaledTimeDuration) * Time.unscaledDeltaTime;
                        Time.timeScale += (1f / scaledTimeDuration) * Time.unscaledDeltaTime;
                    }
                    else DisableTimeScale();
                    break;
                case ScaleTimeTypes.FlatAndScaled:
                    if (flatTimeDuration >= 0)
                    {
                        flatTimeDuration -= Time.unscaledDeltaTime;
                    }
                    else
                    {
                        if (Time.timeScale < normalTimeScale)
                        {
                            Time.fixedDeltaTime += (1f / scaledTimeDuration) * Time.unscaledDeltaTime;
                            Time.timeScale += (1f / scaledTimeDuration) * Time.unscaledDeltaTime;
                        }
                        else DisableTimeScale();
                    }
                    break;
                default:
                    break;
            }
        }
        else return;
	}

    void DisableTimeScale ()
    {
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = normalFixedDeltaTime;
        timeWasScaled = false;
    }

    public void SetTimeScaleAndDuration (float desiredTimeScale, float duration, ScaleTimeTypes timeType)
    {
        scaleTimeType = timeType;

        if (scaleTimeType == ScaleTimeTypes.Flat) flatTimeDuration = duration;
        else if (scaleTimeType == ScaleTimeTypes.Scaled) scaledTimeDuration = duration;

        Time.timeScale = desiredTimeScale;
        Time.fixedDeltaTime = normalFixedDeltaTime * desiredTimeScale;
        timeWasScaled = true;
    }

    public void SetTimeScaleAndDuration(float desiredTimeScale, float scaledDuration, float flatDuration, ScaleTimeTypes timeType)
    {
        scaleTimeType = timeType;

        if (scaleTimeType == ScaleTimeTypes.FlatAndScaled)
        {
            scaledTimeDuration = scaledDuration;
            flatTimeDuration = flatDuration;
        }

        SetTimeScaleAndDuration(desiredTimeScale, 0, ScaleTimeTypes.FlatAndScaled);
    }

    public void SetTimeScale (float desiredTimeScale)
    {
        Time.timeScale = desiredTimeScale;
    }

    public void SetGameState (bool isPaused)
    {
        gameIsPaused = isPaused;
    }
}
