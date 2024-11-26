using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TowerUpgrade : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject upgradeUI;                // 업그레이드 UI
    public TextMeshProUGUI towerTypeText;       // 타워 종류 표시 텍스트
    public TextMeshProUGUI towerLevelText;      // 현재 타워 레벨 텍스트
    public TextMeshProUGUI nextLevelText;       // 업그레이드 후 레벨 텍스트
    public Button upgradeButton;               // 업그레이드 버튼

    private BaseStructure selectedBaseStructure; // 현재 선택된 타워의 BaseStructure

    void Awake()
    {
        upgradeUI.SetActive(false); // 게임 시작 시 UI 비활성화
    }

    void Update()
    {
        // ESC 키로 UI 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUpgradeUI();
        }
    }
    
    void OnMouseDown()
    {
        // 클릭된 타워의 BaseStructure를 가져옴
        BaseStructure baseStructure = GetComponent<BaseStructure>();

        if (baseStructure != null)
        {
            Debug.Log($"Clicked on: {baseStructure.GetType().Name}");
            selectedBaseStructure = baseStructure;
            OpenUpgradeUI(); // UI 열기
        }
        else
        {
            Debug.LogWarning("This object is not a valid tower.");
        }
    }

    // 업그레이드 UI 열기
    private void OpenUpgradeUI()
    {
        if (selectedBaseStructure != null)
        {
          

            // 레벨 정보 업데이트
            towerLevelText.text = $"Current Level: {selectedBaseStructure.TowerLevel}";
            nextLevelText.text = $"Next Level: {selectedBaseStructure.TowerLevel + 1}";

            // 업그레이드 버튼 활성화
            upgradeButton.interactable = true;

            // UI 활성화
            upgradeUI.SetActive(true);
        }
    }

    // 업그레이드 버튼 클릭 처리
    public void OnUpgradeButtonClicked()
    {
        if (selectedBaseStructure != null)
        {
            bool success = selectedBaseStructure.UpgradeWithoutEssence();
            if (success)
            {
                Debug.Log($"Tower upgraded to level {selectedBaseStructure.TowerLevel}");
                OpenUpgradeUI(); // 업그레이드 후 UI 갱신
            }
            else
            {
                Debug.LogWarning("Upgrade failed.");
            }
        }
    }

    // 업그레이드 UI 닫기
    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        selectedBaseStructure = null;
    }
}
