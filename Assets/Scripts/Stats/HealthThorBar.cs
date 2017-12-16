using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthThorBar : MonoBehaviour
{
    public Image thorBarImage;
    public CharacterStats player;

    // Use this for initialization
    void Start()
    {
        UpdateEnergyUI();
    }

    public void UpdateEnergyUI()
    {
        thorBarImage.fillAmount = (1 / 100.0f) * player.currentHealth;
    }
}
