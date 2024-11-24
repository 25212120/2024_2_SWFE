using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionButton : MonoBehaviour
{
    // ��ư�� �Ҵ��� ����
    public Button transitionButton;
    // ��ȯ�� �� �̸�
    public string sceneName;

    void Start()
    {
        // ��ư�� ������ �߰�
        if (transitionButton != null)
        {
            transitionButton.onClick.AddListener(OnButtonClicked);
        }
    }

    void OnButtonClicked()
    {
        // �� ��ȯ
        SceneManager.LoadScene(sceneName);
    }
}
