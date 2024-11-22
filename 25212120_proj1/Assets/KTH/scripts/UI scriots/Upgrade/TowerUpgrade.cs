using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgrade : MonoBehaviour
{
    private Button upgrade;
    private BaseStructure baseStructure;
  

    void Start()
    {
        baseStructure.UpgradeWithoutEssence();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
