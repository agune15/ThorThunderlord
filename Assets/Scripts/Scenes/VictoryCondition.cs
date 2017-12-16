using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryCondition : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject victory;
    public int thorDie;

    // Use this for initialization
    void Start()
    {
        thorDie = PlayerPrefs.GetInt("ThorDie");
        if (thorDie == 1) GameOver();
        else Victory();
    }

    void GameOver()
    {
        gameOver.SetActive(true);
    }

    void Victory()
    {
        victory.SetActive(true);
    }
}