using UnityEngine;

[System.Serializable]
public class StatData
{
    [field: Header("�ʱ�ȭ �� ����")]
    [field: SerializeField] public int level { get; set; } = 1;

    [field: Header("�ʱ�ȭ �� �ִ� ü��")]
    [field: SerializeField] public float hpMax { get; set; }
    [SerializeField][HideInInspector] public float mHpCurrent;
    public float HpCurrent => mHpCurrent;

    [field: Header("�ʱ�ȭ �� �⺻ ���ݷ�")]
    [field: SerializeField] public float baseAttack { get; set; }

    [field: Header("�⺻ �̵��ӵ�")]
    [field: SerializeField] public float baseMovementSpeed { get; set; }

    [field: Header("�⺻ ����")]
    [field: SerializeField] public float baseDefense { get; set; }

    //���� ���� ���
    public float AttackCurrent
    {
        get
        {
            return baseAttack + buffController.BuffStat.attack;
        }
    }

    public float MovementSpeedCurrent
    {
        get
        {
            return baseMovementSpeed + buffController.BuffStat.movementSpeed;
        }
    }

    public float DefenseCurrent
    {
        get
        {
            return baseDefense + buffController.BuffStat.defense;
        }
    }
    // �ܺ� Ŭ����

    [HideInInspector] public BuffController buffController = new BuffController(); // ���� ��Ʈ�ѷ� (��ο��� ����)

 

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
            case StatType.LEVEL:
                level++;
                break;

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
