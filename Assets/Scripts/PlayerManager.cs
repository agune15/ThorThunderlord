using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Keeps track of the player */

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    private LevelLogic managerScenes;
    public GameObject sceneLevelManager;

    #region Singleton

    public static PlayerManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public void Start()
    {
        sceneLevelManager = GameObject.FindWithTag("GameManager");
        managerScenes = sceneLevelManager.GetComponent<LevelLogic>();
    }

    public void KillPlayer()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        managerScenes.LoadEndScene();
        PlayerPrefs.SetInt("ThorDie", 1);
    }

}