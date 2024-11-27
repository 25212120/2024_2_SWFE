using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UImanager : MonoBehaviour
{
    public static UImanager Instance;
    public GameObject upgradeUI; // 중앙 업그레이드 UI
    public TextMeshProUGUI towerLevelText;
    public TextMeshProUGUI nextLevelText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenUpgradeUI(BaseStructure selectedBaseStructure)
    {
        if (selectedBaseStructure != null)
        {
            // UI 업데이트
            towerLevelText.text = $"Current Level: {selectedBaseStructure.TowerLevel}";
            nextLevelText.text = $"Next Level: {selectedBaseStructure.TowerLevel + 1}";
            upgradeUI.SetActive(true);
        }
    }

    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
    }
}
