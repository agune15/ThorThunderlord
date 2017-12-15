using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    LevelLogic levelLoader;
    public int goToScene;

    private void Start()
    {
        levelLoader = GameObject.Find("Managers").GetComponent<LevelLogic>();
    }

    public void NewGameBtn(string newGameLevel)
    {
        levelLoader.StartLoad(goToScene);

    }

}
