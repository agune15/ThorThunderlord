using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour {

    LevelLogic levelLogic;

    private void Start()
    {
        levelLogic = GameObject.FindWithTag("manager").GetComponent<LevelLogic>();
    }

    public void SetScene(int scene)
    {
        levelLogic.StartLoad(scene);
    }
}
