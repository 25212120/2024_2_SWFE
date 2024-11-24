using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider loadingBar; // 로딩 바 UI (Slider)
    [Header("Scene Settings")]
    public string nextSceneName; // 다음 씬 이름을 인스펙터에서 설정

    void Start()
    {
        // 새로운 씬을 비동기적으로 로드
        StartCoroutine(LoadSceneAsync(nextSceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // 비동기적으로 씬 로드 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // 씬이 로드되는 동안 로딩 바를 업데이트
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar != null)
            {
                loadingBar.value = progress; // 로딩 바 값 설정
            }
            yield return null; // 다음 프레임까지 대기
        }
    }
}
