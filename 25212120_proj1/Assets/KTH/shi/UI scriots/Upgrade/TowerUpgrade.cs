using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TowerUpgrade : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject upgradeUI;                // ���׷��̵� UI
    public TextMeshProUGUI towerTypeText;       // Ÿ�� ���� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI towerLevelText;      // ���� Ÿ�� ���� �ؽ�Ʈ
    public TextMeshProUGUI nextLevelText;       // ���׷��̵� �� ���� �ؽ�Ʈ
    public Button upgradeButton;               // ���׷��̵� ��ư

    private BaseStructure selectedBaseStructure; // ���� ���õ� Ÿ���� BaseStructure

    void Awake()
    {
        upgradeUI.SetActive(false); // ���� ���� �� UI ��Ȱ��ȭ
    }

    void Update()
    {
        // ESC Ű�� UI �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUpgradeUI();
        }
    }
    
    void OnMouseDown()
    {
        // Ŭ���� Ÿ���� BaseStructure�� ������
        BaseStructure baseStructure = GetComponent<BaseStructure>();

        if (baseStructure != null)
        {
            Debug.Log($"Clicked on: {baseStructure.GetType().Name}");
            selectedBaseStructure = baseStructure;
            OpenUpgradeUI(); // UI ����
        }
        else
        {
            Debug.LogWarning("This object is not a valid tower.");
        }
    }

    // ���׷��̵� UI ����
    private void OpenUpgradeUI()
    {
        if (selectedBaseStructure != null)
        {
          

            // ���� ���� ������Ʈ
            towerLevelText.text = $"Current Level: {selectedBaseStructure.TowerLevel}";
            nextLevelText.text = $"Next Level: {selectedBaseStructure.TowerLevel + 1}";

            // ���׷��̵� ��ư Ȱ��ȭ
            upgradeButton.interactable = true;

            // UI Ȱ��ȭ
            upgradeUI.SetActive(true);
        }
    }

    // ���׷��̵� ��ư Ŭ�� ó��
    public void OnUpgradeButtonClicked()
    {
        if (selectedBaseStructure != null)
        {
            bool success = selectedBaseStructure.UpgradeWithoutEssence();
            if (success)
            {
                Debug.Log($"Tower upgraded to level {selectedBaseStructure.TowerLevel}");
                OpenUpgradeUI(); // ���׷��̵� �� UI ����
            }
            else
            {
                Debug.LogWarning("Upgrade failed.");
            }
        }
    }

    // ���׷��̵� UI �ݱ�
    public void CloseUpgradeUI()
    {
        upgradeUI.SetActive(false);
        selectedBaseStructure = null;
    }
}
