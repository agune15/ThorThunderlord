using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour {

    public ChangeScene sceneChanger;
    public int nextScene;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player") sceneChanger.SetScene(nextScene);
    }
}
