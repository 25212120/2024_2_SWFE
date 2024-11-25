using System.Collections.Generic;

public class Unit_WithWand : BaseUnit
{
    protected override void Awake()
    {
        base.Awake();

        List<ResourceRequirement> requirements = new List<ResourceRequirement>
        {
            new ResourceRequirement(MaterialManager.ResourceType.Money, 100)
        };

        // ������ ���׷��̵� �ڿ� ����
        upgradeRequirements = requirements;
    }

    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }

    public float GetCurrentHP()
    {
        return statData.HpCurrent;
    }
}