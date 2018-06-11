using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour {

    Canvas healthCanvas;

    //UI elements
    Image healthBarBG;
    Image healthBarMask;
    Image currentHealthBar;

    Text enemyName;

    //Enemy related variables
    [HideInInspector] public GameObject playerAttackingTarget = null;


    private void Start()
    {
        healthBarBG = GameObject.Find("EnemyHealthBarBG").GetComponent<Image>();
        healthBarMask = GameObject.Find("EnemyHealthMask").GetComponent<Image>();
        currentHealthBar = GameObject.Find("EnemyCurrentHealth").GetComponent<Image>();

        enemyName = GameObject.Find("EnemyName").GetComponent<Text>();

        healthCanvas = GameObject.Find("EnemyHealthBar").GetComponent<Canvas>();
        healthCanvas.enabled = false;
    }

    public void DrawEnemyHealthBar(float maxHealth, float currentHealth, string enemyType)
    {
        if (playerAttackingTarget != null) return;

        if (!healthCanvas.enabled) healthCanvas.enabled = true;

        currentHealthBar.rectTransform.sizeDelta = new Vector2(maxHealth * 2, currentHealthBar.rectTransform.sizeDelta.y);
        healthBarMask.rectTransform.sizeDelta = currentHealthBar.rectTransform.sizeDelta;
        healthBarBG.rectTransform.sizeDelta = new Vector2(currentHealthBar.rectTransform.sizeDelta.x, healthBarBG.rectTransform.sizeDelta.y) + new Vector2(8, 0);

        enemyName.text = enemyType.ToUpper();

        currentHealthBar.fillAmount = currentHealth / maxHealth;
    }

    //Overload to assign "playerAttackingTarget"
    public void DrawEnemyHealthBar(GameObject playerTarget, float maxHealth, float currentHealth, string enemyType)
    {
        playerAttackingTarget = playerTarget;

        if (!healthCanvas.enabled) healthCanvas.enabled = true;

        currentHealthBar.rectTransform.sizeDelta = new Vector2(maxHealth * 2, currentHealthBar.rectTransform.sizeDelta.y);
        healthBarMask.rectTransform.sizeDelta = currentHealthBar.rectTransform.sizeDelta;
        healthBarBG.rectTransform.sizeDelta = new Vector2(currentHealthBar.rectTransform.sizeDelta.x, healthBarBG.rectTransform.sizeDelta.y) + new Vector2(8, 0);

        enemyName.text = enemyType.ToUpper();

        currentHealthBar.fillAmount = currentHealth / maxHealth;
    }

    public void DisableEnemyHealthBar()
    {
        if (playerAttackingTarget == null) healthCanvas.enabled = false;
    }

    //Overload to remove "playerAttackingTarget"
    public void DisableEnemyHealthBar(GameObject playerTarget)
    {
        playerAttackingTarget = playerTarget;
        DisableEnemyHealthBar();
    }

}
