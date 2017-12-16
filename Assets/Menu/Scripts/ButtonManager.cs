using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    private LevelLogic managerScenes;
    public GameObject sceneLevelManager;

    private void Start()
    {
        sceneLevelManager = GameObject.FindWithTag("GameManager");
        managerScenes = sceneLevelManager.GetComponent<LevelLogic>();
    }

    public void LoadNewGame()
    {
        managerScenes.StartNewGame();
    }

    public void RetryGame()
    {
        managerScenes.RetryGame();
    }

    public void BackToMenu()
    {
        managerScenes.BackToMenu();
    }

    public void ExitToWin()
    {
        Application.Quit();
    }

}
