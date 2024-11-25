using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ������Ʈ�� ����ϱ� ���� ���ӽ����̽� �߰�

public class UIPanelSwitcher : MonoBehaviour
{
    public Button switchButton; // UI ��ư ����
    public GameObject currentPanel; // ���� Ȱ��ȭ�� UI �г�
    public GameObject nextPanel; // ���� Ȱ��ȭ�� UI �г�

    void Start()
    {
        // ��ư�� ������ �߰�
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
        // ���� �г��� ��Ȱ��ȭ�ϰ�, ���� �г��� Ȱ��ȭ
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
