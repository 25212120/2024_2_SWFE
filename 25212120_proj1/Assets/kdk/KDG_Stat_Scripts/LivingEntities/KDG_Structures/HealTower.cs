
using System.Collections.Generic;

public class HealTower : BaseStructure
{
    protected override void Awake()
    {
        base.Awake();

        List<ResourceRequirement> requirements = new List<ResourceRequirement>
        {
            new ResourceRequirement(MaterialManager.ResourceType.Money, 100),  // �����ڿ� �°� ����
            new ResourceRequirement(MaterialManager.ResourceType.Wood, 50),
            new ResourceRequirement(MaterialManager.ResourceType.Stone, 30)
        };

        // ������ ���׷��̵� �ڿ� ����
        upgradeRequirements = requirements;
    }
    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }
}
