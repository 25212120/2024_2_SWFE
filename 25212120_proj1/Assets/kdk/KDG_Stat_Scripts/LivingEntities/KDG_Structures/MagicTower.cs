
using System.Collections.Generic;

public class MagicTower : BaseStructure
{
    protected override void Awake()
    {
        base.Awake();

        List<ResourceRequirement> requirements = new List<ResourceRequirement>
        {
            new ResourceRequirement(MaterialManager.ResourceType.Money, 100)
        
        };

        // 유닛의 업그레이드 자원 설정
        upgradeRequirements = requirements;
    }
    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }
}
