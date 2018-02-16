using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    private AudioPlayer audioPlayer;
	// Use this for initialization
	void Start ()
    {
        audioPlayer = GetComponentInChildren<AudioPlayer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            audioPlayer.PlaySFX(0, 1, Random.Range(0.9f, 1.1f));
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            audioPlayer.Play2DSFX(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            audioPlayer.PlayAmbient(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            audioPlayer.PlayMusic(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            audioPlayer.StopMusic();
            audioPlayer.StopAmbient();
        }
    }
}
