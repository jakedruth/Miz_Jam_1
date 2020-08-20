using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static int LevelIndex { get; private set; } = 0;
    public static int MaxLevels { get; private set; }

    private RawImage _blackFade;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        MaxLevels = SceneManager.sceneCountInBuildSettings;
        _blackFade = transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
    }

    private AsyncOperation _loadingLevelAsyncOperation;

    [ContextMenu("Load Next Level")]
    public void LoadNextLevel()
    {
        LevelIndex = (LevelIndex + 1) % MaxLevels;
        StartCoroutine(TransitionToNextLevel());
    }

    private IEnumerator TransitionToNextLevel()
    {
        const float fadeTime = 0.25f;
        const float minWaitTime = 0.5f;
        
        Color fadeColor = _blackFade.color;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
         
            float p = Mathf.Clamp01(timer / fadeTime);

            fadeColor.a = p;
            _blackFade.color = fadeColor;

            yield return null;
        }

        _loadingLevelAsyncOperation = SceneManager.LoadSceneAsync(LevelIndex);

        yield return new WaitForSeconds(minWaitTime);

        while (!_loadingLevelAsyncOperation.isDone)
            yield return null;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float p = Mathf.Clamp01(timer / fadeTime);
            
            fadeColor.a = p;
            _blackFade.color = fadeColor;

            yield return null;
        }
    }
}
