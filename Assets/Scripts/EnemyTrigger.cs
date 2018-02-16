using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour {

    EnemyBehaviour enemyBehaviour;

    private void Start()
    {
        enemyBehaviour = this.GetComponentInParent<EnemyBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            enemyBehaviour.EnemyHasAttacked();
        }
    }
}
