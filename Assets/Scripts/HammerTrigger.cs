using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTrigger : MonoBehaviour {

    EnemyStats.EnemyType enemyType;

    HammerBehaviour hammerBehaviour;

    private void Update()
    {
        if(transform.localPosition != Vector3.zero) transform.localPosition = Vector3.zero;
        if(transform.localRotation.eulerAngles != Vector3.zero) transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Start ()
    {
        hammerBehaviour = this.GetComponentInParent<HammerBehaviour>();
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyStats>().SetDamage(hammerBehaviour.DealDamage());

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
