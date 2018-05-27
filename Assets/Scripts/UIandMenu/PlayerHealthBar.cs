using System.Collections;
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
    public List<TagAndTexture> indicatorTextures = new List<TagAndTexture>();

    bool eventSystemEnabled = true;

    //AOE Indicator
    Transform indicatorTransform;
    Projector indicatorProjector;

    Material indicatorMaterial;

    float qRange;
    float wRange;
    float eRange;
    float rRange;

    private void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        qIcon = GameObject.Find("qGreyUIicon").GetComponent<Image>();
        wIcon = GameObject.Find("wGreyUIicon").GetComponent<Image>();
        eIcon = GameObject.Find("eGreyUIicon").GetComponent<Image>();
        rIcon = GameObject.Find("rGreyUIicon").GetComponent<Image>();

        enableDisablePopUpsAnimator = GameObject.Find("enableDisablePopUps").GetComponent<Animator>();
        enableDisablePopUpsAnimator.SetBool("popUpsEnabled", eventSystemEnabled);

        indicatorTransform = GameObject.FindWithTag("AOEIndicator").transform;
        indicatorProjector = indicatorTransform.gameObject.GetComponent<Projector>();
        indicatorMaterial = indicatorProjector.material;

        AOEInidicator ("none");

        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerBehaviour.GetAbilityCooldowns(out qCd, out wCd, out eCd, out rCd);
        playerBehaviour.GetAbilityRanges(out qRange, out wRange, out eRange, out rRange);

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
        }
        else
        {
            qIcon.gameObject.GetComponent<EventTrigger>().enabled = true;
            wIcon.gameObject.GetComponent<EventTrigger>().enabled = true;
            eIcon.gameObject.GetComponent<EventTrigger>().enabled = true;
            rIcon.gameObject.GetComponent<EventTrigger>().enabled = true;

            eventSystemEnabled = true;
        }

        enableDisablePopUpsAnimator.SetBool("popUpsEnabled", eventSystemEnabled);
    }

    public void AOEInidicator (string abilityKey)
    {
        if (abilityKey == "q")
        {
            indicatorTransform.gameObject.SetActive(true);

            indicatorTransform.localPosition = new Vector3(0, 10, qRange / 2);
            indicatorProjector.orthographicSize = qRange / 2;

            int textureIndex = indicatorTextures.FindIndex(texture => texture.tag == abilityKey);
            indicatorMaterial.SetTexture("_ShadowTex", indicatorTextures[textureIndex].texture);
        }
        else if (abilityKey == "w")
        {
            indicatorTransform.gameObject.SetActive(true);

            indicatorTransform.localPosition = new Vector3(0, 10, 0);
            indicatorProjector.orthographicSize = wRange;

            int textureIndex = indicatorTextures.FindIndex(texture => texture.tag == abilityKey);
            indicatorMaterial.SetTexture("_ShadowTex", indicatorTextures[textureIndex].texture);
        }
        else if (abilityKey == "e")
        {
            indicatorTransform.gameObject.SetActive(true);

            indicatorTransform.localPosition = new Vector3(0, 10, eRange / 2);
            indicatorProjector.orthographicSize = eRange / 2;

            int textureIndex = indicatorTextures.FindIndex(texture => texture.tag == abilityKey);
            indicatorMaterial.SetTexture("_ShadowTex", indicatorTextures[textureIndex].texture);
        }
        else if (abilityKey == "r")
        {
            indicatorTransform.gameObject.SetActive(true);

            indicatorTransform.localPosition = new Vector3(0, 10, 0);
            indicatorProjector.orthographicSize = rRange;

            int textureIndex = indicatorTextures.FindIndex(texture => texture.tag == abilityKey);
            indicatorMaterial.SetTexture("_ShadowTex", indicatorTextures[textureIndex].texture);
        }
        else
        {
            indicatorTransform.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class TagAndTexture
{
    public string tag;
    public Texture2D texture;

    public TagAndTexture (string textureTag, Texture2D textureInput)
    {
        tag = textureTag;
        texture = textureInput;
    }
}