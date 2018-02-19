using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LogoSceneChange : MonoBehaviour {

    public VideoPlayer videoPlayer;
    public ChangeScene sceneChanger;
    public int sceneToJumpTo;

    float videoCountdown;

    private void Start()
    {
        videoCountdown = (float)videoPlayer.clip.length;
    }

    private void Update()
    {
        if(videoCountdown > 0)
        {
            videoCountdown -= Time.deltaTime;
            return;
        }

        sceneChanger.SetScene(sceneToJumpTo);
    }
}
