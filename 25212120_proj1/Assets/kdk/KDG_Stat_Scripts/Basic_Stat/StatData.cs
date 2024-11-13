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

    
    public bool ModifyCurrentHp(float amount)
    {
        mHpCurrent += amount;
        mHpCurrent = Mathf.Clamp(mHpCurrent, 0, hpMax); // �ּҰ� 0���� ����
        return mHpCurrent <= 0; // ���� ���� ��ȯ
    }




    public void InitStatData()
    {
        mHpCurrent = mHpCurrent == 0 ? hpMax : mHpCurrent;
        
    }

    public enum StatType
    {
        LEVEL,
        HP,
        ATTACK,
        MOVEMENT_SPEED,
        DEFENSE
    }

    public void UpgradeBaseStat(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.HP: // ü��
                hpMax += value;
                ModifyCurrentHp(value); // ���� ü���� �߰��� ü�¿� �°� ����
                break;

            case StatType.ATTACK: // ���ݷ�
                baseAttack += value;
                break;

            case StatType.MOVEMENT_SPEED: // �̵� �ӵ�
                baseMovementSpeed += value;
                break;

            case StatType.DEFENSE: // ����
                baseDefense += value;
                break;

            default:
                Debug.LogError($"�ε��� {statType}��(��) �����ϴ�!");
                break;
        }
    }

}
