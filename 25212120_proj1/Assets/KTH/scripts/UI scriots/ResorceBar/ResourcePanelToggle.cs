using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourcePanelToggle : MonoBehaviour
{
    public GameObject resourcePanel; // UI 패널 참조

    // Start에서 패널을 처음에 비활성화
    void Start()
    {
        resourcePanel.SetActive(false);
    }

    // Update는 매 프레임 호출됩니다.
    void Update()
    {
        // 탭 키가 눌린 동안 패널을 보이게 설정
        if (Input.GetKey(KeyCode.Tab))
        {
            Debug.Log("Tab key pressed - Showing Resource Panel");
            resourcePanel.SetActive(true);
        }
        else
        {
            Debug.Log("Tab key released - Hiding Resource Panel");
            resourcePanel.SetActive(false);
        }
    }
}
