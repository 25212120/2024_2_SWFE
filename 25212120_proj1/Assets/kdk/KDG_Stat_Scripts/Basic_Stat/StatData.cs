using UnityEngine;

[System.Serializable]
public class StatData
{
    [field: Header("�ʱ�ȭ �� ����")]
    [field: SerializeField] public int level { get; set; } = 1;

    [field: Header("�ʱ�ȭ �� �ִ� ü��")]
    [field: SerializeField] public float hpMax { get; set; }
    [SerializeField][HideInInspector] private float mHpCurrent;
    public float HpCurrent => mHpCurrent;

    [field: Header("�ʱ�ȭ �� �ִ� ����")]
    [field: SerializeField] public float mpMax { get; set; }
    [SerializeField][HideInInspector] private float mMpCurrent;
    public float MpCurrent => mMpCurrent;

    [field: Header("�ʱ�ȭ �� �⺻ ���ݷ�")]
    [field: SerializeField] public float baseAttack { get; set; }

    [field: Header("�⺻ �̵��ӵ�")]
    [field: SerializeField] public float baseMovementSpeed { get; set; }

    [field: Header("�⺻ ����")]
    [field: SerializeField] public float baseDefense { get; set; }

    //���� ���� ���
    public float DefenseCurrent
    {
        get
        {
            return baseDefense + buffController.BuffStat.defense +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.Defense : 0f);
        }
    }
    public float MovementSpeedCurrent
    {
        get
        {
            return baseMovementSpeed + buffController.BuffStat.movementSpeed +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.MovementSpeed : 0f);
        }
    }
    // ���� ���ݷ� ���
    public float AttackCurrent
    {
        get
        {
            return baseAttack + buffController.BuffStat.attack +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.Attack : 0f);
        }
    }
    // �ܺ� Ŭ����

    [HideInInspector] public BuffController buffController = new BuffController(); // ���� ��Ʈ�ѷ� (��ο��� ����)

    [Space(30)]
    [Header("�ܺ� Ŭ������ �����Ͽ� ���ȿ� �߰� ȿ��")]
    [Header("�ش� ��ü�� ����κ��丮, ������� null ����")]
    [SerializeField] public EquipmentInventory? equipmentInventory = null; // ��� �κ��丮 (���������� �ε��Ͽ� ��� ����)

    public void SetHpMax(float value)
    {
        hpMax = value;
        mHpCurrent = hpMax; // ���� ü���� �ִ� ü������ �ʱ�ȭ
    }

    public void SetMpMax(float value)
    {
        mpMax = value;
        mMpCurrent = mpMax; // ���� ������ �ִ� ������ �ʱ�ȭ
    }

    public bool ModifyCurrentHp(float amount)
    {
        mHpCurrent += amount;
        mHpCurrent = Mathf.Clamp(mHpCurrent, 0, hpMax); // �ּҰ� 0���� ����
        return mHpCurrent <= 0; // ���� ���� ��ȯ
    }


    public void ModifyCurrentMp(float amount)
    {
        mMpCurrent += amount;
        mMpCurrent = Mathf.Clamp(mMpCurrent, 0, mpMax); // �ּҰ� 0���� ����
    }


    public void InitStatData()
    {
        mHpCurrent = mHpCurrent == 0 ? hpMax : mHpCurrent;
        mMpCurrent = mMpCurrent == 0 ? mpMax : mMpCurrent;
    }

    public enum StatType
    {
        LEVEL,
        HP,
        MP,
        ATTACK,
        MOVEMENT_SPEED,
        DEFENSE
    }

    public void UpgradeBaseStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.LEVEL: // ����
                ++level;
                break;

            case StatType.HP: // ü��
                hpMax += 50;
                ModifyCurrentHp(50);
                break;

            case StatType.MP: // ����
                mpMax += 50;
                ModifyCurrentMp(50);
                break;

            case StatType.ATTACK: // ���ݷ�
                baseAttack += 5;
                break;

            case StatType.MOVEMENT_SPEED: // �ӵ�
                // movementSpeed ���� ���� �߰�
                break;

            case StatType.DEFENSE: // ���
                // defense ���� ���� �߰�
                break;

            default:
                Debug.LogError($"�ε��� {statType}�� ����!");
                break;
        }
    }

}
