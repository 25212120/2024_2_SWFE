using UnityEngine;

public class PlayerStat : BaseEntity
{
    [Header("플레이어 장비 인벤토리")]
    public EquipmentInventory equipmentInventory; // EquipmentInventory 컴포넌트 참조
    private PlayerInputManager  playerInputManager;
    public PlayerInventory playerInventory;

    protected override void Awake()
    {
        base.Awake();
        equipmentInventory = GetComponent<EquipmentInventory>(); // EquipmentInventory를 Player 오브젝트에 연결
        playerInventory = GetComponent<PlayerInventory>();
    }
    public int GetLevel()
    {
        return statData.level;
    }
    public void Attack(BaseMonster target)
    {
        Debug.Log("ATTACK");
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // 장비 경험치를 증가시킴 (현재 장비의 경험치에 데미지를 더해줌)
        if (equipmentInventory != null && equipmentInventory.currentEquipment != null)
        {
            // 경험치를 데미지만큼 추가
            equipmentInventory.currentEquipment.AddExperience(damage);

            Debug.Log($"{equipmentInventory.currentEquipment.itemName} 경험치: {equipmentInventory.currentEquipment.experience}");
        }
    }
    // 첫 번째 마법을 사용한 공격
    public void MagicAttack(BaseMonster target, int index)
    {
        Debug.Log("MagicAttack with Magic1");

        PlayerMagic magic = playerInventory.playerMagics[index];

        if (magic != null)
        {
            float damage1 = 0;

            switch (magic.magicType)
            {
                case PlayerMagicType.Wood:
                    damage1 = statData.MagicAttackCurrent_Wood;
                    break;
                case PlayerMagicType.Fire:
                    damage1 = statData.MagicAttackCurrent_Fire;
                    break;
                case PlayerMagicType.Ice:
                    damage1 = statData.MagicAttackCurrent_Ice;
                    break;
                case PlayerMagicType.Sand:
                    damage1 = statData.MagicAttackCurrent_Sand;
                    break;
                default:
                    Debug.LogError("알 수 없는 마법 타입");
                    return; // 알 수 없는 마법 타입 처리
            }

            target.TakeDamage(damage1); 

            magic.AddExperience(damage1);
            Debug.Log($"{magic.magicType} 마법의 경험치: {magic.experience}");
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
    
    public float GetMaxHp()
    {
        return statData.hpMax;
    }
}
