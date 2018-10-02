using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningTrapBehaviour : MonoBehaviour {

    Animator spinningTrapAnimator;
    AudioPlayer spinningTrapAudioPlayer;

    public float slowTime;
    public float delayTime;
    float currentDelayTime;
    bool dealDamage = true;
    bool slowOn = false;
    bool isSlowing = false;

    //[Header("Sound Parameters")]

    //Lista de sonidos
    //Diccionario de audiosources

    private void Start()
    {
        spinningTrapAnimator = GetComponentInParent<Animator>();
        spinningTrapAudioPlayer = GetComponent<AudioPlayer>();

        currentDelayTime = delayTime;
    }

    private void Update()
    {
        if (!dealDamage && slowOn)
        {
            if (isSlowing)
            {
                if (spinningTrapAnimator.speed > 0)
                {
                    if (spinningTrapAnimator.speed - Time.deltaTime / slowTime < 0) spinningTrapAnimator.speed = 0;
                    else spinningTrapAnimator.speed -= Time.deltaTime / slowTime;
                    return;
                }
                else
                {
                    spinningTrapAnimator.speed = 0;
                    isSlowing = false;
                }
            }
            else
            {
                if (currentDelayTime > 0)
                {
                    currentDelayTime -= Time.deltaTime;
                    return;
                }

                if (spinningTrapAnimator.speed < 1)
                {
                    spinningTrapAnimator.speed += Time.deltaTime / slowTime;
                }
                else
                {
                    spinningTrapAnimator.speed = 1;
                    dealDamage = true;
                    slowOn = false;
                    currentDelayTime = delayTime;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (dealDamage)
            {
                other.GetComponent<CharacterBehaviour>().SetDamage(70, true, Quaternion.Euler(new Vector3(0, Vector3.Angle(transform.forward, transform.InverseTransformPoint(other.transform.position)), 0)));
                dealDamage = false;
                spinningTrapAudioPlayer.PlaySFX(0, 0.5f, Random.Range(0.94f, 1.06f));
                SlowTrap();
            }
        }
    }

    void SlowTrap()
    {
        slowOn = true;
        isSlowing = true;
    }
}