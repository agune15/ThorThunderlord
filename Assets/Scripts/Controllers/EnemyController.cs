using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 10f;
   

    Transform target;
    NavMeshAgent agent;
    // Use this for initialization
    void Start ()
    {
        target = EnemyManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        // If inside the radius
        
        if (distance <= lookRadius)
        {
            // Move towards the player
            agent.SetDestination(target.position);
           
        }

        if (distance <= agent.stoppingDistance)
        {
            /*Attack
            combatManager.Attack(Player.instance.playerStats);*/
            FaceTarget();
            
        }
               
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
