using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTrigger : MonoBehaviour {

    EnemyStats.EnemyType enemyType;

    HammerBehaviour hammerBehaviour;
    Transform playerTransform;

    void Start()
    {
        hammerBehaviour = this.GetComponentInParent<HammerBehaviour>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        if(transform.localPosition != Vector3.zero) transform.localPosition = Vector3.zero;
        if(transform.localRotation.eulerAngles != Vector3.zero) transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Enemy")
        {
            Vector3 directionToEnemy = other.transform.position - playerTransform.position;

            other.gameObject.GetComponent<EnemyStats>().SetDamage(hammerBehaviour.DealDamage(), playerTransform.rotation);

            Debug.Log("triggerEnter");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyStats>().SetDamage(hammerBehaviour.DealDamage());

            Debug.Log("triggerStay");
        }
    }
}
