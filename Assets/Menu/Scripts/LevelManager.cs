using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("Scene state")]
    public int backScene;
    public int currentScene;
    public int nextScene;
    private int managerScene = 0;
    private int titleScene = 1;
    private int sceneCountInBuildSettings;
    [Header("Load parameters")]
    private int sceneToLoad;
    private AsyncOperation loadAsync = null;
    private AsyncOperation unloadAsync = null;
    private bool loading = false;
    [Header("UI")]
    public Image blackScreen;
    private float fadeTime = 1.0f;

    void Start ()
    {
        blackScreen.color = Color.black;

        if(SceneManager.sceneCount >= 2) SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

        UpdateSceneState();

        if(currentScene == managerScene) StartLoad(nextScene);
    }

    private void Update()
    {
        //INPUT MANAGER.
        if(Input.GetKey(KeyCode.AltGr))
        {
            if(Input.GetKeyDown(KeyCode.N)) StartLoad(nextScene);
            if(Input.GetKeyDown(KeyCode.B)) StartLoad(backScene);
            if(Input.GetKeyDown(KeyCode.R)) StartLoad(currentScene);
            if(Input.GetKeyDown(KeyCode.M)) StartLoad(titleScene);
        }
    }

    void UpdateSceneState()
    {
        sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;

        currentScene = SceneManager.GetActiveScene().buildIndex;

        if(currentScene + 1 >= sceneCountInBuildSettings) nextScene = managerScene + 1;
        else nextScene = currentScene + 1;

        if(currentScene - 1 <= managerScene) backScene = sceneCountInBuildSettings - 1;
        else backScene = currentScene - 1;
    }

    void StartLoad(int index)
    {
        if(loading) return;
        loading = true;

        sceneToLoad = index;
        FadeOut();
    }
    void Load()
    {
        if(sceneToLoad == -1)
        {
            Application.Quit();
            return;
        }

        if(currentScene != managerScene) unloadAsync = SceneManager.UnloadSceneAsync(currentScene);
        loadAsync = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        StartCoroutine(Loading());
    }

    void FadeIn()
    {
        blackScreen.CrossFadeAlpha(0, fadeTime, true);
    }
    void FadeOut()
    {
        blackScreen.CrossFadeAlpha(1, fadeTime, true);
        StartCoroutine(WaitForFade());
    }

    IEnumerator Loading()
    {
        while(loading)
        {
            Debug.Log(loadAsync.progress);
            if(loadAsync.isDone && (unloadAsync == null || unloadAsync.isDone))
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));

                UpdateSceneState();
                FadeIn();
                loading = false;
            }
            yield return null;
        }
    }
    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(fadeTime);
        //Empezar la descarga y carga
        Load();
    }


}
