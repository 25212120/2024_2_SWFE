using System.Collections.Generic;

public class Wall_1 : BaseStructure
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
}