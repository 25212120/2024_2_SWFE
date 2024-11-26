using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerUpgrade1 : MonoBehaviour
{
    public Button upgrade;
    private BaseStructure baseStructure;
    private ImageSelector2 index;
    private int indexnum = -1;

    void Upgrade()
    {
        // ImageSelector2 컴포넌트를 가져옵니다.
        index = GetComponent<ImageSelector2>();
        
        // Button 컴포넌트를 가져옵니다.
        upgrade = GetComponent<Button>();


        // upgrade 버튼의 클릭 이벤트 리스너를 등록합니다.
        
        
            upgrade.onClick.AddListener(() => TryUpgrade());
        
        
    }

    private void TryUpgrade()
    {
        if (!baseStructure.Upgradecheck())
        {
            EssenceUpgrade();
        }
        else
        {
            Debug.LogWarning("Esssence used");
        }
    }

       private void EssenceUpgrade()
    {
        if (index != null)
        {
            if (index.SelectedIndex() == 0)
            {
                baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.WoodEssence);
            }
            else if (index.SelectedIndex() == 1)
            {
                baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.IceEssence);
            }
            else if (index.SelectedIndex() == 2)
            {
                baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.SandEssence);
            }
            else
            {
                baseStructure.UpgradeWithEssence(MaterialManager.ResourceType.FireEssence);
            }
        }
        else
        {
            Debug.LogWarning("ImageSelector2 component not found on this GameObject.");
        }

        Debug.LogWarning("업그레이드 완료");
    }
}
