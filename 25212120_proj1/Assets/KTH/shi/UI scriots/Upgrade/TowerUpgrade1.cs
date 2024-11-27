using UnityEngine;
using UnityEngine.UI;

public class TowerUpgrade1 : MonoBehaviour
{
    private GameObject upgradeUI;
    private BaseStructure baseStructure;  // 타워의 BaseStructure 참조
    private ImageSelector2 index;         // Essence 선택을 위한 컴포넌트
    public Button upgradeButton;          // 업그레이드 버튼 (Inspector에서 연결)

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
        // 클릭된 타워의 BaseStructure를 가져옴
        baseStructure = GetComponent<BaseStructure>();

        if (baseStructure != null)
        {
            Debug.Log($"Clicked on: {baseStructure.GetType().Name}");
            // 업그레이드 버튼 활성화 또는 업그레이드 관련 작업 진행 가능
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
            // 기존 리스너 제거 후 새 리스너 등록 (중복 방지)
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

        // 업그레이드 가능 여부 확인
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
        // ImageSelector2 컴포넌트를 가져옵니다.
        if (index == null)
        {
            index = GetComponent<ImageSelector2>();
        }

        if (index != null)
        {
            // 선택된 Essence에 따라 업그레이드
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
