using UnityEngine;
using UnityEngine.EventSystems;

public class Towerpopup : MonoBehaviour
{
    public GameObject popupPanel;  // 팝업 창 UI (Inspector에서 할당)

    void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);  // 시작 시 팝업 창을 비활성화
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            popupPanel.SetActive(false);
        }

        // 마우스 왼쪽 버튼 클릭 감지
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 만약 UI 요소 위를 클릭한 경우라면 (UI가 클릭되었는지 확인)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;  // UI를 클릭한 경우는 아무 작업도 하지 않음
            }

            // Raycast로 클릭한 오브젝트 감지
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 만약 타워 오브젝트를 클릭했다면
                if (hit.collider.CompareTag("tower"))
                {
                    // 팝업 패널 활성화
                    if (popupPanel != null)
                    {
                        popupPanel.SetActive(true);
                    }
                }
                // 다른 오브젝트를 클릭했다면 팝업 패널 비활성화
                else
                {
                    if (popupPanel != null && popupPanel.activeSelf)
                    {
                        popupPanel.SetActive(false);
                    }
                }
            }
            // 아무것도 클릭하지 않은 경우 (빈 공간 클릭 시)
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
