using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider loadingBar; // �ε� �� UI (Slider)
    [Header("Scene Settings")]
    public string nextSceneName; // ���� �� �̸��� �ν����Ϳ��� ����

    void Start()
    {
        // ���ο� ���� �񵿱������� �ε�
        StartCoroutine(LoadSceneAsync(nextSceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // �񵿱������� �� �ε� ����
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // ���� �ε�Ǵ� ���� �ε� �ٸ� ������Ʈ
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingBar != null)
            {
                loadingBar.value = progress; // �ε� �� �� ����
            }
            yield return null; // ���� �����ӱ��� ���
        }
    }
}
