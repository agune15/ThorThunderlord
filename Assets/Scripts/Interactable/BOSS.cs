using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class BOSS : Interactable
{
    private EnemyController enemyController; 

    public Transform[] path;
    private int pathIndex = 0;

    public Material enemyMat;
    PlayerManager playerManager;
    CharacterStats myStats;
    public float delay = 0.5f; // Temps que triga en posar-se blanc again.

    void Start()
    {
        enemyMat.color = Color.white;
        playerManager = PlayerManager.instance;
        myStats = GetComponent<CharacterStats>();
        enemyController = GetComponent<EnemyController>();
    }

    public override void Update()
    {
        base.Update();
        if (myStats.currentHealth < myStats.maxHealth.baseValue / 2)
        {
            myStats.damage.baseValue = 30; 
        }
        if (myStats.currentHealth < myStats.maxHealth.baseValue / 3)
        {
            myStats.damage.baseValue = 40;
        }
        if (myStats.currentHealth < myStats.maxHealth.baseValue / 5)
        {
            myStats.damage.baseValue = 50;
        }
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

    public override void Patrol()
    {
        base.Patrol();
        if (enemyController.agent.remainingDistance <= enemyController.agent.stoppingDistance)
        {
            pathIndex++;
            if (pathIndex >= path.Length) pathIndex = 0;
            NewPatrolPath();
        }
    }

    void NewPatrolPath()
    {
        enemyController.agent.SetDestination(path[pathIndex].position);
    }
}
