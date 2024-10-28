using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatData
{
    [field: Header("초기화 시 레벨")]
    [field: SerializeField] public int level { private set; get; } = 1;


    [field: Header("초기화 시 최대 체력")]
    [field: SerializeField] public float hpMax { private set; get; }
    [SerializeField][HideInInspector] private float mHpCurrent;
    public float HpCurrent
    {
        get
        {
            return mHpCurrent;
        }
    }



    [field: Header("초기화 시 최대 마나")]
    [field: SerializeField] public float mpMax { private set; get; }
    [SerializeField][HideInInspector] private float mMpCurrent;
    public float MpCurrent
    {
        get
        {
            return mMpCurrent;
        }
    }



    [field: Header("초기화 시 기본 공격력")]
    [field: SerializeField] public float baseAttack { private set; get; }

    
    /// 현재 공격력
    public float AttackCurrent
    {
        get
        {
            return baseAttack + buffController.BuffStat.attack +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.Attack : 0f);
        }
    }



    [field: Header("초기화 시 기본 이동속도")]
    [field: SerializeField] public float baseMovementSpeed { private set; get; }

    
    /// 현재 이동속도
    
    public float MovementSpeedCurrent
    {
        get
        {
            return baseMovementSpeed + buffController.BuffStat.movementSpeed +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.MovementSpeed : 0f);
        }
    }



    [field: Header("초기화 시 기본 방어력")]
    [field: SerializeField] public float baseDefense { private set; get; }

    public float DefenseCurrent
    {
        get
        {
            return baseDefense + buffController.BuffStat.defense +
            (equipmentInventory is not null ? equipmentInventory.CurrentEquipmentEffect.Defense : 0f);
        }
    }



    // 외부 클래스

    [HideInInspector] public BuffController buffController = new BuffController(); // 버프 컨트롤러 (모두에게 고유)

    [Space(30)]
    [Header("외부 클래스를 참조하여 스탯에 추가 효과")]
    [Header("해당 객체의 장비인벤토리, 없을경우 null 가능")]
    [SerializeField] public EquipmentInventory? equipmentInventory = null; // 장비 인벤토리 (개별적으로 로드하여 사용 가능)

    

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
            case StatType.LEVEL: // 레벨
                ++level;
                break;

            case StatType.HP: // 체력
                hpMax += 50;
                ModifyCurrentHp(50);
                break;

            case StatType.MP: // 마나
                mpMax += 50;
                ModifyCurrentMp(50);
                break;

            case StatType.ATTACK: // 공격력
                baseAttack += 5;
                break;

            case StatType.MOVEMENT_SPEED: // 속도
                baseMovementSpeed += 1;
                break;

            case StatType.DEFENSE: // 방어
                baseDefense += 2.5f;
                break;

            default:
                Debug.LogError($"인덱스 {statType}은 없음!");
                break;
        }
    }
}
