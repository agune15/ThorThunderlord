﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerHealthBar : MonoBehaviour {

    public enum Icons { Passive, Q, W, E, R }

    CharacterBehaviour playerBehaviour;

    //UI elements
    [SerializeField] Image healthBar;


    Image qIcon;
    Image wIcon;
    Image eIcon;
    Image rIcon;

    //Cooldowns
    float passiveFillAmount;

    float qCd;
    float wCd;
    float eCd;
    float rCd;

    //Pop-Ups
    Animator enableDisablePopUpsAnimator;

    bool eventSystemEnabled = true;

    private void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        qIcon = GameObject.Find("qGreyUIicon").GetComponent<Image>();
        wIcon = GameObject.Find("wGreyUIicon").GetComponent<Image>();
        eIcon = GameObject.Find("eGreyUIicon").GetComponent<Image>();
        rIcon = GameObject.Find("rGreyUIicon").GetComponent<Image>();

        enableDisablePopUpsAnimator = GameObject.Find("enableDisablePopUps").GetComponent<Animator>();
        enableDisablePopUpsAnimator.SetBool("popUpsEnabled", eventSystemEnabled);

        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerBehaviour.GetAbilityCooldowns(out qCd, out wCd, out eCd, out rCd);

        SetIconsInitFillAmount();
    }

    public void SetCurrentPlayerHealth (float maxHealth, float currentHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    void SetIconsInitFillAmount ()
    {
        qIcon.fillAmount = 0;
        wIcon.fillAmount = 0;
        eIcon.fillAmount = 0;
        rIcon.fillAmount = 0;
    }

    public void SetIconFillAmount (Icons iconToFill, float iconFillAmount)
    {
        switch (iconToFill)
        {
            case Icons.Passive:
                break;
            case Icons.Q:
                qIcon.fillAmount = iconFillAmount;
                break;
            case Icons.W:
                wIcon.fillAmount = iconFillAmount;
                break;
            case Icons.E:
                eIcon.fillAmount = iconFillAmount;
                break;
            case Icons.R:
                rIcon.fillAmount = iconFillAmount;
                break;
            default:
                break;
        }
    }

    public void EmptyGreyIcon (Icons iconToEmpty)
    {
        StartCoroutine(EmptyGreyCorroutine(iconToEmpty));
    }

    IEnumerator EmptyGreyCorroutine (Icons iconToEmpty)
    {
        float desiredCd = GetDesiredCd(iconToEmpty);
        float timer = desiredCd;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;

            switch (iconToEmpty)
            {
                case Icons.Passive:
                    break;
                case Icons.Q:
                    yield return qIcon.fillAmount = timer / desiredCd;
                    break;
                case Icons.W:
                    yield return wIcon.fillAmount = timer / desiredCd;
                    break;
                case Icons.E:
                    yield return eIcon.fillAmount = timer / desiredCd;
                    break;
                case Icons.R:
                    yield return rIcon.fillAmount = timer / desiredCd;
                    break;
                default:
                    break;
            }
        }

        //yield return null;
    }

    private float GetDesiredCd(Icons cdToReturn)
    {
        switch (cdToReturn)
        {
            case Icons.Passive:
                return 0;
            case Icons.Q:
                return qCd;
            case Icons.W:
                return wCd;
            case Icons.E:
                return eCd;
            case Icons.R:
                return rCd;
            default:
                return 0;
        }
    }

    public void SetPopUpsAvailability ()
    {
        if (eventSystemEnabled)
        {
            qIcon.gameObject.GetComponent<EventTrigger>().enabled = false;
            wIcon.gameObject.GetComponent<EventTrigger>().enabled = false;
            eIcon.gameObject.GetComponent<EventTrigger>().enabled = false;
            rIcon.gameObject.GetComponent<EventTrigger>().enabled = false;

            eventSystemEnabled = false;

            enableDisablePopUpsAnimator.SetBool("popUpsEnabled", eventSystemEnabled);
        }
        else
        {
            qIcon.gameObject.GetComponent<EventTrigger>().enabled = true;
            wIcon.gameObject.GetComponent<EventTrigger>().enabled = true;
            eIcon.gameObject.GetComponent<EventTrigger>().enabled = true;
            rIcon.gameObject.GetComponent<EventTrigger>().enabled = true;

            eventSystemEnabled = true;

            enableDisablePopUpsAnimator.SetBool("popUpsEnabled", eventSystemEnabled);
        }
    }
}
