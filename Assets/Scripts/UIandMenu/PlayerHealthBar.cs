using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {

    public enum Icons { Passive, Q, W, E, R }

    CharacterBehaviour playerBehaviour;

    //UI elements
    Image healthBar;

    Image qIcon;
    Image wIcon;
    Image eIcon;

    //Cooldowns
    float passiveFillAmount;

    float qCd;
    float wCd;
    float eCd;
    float rCd;

    private void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        qIcon = GameObject.Find("qGreyUIicon").GetComponent<Image>();
        wIcon = GameObject.Find("wGreyUIicon").GetComponent<Image>();
        eIcon = GameObject.Find("eGreyUIicon").GetComponent<Image>();

        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerBehaviour.GetAbilityCooldowns(out qCd, out wCd, out eCd);

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
                return 0;
            default:
                return 0;
        }
    }

    #region Corroutines


    #endregion
}
