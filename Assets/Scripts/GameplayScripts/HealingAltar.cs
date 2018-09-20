﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAltar : MonoBehaviour {

    public Transform auraTransform;
    public Renderer auraRenderer;

    CharacterBehaviour playerBehaviour;
    ParticleInstancer particleInstancer;

    //Healing parameters
    public int healingPercentage;
    public float healingTime;
    float playerInitLife;
    bool hasHealed = false;

    //Translation parameters
    Vector3 initPosition;

    public float translationTime;
    float translationTimer = 0;
    float currentY;
    float initY;
    public float endY;
    float deltaY;

    bool isGoingUp = true;
    
    [Header("SFX Parameters")] //SFX parameters
    public List<ClipAndVolume> healingAudios = new List<ClipAndVolume>();
    Dictionary<string, SourceAndVolume> audioSources = new Dictionary<string, SourceAndVolume>();


	void Start () {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerInitLife = playerBehaviour.GetLife();
        particleInstancer = GameObject.FindWithTag("ParticleInstancer").GetComponent<ParticleInstancer>();

        initPosition = auraTransform.localPosition;

        initY = initPosition.y;
        deltaY = endY - initY;

        PlayAudio(0, healingAudios[0].volume, this.gameObject, true, true);
    }

    private void Update()
    {
        if (auraTransform.gameObject.activeInHierarchy)
        {
            if(isGoingUp)
            {
                translationTimer += Time.deltaTime;
                currentY = Easing.QuadEaseInOut(translationTimer, initY, deltaY, translationTime);

                if(translationTimer >= translationTime)
                {
                    isGoingUp = false;
                    translationTimer = translationTime;
                    currentY = deltaY;
                }

                auraTransform.localPosition = initPosition + new Vector3(0, currentY, 0);
            }
            else
            {
                translationTimer -= Time.deltaTime;
                currentY = Easing.QuadEaseInOut(translationTimer, initY, deltaY, translationTime);

                if(translationTimer <= 0)
                {
                    isGoingUp = true;
                    translationTimer = 0;
                    currentY = initY;
                }

                auraTransform.localPosition = initPosition + new Vector3(0, currentY, 0);
            }
        }

        if (hasHealed)
        {
            //Fade-out mystic audio
            //Fade-in thor heal
        }
        else
        {
            //calcular volumen de MysticChimes en relación al Player
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!hasHealed)
            {
                if (!playerBehaviour.isBeingAttacked)
                {
                    if (playerBehaviour.GetLife() >= playerInitLife) return;
                    else
                    {
                        StartCoroutine(playerBehaviour.HealOverTime(healingPercentage, healingTime));
                        StartCoroutine(ParticleFadeOut());
                        //change lightning?

                        hasHealed = true;
                        //Play healing audio on Thor
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!hasHealed)
            {
                if (!playerBehaviour.isBeingAttacked)
                {
                    if (playerBehaviour.GetLife() >= playerInitLife) return;
                    else
                    {
                        StartCoroutine(playerBehaviour.HealOverTime(healingPercentage, healingTime));
                        StartCoroutine(ParticleFadeOut());
                        //change lightning?

                        hasHealed = true;
                        //Play healing audio on Thor
                    }
                }
            }
        }
    }

    void PlayAudio (int clipIndex, float audioVolume, GameObject audioTarget, bool loopAudio, bool audio2D)
    {
        AudioSource source = audioTarget.AddComponent<AudioSource>();

        source.Play(healingAudios[clipIndex].clip, audioVolume, 1.0f, loopAudio, audio2D, "SFX");

        SourceAndVolume newSource = new SourceAndVolume(source, healingAudios[clipIndex].volume);
        if (!audioSources.ContainsKey(healingAudios[clipIndex].clip.name)) audioSources.Add(healingAudios[clipIndex].clip.name, newSource);
    }

    IEnumerator ParticleFadeOut ()
    {
        float time = healingTime;
        Color currentColor = auraRenderer.material.GetColor("_TintColor");
        float amountToRemove = currentColor.a;

        while (time >= 0)
        {
            time -= Time.deltaTime;

            currentColor.a = Easing.SineEaseInOut(time, 0, amountToRemove, healingTime);
            auraRenderer.material.SetColor("_TintColor", currentColor);

            if(time <= 0)
            {
                auraTransform.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }
}

[System.Serializable]
public class ClipAndVolume
{
    public AudioClip clip;
    public float volume;

    public ClipAndVolume (AudioClip audioClip, float audioVolume)
    {
        clip = audioClip;
        volume = audioVolume;
    }
}

public class SourceAndVolume
{
    public AudioSource source;
    public float volume;

    public SourceAndVolume (AudioSource audioSource, float sourceVolume)
    {
        source = audioSource;
        volume = sourceVolume;
    }
}