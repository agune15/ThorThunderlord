using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAltar : MonoBehaviour {

    public Transform auraTransform;
    public Renderer auraRenderer;

    CharacterBehaviour playerBehaviour;
    ParticleInstancer particleInstancer;

    //Healing parameters
    public int healingPercentage;
    float healingTimer;
    public float healingTime;
    float playerInitLife;
    bool hasHealed = false;
    bool hasFinishedHealing = false;

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

        if (!hasFinishedHealing)
        {
            if (hasHealed)
            {
                audioSources["MysticChimes"].source.volume = audioSources["MysticChimes"].volume * (healingTimer / healingTime);

                if (healingTimer >= healingTime * 0.75f)
                {
                    float healingTimerFactor = 1 - ((healingTimer - (healingTime * 0.75f)) / (healingTime * 0.25f));

                    audioSources["Thor_healing"].source.volume = audioSources["Thor_healing"].volume * healingTimerFactor;
                }
                else if (healingTimer <= healingTime * 0.25f)
                {
                    float healingTimerFactor = healingTimer / (healingTime * 0.25f);

                    audioSources["Thor_healing"].source.volume = audioSources["Thor_healing"].volume * healingTimerFactor;
                }

                if (healingTimer <= 0)
                {
                    foreach (SourceAndVolume sourceAndVolume in audioSources.Values)
                    {
                        Destroy(sourceAndVolume.source);
                    }

                    hasFinishedHealing = true;
                }
            }
            else
            {
                float distanceFromPlayer = (Vector3.Distance(transform.position, playerBehaviour.transform.position) > 20) ? 20 : Vector3.Distance(transform.position, playerBehaviour.transform.position);
                distanceFromPlayer = (20 - distanceFromPlayer) / 20;

                audioSources["MysticChimes"].source.volume = 0.1f * distanceFromPlayer;
            }
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
                        PlayAudio(1, 0, playerBehaviour.gameObject, false, true);
                        //change lighting?

                        hasHealed = true;
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
                        PlayAudio(1, 0, playerBehaviour.gameObject, false, true);
                        //change lighting?

                        hasHealed = true;
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
        healingTimer = healingTime;
        Color currentColor = auraRenderer.material.GetColor("_TintColor");
        float amountToRemove = currentColor.a;

        while (healingTimer >= 0)
        {
            healingTimer -= Time.deltaTime;

            currentColor.a = Easing.SineEaseInOut(healingTimer, 0, amountToRemove, healingTime);
            auraRenderer.material.SetColor("_TintColor", currentColor);

            if(healingTimer <= 0)
            {
                auraTransform.gameObject.SetActive(false);
                yield break;
            }

            yield return null;
        }
    }
}