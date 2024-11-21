using UnityEngine;

public class PlayerStat : BaseEntity
{
    [Header("�÷��̾� ��� �κ��丮")]
    public EquipmentInventory equipmentInventory; // EquipmentInventory ������Ʈ ����
    private PlayerInputManager  playerInputManager;
    public PlayerInventory playerInventory;

    private void Start()
    {
        equipmentInventory = GetComponent<EquipmentInventory>(); // EquipmentInventory�� Player ������Ʈ�� ����
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
    // ù ��° ������ ����� ����
    public void MagicAttackWithMagic1(BaseMonster target)
    {
        Debug.Log("MagicAttack with Magic1");

        PlayerMagic magic1 = GetEquippedMagic1();

        if (magic1 != null)
        {
            float damage1 = 0;

            // ù ��° ������ ������ ���
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
                    Debug.LogError("�� �� ���� ���� Ÿ��");
                    return; // �� �� ���� ���� Ÿ�� ó��
            }

            target.TakeDamage(damage1);

            // ù ��° ������ ����ġ ����
            magic1.AddExperience(damage1);
            Debug.Log($"{magic1.magicType} ������ ����ġ: {magic1.experience}");
        }
    }

    // �� ��° ������ ����� ����
    public void MagicAttackWithMagic2(BaseMonster target)
    {
        Debug.Log("MagicAttack with Magic2");

        // �� ��° ���� ��������
        PlayerMagic magic2 = GetEquippedMagic2();

        if (magic2 != null)
        {
            float damage2 = 0;

            // �� ��° ������ ������ ���
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
                    Debug.LogError("�� �� ���� ���� Ÿ��");
                    return; // �� �� ���� ���� Ÿ�� ó��
            }

            target.TakeDamage(damage2);

            // �� ��° ������ ����ġ ����
            magic2.AddExperience(damage2);
            Debug.Log($"{magic2.magicType} ������ ����ġ: {magic2.experience}");
        }
    }


    // ù ��° ������ ������ ��ȯ�ϴ� �޼���
    public PlayerMagic GetEquippedMagic1()
    {
        int magic1Index = playerInputManager.GetElement1Index();
        if (magic1Index != -1)
        {
            return playerInventory.playerMagics[magic1Index];
        }
        return null; // ������ ������ ������ null ��ȯ
    }

    // �� ��° ������ ������ ��ȯ�ϴ� �޼���
    public PlayerMagic GetEquippedMagic2()
    {
        int magic2Index = playerInputManager.GetElement2Index();
        if (magic2Index != -1)
        {
            return playerInventory.playerMagics[magic2Index];
        }
        return null; // ������ ������ ������ null ��ȯ
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
