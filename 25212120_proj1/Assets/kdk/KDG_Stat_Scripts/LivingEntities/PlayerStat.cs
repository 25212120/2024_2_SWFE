using UnityEngine;

public class PlayerStat : BaseEntity
{
    [Header("플레이어 장비 인벤토리")]
    public EquipmentInventory equipmentInventory; // EquipmentInventory 컴포넌트 참조
    private PlayerInputManager  playerInputManager;
    public PlayerInventory playerInventory;

    private void Start()
    {
        equipmentInventory = GetComponent<EquipmentInventory>(); // EquipmentInventory를 Player 오브젝트에 연결
        playerInventory = GetComponent<PlayerInventory>();
        
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
    // 첫 번째 마법을 사용한 공격
    public void MagicAttackWithMagic1(BaseMonster target)
    {
        Debug.Log("MagicAttack with Magic1");

        PlayerMagic magic1 = GetEquippedMagic1();

        if (magic1 != null)
        {
            float damage1 = 0;

            // 첫 번째 마법의 데미지 계산
            switch (magic1.magicType)
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

            // 첫 번째 마법의 경험치 증가
            magic1.AddExperience(damage1);
            Debug.Log($"{magic1.magicType} 마법의 경험치: {magic1.experience}");
        }
    }

    // 두 번째 마법을 사용한 공격
    public void MagicAttackWithMagic2(BaseMonster target)
    {
        Debug.Log("MagicAttack with Magic2");

        // 두 번째 마법 가져오기
        PlayerMagic magic2 = GetEquippedMagic2();

        if (magic2 != null)
        {
            float damage2 = 0;

            // 두 번째 마법의 데미지 계산
            switch (magic2.magicType)
            {
                case PlayerMagicType.Wood:
                    damage2 = statData.MagicAttackCurrent_Wood;
                    break;
                case PlayerMagicType.Fire:
                    damage2 = statData.MagicAttackCurrent_Fire;
                    break;
                case PlayerMagicType.Ice:
                    damage2 = statData.MagicAttackCurrent_Ice;
                    break;
                case PlayerMagicType.Sand:
                    damage2 = statData.MagicAttackCurrent_Sand;
                    break;
                default:
                    Debug.LogError("알 수 없는 마법 타입");
                    return; // 알 수 없는 마법 타입 처리
            }

            target.TakeDamage(damage2);

            // 두 번째 마법의 경험치 증가
            magic2.AddExperience(damage2);
            Debug.Log($"{magic2.magicType} 마법의 경험치: {magic2.experience}");
        }
    }


    // 첫 번째 장착된 마법을 반환하는 메서드
    public PlayerMagic GetEquippedMagic1()
    {
        int magic1Index = playerInputManager.GetElement1Index();
        if (magic1Index != -1)
        {
            return playerInventory.playerMagics[magic1Index];
        }
        return null; // 장착된 마법이 없으면 null 반환
    }

    // 두 번째 장착된 마법을 반환하는 메서드
    public PlayerMagic GetEquippedMagic2()
    {
        int magic2Index = playerInputManager.GetElement2Index();
        if (magic2Index != -1)
        {
            return playerInventory.playerMagics[magic2Index];
        }
        return null; // 장착된 마법이 없으면 null 반환
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
