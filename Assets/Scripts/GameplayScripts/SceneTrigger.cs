using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour {

    ChangeScene sceneChanger;
    public int nextScene;

    private void Start()
    {
        sceneChanger = GameObject.Find("GameplayUI").GetComponent<ChangeScene>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.gameObject.GetComponent<CharacterBehaviour>().isBeingAttacked) sceneChanger.SetScene(nextScene);
        }
    }
}
