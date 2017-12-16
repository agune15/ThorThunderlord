using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles interaction with the Enemy */

[RequireComponent(typeof(CharacterStats))]
public class Enemy : Interactable
{
    public Material enemyMat;
    PlayerManager playerManager;
    CharacterStats myStats;
    public float delay = 0.5f; // Temps que triga en posar-se blanc again.

    void Start()
    {
        enemyMat.color = Color.white;
        playerManager = PlayerManager.instance;
        myStats = GetComponent<CharacterStats>();
    }

    public override void Interact()
    {
        base.Interact();
        CharacterCombat playerCombat = playerManager.player.GetComponent<CharacterCombat>();
        if (playerCombat != null)
        {
            playerCombat.Attack(myStats);
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        StartCoroutine(MatDelay(delay));
        enemyMat.color = Color.red;
    }

    IEnumerator MatDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyMat.color = Color.white;
    }
}