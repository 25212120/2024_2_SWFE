using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 컴포넌트를 사용하기 위한 네임스페이스 추가

public class UIPanelSwitcher : MonoBehaviour
{
    public Button switchButton; // UI 버튼 연결
    public GameObject currentPanel; // 현재 활성화된 UI 패널
    public GameObject nextPanel; // 새로 활성화할 UI 패널

    void Start()
    {
        // 버튼에 리스너 추가
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(SwitchPanel);
        }
        else
        {
            Debug.LogError("Switch button not assigned in the inspector.");
        }
    }

    void SwitchPanel()
    {
        // 현재 패널을 비활성화하고, 다음 패널을 활성화
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
        }

        if (nextPanel != null)
        {
            nextPanel.SetActive(true);
        }
    }
}
