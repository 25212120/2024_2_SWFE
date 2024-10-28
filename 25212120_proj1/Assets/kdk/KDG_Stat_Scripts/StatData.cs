using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatData
{
    [field: Header("�ʱ�ȭ �� ����")]
    [field: SerializeField] public int level { private set; get; } = 1;


    [field: Header("�ʱ�ȭ �� �ִ� ü��")]
    [field: SerializeField] public float hpMax { private set; get; }
    [SerializeField][HideInInspector] private float mHpCurrent;
    public float HpCurrent
    {
        get
        {
            return mHpCurrent;
        }
    }



    [field: Header("�ʱ�ȭ �� �ִ� ����")]
    [field: SerializeField] public float mpMax { private set; get; }
    [SerializeField][HideInInspector] private float mMpCurrent;
    public float MpCurrent
    {
        get
        {
            return mMpCurrent;
        }
    }



    [field: Header("�ʱ�ȭ �� �⺻ ���ݷ�")]
    [field: SerializeField] public float baseAttack { private set; get; }

    
    /// ���� ���ݷ�
    public float AttackCurrent
    {
        get
        {
            return baseAttack + buffController.BuffStat.attack +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.Attack : 0f);
        }
    }



    [field: Header("�ʱ�ȭ �� �⺻ �̵��ӵ�")]
    [field: SerializeField] public float baseMovementSpeed { private set; get; }

    
    /// ���� �̵��ӵ�
    
    public float MovementSpeedCurrent
    {
        get
        {
            return baseMovementSpeed + buffController.BuffStat.movementSpeed +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.MovementSpeed : 0f);
        }
    }



    [field: Header("�ʱ�ȭ �� �⺻ ����")]
    [field: SerializeField] public float baseDefense { private set; get; }

    public float DefenseCurrent
    {
        get
        {
            return baseDefense + buffController.BuffStat.defense +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.Defense : 0f);
        }
    }



    // �ܺ� Ŭ����

    [HideInInspector] public BuffController buffController = new BuffController(); // ���� ��Ʈ�ѷ� (��ο��� ����)

    [Space(30)]
    [Header("�ܺ� Ŭ������ �����Ͽ� ���ȿ� �߰� ȿ��")]
    [Header("�ش� ��ü�� ����κ��丮, ������� null ����")]
    [SerializeField] public EquipmentInventory? equipmentInventory = null; // ��� �κ��丮 (���������� �ε��Ͽ� ��� ����)

    

    public enum StatType
    {
        LEVEL,
        HP,
        MP,
        ATTACK,
        MOVEMENT_SPEED,
        DEFENSE
    }


    public void InitStatData()
    {
        mHpCurrent = mHpCurrent == 0 ? hpMax : mHpCurrent;
        mMpCurrent = mMpCurrent == 0 ? mpMax : mMpCurrent;
    }

    
    public bool ModifyCurrentHp(float amount)
    {
        mHpCurrent += amount;
        mHpCurrent = Mathf.Clamp(mHpCurrent, float.MinValue, hpMax);

        return mHpCurrent < 0f;
    }

    
    public void ModifyCurrentMp(float amount)
    {
        mMpCurrent += amount;
        mMpCurrent = Mathf.Clamp(mMpCurrent, float.MinValue, mpMax);
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
                baseMovementSpeed += 1;
                break;

            case StatType.DEFENSE: // ���
                baseDefense += 2.5f;
                break;

            default:
                Debug.LogError($"�ε��� {statType}�� ����!");
                break;
        }
    }
}
