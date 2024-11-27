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
        if (!this.enabled) // 만약 이 스크립트가 비활성화 되어 있다면
        {
            this.enabled = true; // 스크립트를 다시 활성화
        }


        if (upgradeUI == null)
        {
            upgradeUI = GameObject.Find("TowerUpgrade1");
        }

        if (towerTypeText == null)
        {
            towerTypeText = upgradeUI.transform.Find("Text (TMP) (4)").GetComponent<TextMeshProUGUI>();
        }
        if (towerLevelText == null)
        {
            towerLevelText = upgradeUI.transform.Find("PreLevel").GetComponent<TextMeshProUGUI>();
        }
        if (nextLevelText == null)
        {
            nextLevelText = upgradeUI.transform.Find("NextLevel").GetComponent<TextMeshProUGUI>();
        }
        if (upgradeButton == null)
        {
            upgradeButton = upgradeUI.transform.Find("UpgradeButton1").GetComponent<Button>();
        }

        upgradeUI.SetActive(false); // 초기 비활성화
    }
    void Start()
    {
        // 게임 시작 시 스크립트가 비활성화된 경우 자동으로 활성화
        if (!this.enabled)
        {
            this.enabled = true;
        }
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
        Debug.Log("클릭은 됨.");
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
