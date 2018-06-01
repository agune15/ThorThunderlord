using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour {

    CharacterBehaviour playerBehaviour;
    AudioPlayer playerAudioPlayer;

    public bool playOnThor;

    bool hasPlayedAudio = false;

    public AudioClip audioToPlay;
    public float volume;
    public float pitch;
    public bool isAudio2D;
    public string audioGroup;

	void Start () {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerAudioPlayer = playerBehaviour.gameObject.GetComponent<AudioPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayedAudio) return;

        if (other.tag == "Player")
        {
            if (!playerBehaviour.isBeingAttacked && !hasPlayedAudio)
            {
                if (playOnThor) playerAudioPlayer.Play(audioToPlay, volume, pitch, false, isAudio2D, audioGroup);
                else playerAudioPlayer.Play(gameObject, audioToPlay, volume, pitch, false, isAudio2D, audioGroup);

                GetComponent<Collider>().enabled = false;

                hasPlayedAudio = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasPlayedAudio) return;

        if (other.tag == "Player")
        {
            if (!playerBehaviour.isBeingAttacked && !hasPlayedAudio)
            {
                if (playOnThor) playerAudioPlayer.Play(audioToPlay, volume, pitch, false, isAudio2D, audioGroup);
                else playerAudioPlayer.Play(gameObject, audioToPlay, volume, pitch, false, isAudio2D, audioGroup);

                GetComponent<Collider>().enabled = false;

                hasPlayedAudio = true;
            }
        }
    }
}
