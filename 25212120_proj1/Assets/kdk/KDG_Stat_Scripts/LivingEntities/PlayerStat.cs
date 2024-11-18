using UnityEngine;

public class PlayerStat : BaseEntity
{
    [Header("플레이어 장비 인벤토리")]
    public EquipmentInventory equipmentInventory; // EquipmentInventory 컴포넌트 참조


    private void Start()
    {
        equipmentInventory = GetComponent<EquipmentInventory>(); // EquipmentInventory를 Player 오브젝트에 연결

        
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
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);

        // 장비 경험치를 증가시킴 (현재 장비의 경험치에 데미지를 더해줌)
        if (equipmentInventory != null && equipmentInventory.currentEquipment != null)
        {
            // 경험치를 데미지만큼 추가
            equipmentInventory.currentEquipment.AddExperience(damage);

            Debug.Log($"{equipmentInventory.currentEquipment.itemName} 경험치: {equipmentInventory.currentEquipment.experience}");
        }
    }

    public void LevelUp()
    {
        statData.UpgradeBaseStat(StatData.StatType.HP, 1);
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, 1);
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 1); // 추후 세부 조정
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
