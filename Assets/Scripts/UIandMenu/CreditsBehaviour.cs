using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsBehaviour : MonoBehaviour {

    public AnimationClip creditsAnimation;
    [SerializeField] float creditsTime;

    AudioSource creditsAudio;

    ChangeScene sceneChanger;


	void Start () {
        creditsTime = creditsAnimation.length;
        sceneChanger = gameObject.GetComponent<ChangeScene>();

        creditsAudio = gameObject.GetComponent<AudioSource>();

        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey && creditsTime >= creditsAnimation.length / 10) creditsTime = 0;

        if (creditsTime >= 0)
        {
            creditsTime -= Time.deltaTime;
            return;
        }

        StartCoroutine(FadeCreditsAudio());
        Cursor.visible = true;
        sceneChanger.SetScene(2);
    }

    IEnumerator FadeCreditsAudio ()
    {
        float fadeTime = 0.4f;
        float timer = 0;
        float audioCurrentVolume = creditsAudio.volume;

        while (timer <= fadeTime)
        {
            timer += Time.deltaTime;
            creditsAudio.volume = Mathf.SmoothStep(audioCurrentVolume, 0, timer / fadeTime);

            yield return null;
        }
    }
}
