using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public Transform canvas;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
                if (canvas.gameObject.activeInHierarchy == false)
            {
                canvas.gameObject.SetActive(true);
                Time.timeScale = 0;
                //Player.GetComponent<FirstPersonController>().enabled = false;
            }
        else
            {
                canvas.gameObject.SetActive(false);
                Time.timeScale = 1;
                //Player.GetComponent<FirstPersonController>().enabled = true;
            }
            //Pause();
        }
    }
    /*public void Pause()
    {

    }*/
}
