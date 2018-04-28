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
            Vector3 directionToTarget = this.transform.position - other.transform.position;
            float desiredAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

            other.gameObject.GetComponent<EnemyStats>().SetDamage(hammerBehaviour.DealDamage(), Quaternion.Euler(new Vector3(0, desiredAngle, 0)));

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
