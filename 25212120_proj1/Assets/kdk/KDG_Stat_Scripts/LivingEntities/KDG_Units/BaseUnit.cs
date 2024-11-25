using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public abstract class BaseUnit : BaseEntity
{
    [Header("���׷��̵忡 �ʿ��� �ڿ�")]
    [SerializeField] public List<ResourceRequirement> upgradeRequirements = new List<ResourceRequirement>(); // ���׷��̵忡 �ʿ��� �ڿ� ����Ʈ

    [Header("���� ����")]
    [SerializeField] public int UnitLevel = 1;

    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType;  // �ڿ� Ÿ��
        public int amount;                                 // �ڿ���

        // �� ���� ���ڸ� �޴� ������ �߰�
        public ResourceRequirement(MaterialManager.ResourceType resourceType, int amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }
    }
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
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 1);
        UnitLevel++;
        Debug.Log("���׷��̵� �Ϸ�: ���ݷ� +5, ü�� +50");
    }
    public void UpdateAllUnitsStats(List<BaseUnit> units)
    {
        // ���޹��� ����Ʈ�� �ִ� �� ���ֿ� ���� ������ ������Ʈ
        foreach (BaseUnit unit in units)
        {
            if (unit is Unit_Test)
            {
                unit.PerformUpgrade();  // PerformUpgrade�� ȣ���Ͽ� �ش� ������ ������ ������Ʈ
            }
        }
    }
    public void Heal(float amount)
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
}
