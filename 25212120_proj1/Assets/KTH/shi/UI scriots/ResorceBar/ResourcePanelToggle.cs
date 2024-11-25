using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourcePanelToggle : MonoBehaviour
{
    public GameObject resourcePanel; // UI �г� ����

    // Start���� �г��� ó���� ��Ȱ��ȭ
    void Start()
    {
        resourcePanel.SetActive(false);
    }

    // Update�� �� ������ ȣ��˴ϴ�.
    void Update()
    {
        // �� Ű�� ���� ���� �г��� ���̰� ����
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
