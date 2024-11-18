using System.Collections.Generic;
using UnityEngine;

public class BaseStructure : BaseEntity
{
    [Header("���׷��̵忡 �ʿ��� �ڿ�")]
    [SerializeField] private List<ResourceRequirement> upgradeRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ

    protected override void Awake()
    {
        base.Awake();
    }

    // �ڿ� �Ҹ��Ͽ� ���׷��̵� ����
    public bool Upgrade()
    {
        // ���׷��̵忡 �ʿ��� �ڿ����� ��� �Ҹ��� �� �ִ��� Ȯ��
        foreach (var requirement in upgradeRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // �ڿ��� �����ϸ� ���׷��̵� ����
                Debug.LogWarning($"{requirement.resourceType} �ڿ��� �����մϴ�. ���׷��̵� ����.");
                return false;
            }
        }

        // ��� �ڿ� �Һ� �����ϸ� ���׷��̵� ����
        PerformUpgrade();

        return true;
    }

    // ���׷��̵� ���� (������ ������Ŵ)
    private void PerformUpgrade()
    {
        // ������ ������Ű�� ���� (����: ���ݷ°� ü�� ����)
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, 5); // ����: ���ݷ� +5
        statData.UpgradeBaseStat(StatData.StatType.HP, 50);   // ����: ü�� +50

        Debug.Log("���׷��̵� �Ϸ�: ���ݷ� +5, ü�� +50");
    }

    public virtual void Repair(float amount)
    {
        statData.ModifyCurrentHp(amount);
    }

    public virtual void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();
    }

    // ���׷��̵忡 �ʿ��� �ڿ� Ŭ����
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType; // �ڿ� Ÿ��
        public int amount;                                // �ڿ� ����
    }
}
