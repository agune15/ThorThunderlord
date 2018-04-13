using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLogic : MonoBehaviour
{
    InputManager inputManager;

    [Header("Scene state")]
    public int backScene;
    public int currentScene;
    public int nextScene;
    private int managerScene;
    private int sceneCountInBuildSettings;
    //public int gameplayScene = SceneManager.GetSceneByName("Scene_01");

    [Header("Loader")]
    public int sceneToLoad;
    private bool loading = false;
    private AsyncOperation loadAsync = null;
    private AsyncOperation unloadAsync = null;
    private float fadeTime = 1.0f;  

    [Header("UI")]
    [SerializeField] private Canvas managerCanvas;
    public Text percentText;
    public Image blackScreen;

    void Start()
    {
        managerCanvas = GameObject.Find("ManagerCanvas").GetComponent<Canvas>();
        managerCanvas.enabled = true;

        blackScreen = GameObject.Find("blackScreen").GetComponent<Image>();
        percentText = GameObject.Find("percentText").GetComponent<Text>();

        blackScreen.color = Color.black;
        blackScreen.CrossFadeAlpha(0, fadeTime, true);

        if(SceneManager.sceneCount >= 2) SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

        UpdateSceneState();

        if(currentScene == managerScene) StartLoad(nextScene);

        inputManager = this.GetComponent<InputManager>();
        if (inputManager != null)
        {
            inputManager.SetScripts();
            inputManager.ResetStaticVariables();
        }
    }

    void UpdateSceneState()
    {
        sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;

        managerScene = 0;
        currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene <= managerScene + 1) backScene = sceneCountInBuildSettings - 1;
        else backScene = currentScene - 1;

        if (currentScene >= sceneCountInBuildSettings - 1) nextScene = 1;
        else nextScene = currentScene + 1;
    }

    public void StartLoad(int index)
    {
        if (loading) return;
        loading = true;
        sceneToLoad = index;
        managerCanvas.enabled = true;
        FadeOut();

        if (inputManager != null)
        {
            inputManager.SetScripts();
            inputManager.ResetStaticVariables();
        }
    }

    void Load()
    {
        if (sceneToLoad == -1)
        {
            Application.Quit();
            return;
        }
        //Debug.Log("Unload current scene: " + currentScene);

        if(currentScene != managerScene) unloadAsync = SceneManager.UnloadSceneAsync(currentScene);
        loadAsync = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        StartCoroutine(this.Loading());
    }

    void FadeIn()
    {
        blackScreen.CrossFadeAlpha(0, fadeTime, true);
        StartCoroutine(CanvasUnable());
    }
    void FadeOut()
    {
        blackScreen.CrossFadeAlpha(1.0f, fadeTime, true);
        StartCoroutine(WaitForFade());
    }

    IEnumerator Loading()
    {
        while (loading)
        {
            percentText.text = (loadAsync.progress * 100).ToString() + "%";
            //Debug.Log(loadAsync.progress * 100);
            if ((unloadAsync == null || unloadAsync.isDone) && loadAsync.isDone)
            {
                percentText.text = "";  
                unloadAsync = null;
                loadAsync = null;   

                FadeIn();

                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));
                UpdateSceneState();
                if (inputManager != null)
                {
                    inputManager.SetScripts();
                    inputManager.ResetStaticVariables();
                }

                loading = false;
            }
            yield return null;
        }        
    }

    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(fadeTime);
        Load();
    }

    IEnumerator CanvasUnable()
    {
        yield return new WaitForSeconds(fadeTime);
        managerCanvas.enabled = false;
    }

}
