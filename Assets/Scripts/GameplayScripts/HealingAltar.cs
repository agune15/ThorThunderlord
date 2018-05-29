using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingAltar : MonoBehaviour {

    CharacterBehaviour playerBehaviour;

    public int healingPercentage;
    public float healingTime;
    float playerInitLife;
    bool hasHealed = false;
    

	void Start () {
        playerBehaviour = GameObject.FindWithTag("Player").GetComponent<CharacterBehaviour>();
        playerInitLife = playerBehaviour.GetLife();
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
                        //playerBehaviour.HealLifeOverTime(healingPercentage, healingTime);
                        StartCoroutine(playerBehaviour.HealHealOverTime(healingPercentage, healingTime));
                        Debug.Log("yeeeeeeeeeee");
                        //start coroutine -> heal
                        //instanciate particle
                        //remove particle that shows altar hasn't been used
                        //change lightning?

                        hasHealed = true;
                    }
                }
            }
        }
    }
}
