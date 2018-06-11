using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    string characterType;
    [SerializeField] List<string> targetsInRange = new List<string>();

    private void Start()
    {
        characterType = transform.parent.tag;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (characterType == "Player")
        {
            if (other.tag == "Enemy")
            {
                if (!targetsInRange.Contains(other.name)) targetsInRange.Add(other.name);
            }
        }
        else if (characterType == "Enemy")
        {
            if (other.tag == "Player")
            {
                if (!targetsInRange.Contains(other.name)) targetsInRange.Add(other.name);
            }
        }
    }

    private void OnTriggerExit (Collider other)
    {
        if (characterType == "Player")
        {
            if (other.tag == "Enemy")
            {
                if (targetsInRange.Contains(other.name)) targetsInRange.Remove(other.name);
            }
        }
        else if (characterType == "Enemy")
        {
            if (other.tag == "Player")
            {
                if (targetsInRange.Contains(other.name)) targetsInRange.Remove(other.name);
            }
        }
    }

    public bool TargetIsInRange (string targetName)
    {
        return targetsInRange.Contains(targetName);
    }
}
