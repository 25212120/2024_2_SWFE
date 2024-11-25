using UnityEngine;
using UnityEngine.EventSystems;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            popupPanel.SetActive(false);
        }

        // ���콺 ���� ��ư Ŭ�� ����
        if (Input.GetKeyDown(KeyCode.F))
        {
            // ���� UI ��� ���� Ŭ���� ����� (UI�� Ŭ���Ǿ����� Ȯ��)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;  // UI�� Ŭ���� ���� �ƹ� �۾��� ���� ����
            }

            // Raycast�� Ŭ���� ������Ʈ ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ���� Ÿ�� ������Ʈ�� Ŭ���ߴٸ�
                if (hit.collider.CompareTag("tower"))
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
