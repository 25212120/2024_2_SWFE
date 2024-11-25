
using System.Collections.Generic;
using UnityEngine;

public class ArrowTower : BaseStructure
{
    protected override void Awake()
    {
        base.Awake();

        List<ResourceRequirement> requirements = new List<ResourceRequirement>
        {
            new ResourceRequirement(MaterialManager.ResourceType.Money, 100),  // 생성자에 맞게 수정
            new ResourceRequirement(MaterialManager.ResourceType.Wood, 50),
            new ResourceRequirement(MaterialManager.ResourceType.Stone, 30)
        };

        // 유닛의 업그레이드 자원 설정
        upgradeRequirements = requirements;
    }
    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }

    protected override void Update()
    {
        Debug.Log("화살 업글 준비중");
        base.Update();
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.WoodEssence);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.SandEssence);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.FireEssence);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            UpgradeWithEssence(MaterialManager.ResourceType.IceEssence);
        }
    }
}
