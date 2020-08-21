using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static bool LoadingLevel { get; private set; }

    public static int LevelIndex { get; private set; }

    public RawImage blackFade;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LevelIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private AsyncOperation _loadingLevelAsyncOperation;
    public static void LoadNextLevel()
    {
        if (LoadingLevel)
            return;

        int index = SceneManager.GetActiveScene().buildIndex;

        LevelIndex = (index + 1) % SceneManager.sceneCountInBuildSettings;

        instance.StartCoroutine(TransitionToNextLevel());
    }

    public static void ReloadLevel()
    {
        if (LoadingLevel)
            return;

        instance.StartCoroutine(TransitionToNextLevel());
    }

    private static IEnumerator TransitionToNextLevel()
    {
        LoadingLevel = true;

        const float fadeTime = 0.25f;
        const float minWaitTime = 0.5f;
        
        Color fadeColor = instance.blackFade.color;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
         
            float p = Mathf.Clamp01(timer / fadeTime);

            fadeColor.a = p;
            instance.blackFade.color = fadeColor;

            yield return null;
        }

        instance._loadingLevelAsyncOperation = SceneManager.LoadSceneAsync(LevelIndex);

        yield return new WaitForSeconds(minWaitTime);

        while (!instance._loadingLevelAsyncOperation.isDone)
            yield return null;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float p = Mathf.Clamp01(timer / fadeTime);
            
            fadeColor.a = p;
            instance.blackFade.color = fadeColor;

            yield return null;
        }

        LoadingLevel = false;
    }
}
