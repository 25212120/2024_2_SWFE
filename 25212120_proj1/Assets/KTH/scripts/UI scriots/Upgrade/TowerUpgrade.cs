using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgrade : MonoBehaviour
{
    public Button upgrade;
    private BaseStructure baseStructure;
  
    void Update()
    {
        upgrade.onClick.AddListener(() => baseStructure.UpgradeWithoutEssence());
    }
}
