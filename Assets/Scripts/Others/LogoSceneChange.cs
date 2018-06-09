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

        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)) videoCountdown = 0;

        if (videoCountdown > 0)
        {
            videoCountdown -= Time.deltaTime;
            return;
        }

        Cursor.visible = true;
        sceneChanger.SetScene(sceneToJumpTo);
    }
}
