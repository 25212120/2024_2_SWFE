using UnityEngine;
using UnityEngine.EventSystems;

public class Towerpopup : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject popupPanel;  // �˾� â UI (Inspector���� �Ҵ�)

    [Header("Raycast Settings")]
    [Tooltip("Ž���� �±׸� �����մϴ�.")]
    public string towerTag = "tower";  // Ž���� Ÿ�� �±� (Inspector���� ���� ����)

    void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);  // ���� �� �˾� â�� ��Ȱ��ȭ
        }
    }

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� ����
        if (Input.GetMouseButtonDown(0))
        {
            // UI ��� ���� Ŭ���� ����� (UI�� Ŭ���Ǿ����� Ȯ��)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;  // UI�� Ŭ���� ���� �ƹ� �۾��� ���� ����
            }

            // Raycast�� Ŭ���� ������Ʈ ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Inspector���� ������ Ÿ�� �±׸� ���� ������Ʈ�� Ŭ���ߴ��� Ȯ��
                if (hit.collider.CompareTag(towerTag))
                {
                    // �˾� �г� Ȱ��ȭ
                    if (popupPanel != null)
                    {
                        popupPanel.SetActive(true);
                    }
                }
                // �ٸ� ������Ʈ�� Ŭ���ߴٸ� �˾� �г� ��Ȱ��ȭ
                else
                {
                    if (popupPanel != null && popupPanel.activeSelf)
                    {
                        popupPanel.SetActive(false);
                    }
                }
            }
            // �ƹ��͵� Ŭ������ ���� ��� (�� ���� Ŭ�� ��)
            else
            {
                if (popupPanel != null && popupPanel.activeSelf)
                {
                    popupPanel.SetActive(false);
                }
            }
        }
    }
}
