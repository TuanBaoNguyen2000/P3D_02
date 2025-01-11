using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private Text tipsText;

    [SerializeField] private string[] tips;


    private void Start()
    {
        if (tips != null && tips.Length > 0)
        {
            tipsText.text = tips[Random.Range(0, tips.Length)];
        }

        StartCoroutine(LoadSceneAsync()); 
    }

    private IEnumerator LoadSceneAsync()
    {
        string sceneName = SceneLoader.TargetScene;
        if (string.IsNullOrEmpty(sceneName)) sceneName = "MainHomeScene";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = (progress * 100).ToString("F0") + "%";

            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
