
using System.Collections.Generic;

public class HealTower : BaseStructure
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
}
