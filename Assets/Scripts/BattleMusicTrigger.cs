using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMusicTrigger : MonoBehaviour
{

    public AudioSource BattleMusic;

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Thor")
        {
           BattleMusic.Play();
        }
    }


}