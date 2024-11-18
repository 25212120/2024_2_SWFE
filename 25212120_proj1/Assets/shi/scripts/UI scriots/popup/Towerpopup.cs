using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Towerpopup : MonoBehaviour
{
    public GameObject popupPanel;  // �˾� â UI (Inspector���� �Ҵ�)

    void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);  // ���� �� �˾� â�� ��Ȱ��ȭ
        }
    }

    void OnMouseDown()
    {
        if (CompareTag("tower"))  // Ư�� �±� Ȯ��
        {
            if (popupPanel != null)
            {
                popupPanel.SetActive(true);  // �˾� â Ȱ��ȭ
            }
        }
    }
}
