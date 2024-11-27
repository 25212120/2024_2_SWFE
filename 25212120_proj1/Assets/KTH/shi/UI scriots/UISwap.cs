using UnityEngine;
using UnityEngine.UI;

public class UISwap : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel1; // 첫 번째 UI 패널
    [SerializeField] private GameObject uiPanel2; // 두 번째 UI 패널
    [SerializeField] private Button button1; // 첫 번째 버튼
    [SerializeField] private Button button2; // 두 번째 버튼

    private void Start()
    {
        // 버튼 클릭 이벤트에 메서드 연결
        button1.onClick.AddListener(() => ActivatePanel(uiPanel1, uiPanel2));
        button2.onClick.AddListener(() => ActivatePanel(uiPanel2, uiPanel1));
    }

    private void ActivatePanel(GameObject panelToActivate, GameObject panelToDeactivate)
    {
        panelToActivate.SetActive(true); // 활성화
        panelToDeactivate.SetActive(false); // 비활성화
    }
}
