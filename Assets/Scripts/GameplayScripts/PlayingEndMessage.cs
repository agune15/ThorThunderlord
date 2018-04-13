using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayingEndMessage : MonoBehaviour {

    public VideoPlayer victoryVideo;
    public VideoPlayer defeatVideo;
    public GameObject backToMenuCanvas;

    public static bool play = false;
    public static bool playerWon = false;

    float backToMenuDelay = 1.5f;
	
	// Update is called once per frame
	void Update () {
		if (play)
        {
            if (playerWon) victoryVideo.enabled = true;
            else defeatVideo.enabled = true;

            if(backToMenuDelay > 0)
            {
                backToMenuDelay -= Time.deltaTime;
                return;
            }

            if (!backToMenuCanvas.activeSelf) backToMenuCanvas.SetActive(true);
        }
	}

    public static void PlayVictory ()
    {
        play = true;
        playerWon = true;
    }

    public static void PlayDefeat ()
    {
        play = true;
        playerWon = false;
    }

    public static void ResetEndMessage ()
    {
        play = false;
        playerWon = false;
    }
}
