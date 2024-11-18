using UnityEngine;

public class PlayerStat : BaseEntity
{
    [Header("�÷��̾� ��� �κ��丮")]
    public EquipmentInventory equipmentInventory; // EquipmentInventory ������Ʈ ����


    private void Start()
    {
        equipmentInventory = GetComponent<EquipmentInventory>(); // EquipmentInventory�� Player ������Ʈ�� ����

        
    }
    protected override void Awake()
    {
        base.Awake();
    }
    public int GetLevel()
    {
        return statData.level;
    }
    public void Attack(BaseMonster target)
    {
        Debug.Log("ATTACK");
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);

        // ��� ����ġ�� ������Ŵ (���� ����� ����ġ�� �������� ������)
        if (equipmentInventory != null && equipmentInventory.currentEquipment != null)
        {
            // ����ġ�� ��������ŭ �߰�
            equipmentInventory.currentEquipment.AddExperience(damage);

            Debug.Log($"{equipmentInventory.currentEquipment.itemName} ����ġ: {equipmentInventory.currentEquipment.experience}");
        }
    }

    public void LevelUp()
    {
        statData.UpgradeBaseStat(StatData.StatType.HP, 1);
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, 1);
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 1); // ���� ���� ����
        statData.UpgradeBaseStat(StatData.StatType.LEVEL, 1);
    }
    public void Heal(float healAmount)
    {
        statData.ModifyCurrentHp(healAmount);
    }

    public float GetCurrentHP()
    {
        return statData.HpCurrent;
    }
}
