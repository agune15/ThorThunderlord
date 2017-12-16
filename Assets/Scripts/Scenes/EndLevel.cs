
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private LevelLogic managerScenes;
    public GameObject sceneLevelManager;

    private void Start()
    {
        sceneLevelManager = GameObject.FindWithTag("GameManager");
        managerScenes = sceneLevelManager.GetComponent<LevelLogic>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Thor")
        {
            managerScenes.LoadEndScene();
        }
    }
}
