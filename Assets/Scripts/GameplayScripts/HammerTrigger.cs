using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTrigger : MonoBehaviour {

    EnemyStats.EnemyType enemyType;
    
    HammerBehaviour hammerBehaviour;
    AudioPlayer playerAudioPlayer;
    TimeManager timeManager;
    CameraBehaviour cameraBehaviour;

    void Start()
    {
        hammerBehaviour = GetComponentInParent<HammerBehaviour>();
        playerAudioPlayer = GetComponentInParent<AudioPlayer>();
        cameraBehaviour = GameObject.FindWithTag("CameraController").GetComponent<CameraBehaviour>();
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

            if (hammerBehaviour.GetIsThrowing())
            {
                other.gameObject.GetComponent<EnemyStats>().SetDamage(hammerBehaviour.ThrowHammerDamage(), true, Quaternion.Euler(new Vector3(0, desiredAngle, 0)));
                playerAudioPlayer.PlaySFX(Random.Range(2, 4), 0.5f, Random.Range(0.96f, 1.04f), gameObject);   //Hammer Impact sound

                cameraBehaviour.CameraMoveTowards(0.25f, transform.position, other.transform.position);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyStats>().SetDamage(hammerBehaviour.ThrowHammerDamage(), false);
        }
    }
}
