using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour {

    ChangeScene sceneChanger;
    MusicAmbientController musicController;
    public int nextScene;
    public bool fadeOutMusic;

    private void Start()
    {
        sceneChanger = GameObject.Find("GameplayUI").GetComponent<ChangeScene>();
        musicController = GameObject.FindWithTag("MusicAmbientController").GetComponent<MusicAmbientController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.gameObject.GetComponent<CharacterBehaviour>().isBeingAttacked) sceneChanger.SetScene(nextScene);
            if (fadeOutMusic) musicController.SetMusicType(MusicAmbientController.MusicTypes.None, 0.8f, 0);
        }
    }
}
