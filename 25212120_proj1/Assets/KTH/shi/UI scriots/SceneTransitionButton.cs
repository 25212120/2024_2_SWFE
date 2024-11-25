using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionButton : MonoBehaviour
{
    // 버튼에 할당할 변수
    public Button transitionButton;
    // 전환할 씬 이름
    public string sceneName;

    void Start()
    {
        // 버튼에 리스너 추가
        if (transitionButton != null)
        {
            transitionButton.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked()
    {
        // 씬 전환
        SceneManager.LoadScene(sceneName);
    }
}
