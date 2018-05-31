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
    

	void Start () {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerInitLife = playerBehaviour.GetLife();
        particleInstancer = GameObject.FindWithTag("ParticleInstancer").GetComponent<ParticleInstancer>();

        initPosition = auraTransform.localPosition;

        initY = initPosition.y;
        deltaY = endY - initY;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!playerBehaviour.isBeingAttacked)
            {
                if (!hasHealed)
                {
                    if (playerBehaviour.GetLife() >= playerInitLife) return;
                    else
                    {
                        StartCoroutine(playerBehaviour.HealOverTime(healingPercentage, healingTime));
                        StartCoroutine(ParticleFadeOut());
                        //particleInstancer.InstanciateParticleSystem("Thor_Healing")
                        //instanciate particle Player
                        //remove particle that shows altar hasn't been used
                        //change lightning?

                        hasHealed = true;
                    }
                }
            }
        }
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
