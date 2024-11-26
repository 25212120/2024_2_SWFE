using Photon.Chat.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TowerUpgrade : MonoBehaviour
{
    [Header("UI Elements")]
    public Button upgradeButton;             // ���׷��̵� ��ư
    public TextMeshProUGUI towerpreLevel; 
    public TextMeshProUGUI towerLevelText;              // Ÿ�� ���� ǥ��
    public TextMeshProUGUI resourceRequirementsText;    // ���׷��̵忡 �ʿ��� �ڿ� ǥ��

    private BaseStructure baseStructure;     // ���� ���õ� Ÿ��

    void Start()
    {
        // ���׷��̵� ��ư�� ������ �߰�
        upgradeButton.onClick.AddListener(UpgradeTower);
    }

    void Update()
    {
        if (baseStructure != null)
        {
            // Ÿ�� ���� ������Ʈ
            UpdateUI();
        }
    }

    // Ÿ�� ������ �����ϴ� �Լ�
    public void SetSelectedTower(BaseStructure tower)
    {
        baseStructure = tower;
        UpdateUI(); // UI�� �ʱ�ȭ
    }

    // Ÿ�� ������ ������� UI ������Ʈ
    private void UpdateUI()
    {
        if (baseStructure != null)
        {
            towerpreLevel.text = $"Tower Level: {baseStructure.TowerLevel - 1}";
            // Ÿ�� ���� ǥ��
            towerLevelText.text = $"Tower Level: {baseStructure.TowerLevel}";

            // ���׷��̵忡 �ʿ��� �ڿ� ǥ��
            resourceRequirementsText.text = GetResourceRequirementsText(baseStructure.upgradeRequirements);
        }
    }

    // ���׷��̵忡 �ʿ��� �ڿ� ������ �ؽ�Ʈ�� ��ȯ
    private string GetResourceRequirementsText(List<BaseStructure.ResourceRequirement> requirements)
    {
        string result = "Upgrade Requirements:\n";
        foreach (var req in requirements)
        {
            result += $"{req.resourceType}: {req.amount}\n";
        }
        return result;
    }

    // ���׷��̵� ��ư ����
    private void UpgradeTower()
    {
        if (baseStructure != null)
        {
            // ���׷��̵� �õ�
            bool success = baseStructure.UpgradeWithoutEssence();
            if (success)
            {
                Debug.Log("Upgrade successful!");
                UpdateUI(); // ���׷��̵� �� UI ����
            }
            else
            {
                Debug.LogWarning("Upgrade failed. Not enough resources.");
            }
        }
    }
}
