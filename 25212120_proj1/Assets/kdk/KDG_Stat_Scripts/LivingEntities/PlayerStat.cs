using UnityEngine;

public class PlayerStat : BaseEntity
{
    [Header("�÷��̾� ��� �κ��丮")]
    public EquipmentInventory equipmentInventory; // EquipmentInventory ������Ʈ ����
    private PlayerInputManager  playerInputManager;
    public PlayerInventory playerInventory;

    protected override void Awake()
    {
        base.Awake();
        equipmentInventory = GetComponent<EquipmentInventory>(); // EquipmentInventory�� Player ������Ʈ�� ����
        playerInventory = GetComponent<PlayerInventory>();
    }
    public int GetLevel()
    {
        return statData.level;
    }
    public void Attack(BaseMonster target)
    {
        Debug.Log("ATTACK");
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        // ��� ����ġ�� ������Ŵ (���� ����� ����ġ�� �������� ������)
        if (equipmentInventory != null && equipmentInventory.currentEquipment != null)
        {
            // ����ġ�� ��������ŭ �߰�
            equipmentInventory.currentEquipment.AddExperience(damage);

            Debug.Log($"{equipmentInventory.currentEquipment.itemName} ����ġ: {equipmentInventory.currentEquipment.experience}");
        }
    }
    // ù ��° ������ ����� ����
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
                    Debug.LogError("�� �� ���� ���� Ÿ��");
                    return; // �� �� ���� ���� Ÿ�� ó��
            }

            target.TakeDamage(damage1); 

            magic.AddExperience(damage1);
            Debug.Log($"{magic.magicType} ������ ����ġ: {magic.experience}");
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
    
    public float GetMaxHp()
    {
        return statData.hpMax;
    }
}
