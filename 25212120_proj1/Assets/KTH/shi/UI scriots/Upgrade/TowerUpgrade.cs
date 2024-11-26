using Photon.Chat.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TowerUpgrade : MonoBehaviour
{
    [Header("UI Elements")]
    public Button upgradeButton;             // 업그레이드 버튼
    public TextMeshProUGUI towerpreLevel; 
    public TextMeshProUGUI towerLevelText;              // 타워 레벨 표시
    public TextMeshProUGUI resourceRequirementsText;    // 업그레이드에 필요한 자원 표시

    private BaseStructure baseStructure;     // 현재 선택된 타워

    void Start()
    {
        // 업그레이드 버튼에 리스너 추가
        upgradeButton.onClick.AddListener(UpgradeTower);
    }

    void Update()
    {
        if (baseStructure != null)
        {
            // 타워 정보 업데이트
            UpdateUI();
        }
    }

    // 타워 정보를 설정하는 함수
    public void SetSelectedTower(BaseStructure tower)
    {
        baseStructure = tower;
        UpdateUI(); // UI를 초기화
    }

    // 타워 정보를 기반으로 UI 업데이트
    private void UpdateUI()
    {
        if (baseStructure != null)
        {
            towerpreLevel.text = $"Tower Level: {baseStructure.TowerLevel - 1}";
            // 타워 레벨 표시
            towerLevelText.text = $"Tower Level: {baseStructure.TowerLevel}";

            // 업그레이드에 필요한 자원 표시
            resourceRequirementsText.text = GetResourceRequirementsText(baseStructure.upgradeRequirements);
        }
    }

    // 업그레이드에 필요한 자원 정보를 텍스트로 변환
    private string GetResourceRequirementsText(List<BaseStructure.ResourceRequirement> requirements)
    {
        string result = "Upgrade Requirements:\n";
        foreach (var req in requirements)
        {
            result += $"{req.resourceType}: {req.amount}\n";
        }
        return result;
    }

    // 업그레이드 버튼 동작
    private void UpgradeTower()
    {
        if (baseStructure != null)
        {
            // 업그레이드 시도
            bool success = baseStructure.UpgradeWithoutEssence();
            if (success)
            {
                Debug.Log("Upgrade successful!");
                UpdateUI(); // 업그레이드 후 UI 갱신
            }
            else
            {
                Debug.LogWarning("Upgrade failed. Not enough resources.");
            }
        }
    }
}
