using UnityEngine;
using UnityEngine.UI;

public class TowerUpgrade1 : MonoBehaviour
{
    private GameObject upgradeUI;
    private BaseStructure baseStructure;  // Ÿ���� BaseStructure ����
    private ImageSelector2 index;         // Essence ������ ���� ������Ʈ
    public Button upgradeButton;          // ���׷��̵� ��ư (Inspector���� ����)

    void Awake()
    {

        if (upgradeUI == null)
        {
            upgradeUI = GameObject.Find("TowerUpgrade");
        }

        if (upgradeButton == null)
        {
            upgradeButton = upgradeUI.transform.Find("UpgradeButton2").GetComponent<Button>();
        }

        
    }
    void OnMouseDown()
    {
        // Ŭ���� Ÿ���� BaseStructure�� ������
        baseStructure = GetComponent<BaseStructure>();

        if (baseStructure != null)
        {
            Debug.Log($"Clicked on: {baseStructure.GetType().Name}");
            // ���׷��̵� ��ư Ȱ��ȭ �Ǵ� ���׷��̵� ���� �۾� ���� ����
            SetupUpgradeButton();
        }
        else
        {
            Debug.LogWarning("This object is not a valid tower.");
        }
    }

    private void SetupUpgradeButton()
    {
        if (upgradeButton != null)
        {
            // ���� ������ ���� �� �� ������ ��� (�ߺ� ����)
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() => TryUpgrade());
        }
        else
        {
            Debug.LogWarning("Upgrade button is not assigned in the Inspector.");
        }
    }

    private void TryUpgrade()
    {
        if (baseStructure == null)
        {
            Debug.LogWarning("BaseStructure not found. Cannot upgrade.");
            return;
        }

        // ���׷��̵� ���� ���� Ȯ��
        if (!baseStructure.Upgradecheck())
        {
            Debug.Log("No essence required for upgrade.");
            EssenceUpgrade();
        }
        else
        {
            Debug.LogWarning("Essence required. Proceeding with Essence upgrade.");
        }
    }

    private void EssenceUpgrade()
    {
        // ImageSelector2 ������Ʈ�� �����ɴϴ�.
        if (index == null)
        {
            index = GetComponent<ImageSelector2>();
        }

        if (index != null)
        {
            // ���õ� Essence�� ���� ���׷��̵�
            switch (index.SelectedIndex())
            {
                case 0:
                    baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.WoodEssence);
                    break;
                case 1:
                    baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.IceEssence);
                    break;
                case 2:
                    baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.SandEssence);
                    break;
                case 3:
                    baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.FireEssence);
                    break;
                default:
                    Debug.LogWarning("Invalid Essence selected.");
                    break;
            }

            Debug.Log("Upgrade completed.");
        }
        else
        {
            Debug.LogWarning("ImageSelector2 component not found on this GameObject.");
        }
    }
}
