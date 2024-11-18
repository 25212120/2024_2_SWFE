using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    void OnMouseDown()
    {
        if (CompareTag("tower"))  // 특정 태그 확인
        {
            if (popupPanel != null)
            {
                popupPanel.SetActive(true);  // 팝업 창 활성화
            }
        }
    }
}
