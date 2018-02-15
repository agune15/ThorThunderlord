using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullTrigger : MonoBehaviour {

    SkullBehaviour skullBehaviour;

    private void Start()
    {
        skullBehaviour = this.GetComponentInParent<SkullBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            skullBehaviour.SkullHasAttacked();
        }
    }
}
