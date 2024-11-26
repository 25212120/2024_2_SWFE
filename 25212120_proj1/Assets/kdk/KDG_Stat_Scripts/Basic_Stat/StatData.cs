using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class StatData
{
    [field: Header("초기화 시 레벨")]
    [field: SerializeField] public int level { get; set; } = 1;

    [field: Header("초기화 시 최대 체력")]
    [field: SerializeField] public float hpMax { get; set; }
    [SerializeField][HideInInspector] public float mHpCurrent;
    public float HpCurrent => mHpCurrent;

    [field: Header("초기화 시 기본 공격력")]
    [field: SerializeField] public float baseAttack { get; set; }

    [field: Header("기본 이동속도")]
    [field: SerializeField] public float baseMovementSpeed { get; set; }

    [field: Header("기본 방어력")]
    [field: SerializeField] public float baseDefense { get; set; }

    [field: Header("나무 마법 공격력")]
    [field: SerializeField] public float baseMagicAttack_Wood { get; set; }
    [field: Header("불 마법 공격력")]
    [field: SerializeField] public float baseMagicAttack_Fire { get; set; }
    [field: Header("얼음 마법 공격력")]
    [field: SerializeField] public float baseMagicAttack_Ice { get; set; }
    [field: Header("사막 마법 공격력")]
    [field: SerializeField] public float baseMagicAttack_Sand { get; set; }

    //현재 스텟 계산
    public float MagicAttackCurrent_Wood
    {
        get
        {
            return baseMagicAttack_Wood;
        }
    }
    public float MagicAttackCurrent_Fire
    {
        get
        {
            return baseMagicAttack_Fire;
        }
    }
    public float MagicAttackCurrent_Ice
    {
        get
        {
            return baseMagicAttack_Ice;
        }
    }
    public float MagicAttackCurrent_Sand
    {
        get
        {
            return baseMagicAttack_Sand;
        }
    }

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
    // 외부 클래스

    [HideInInspector] public BuffController buffController = new BuffController(); // 버프 컨트롤러 (모두에게 고유)

 

    public void SetHpMax(float value)
    {
        hpMax = value;
        mHpCurrent = hpMax; // 현재 체력을 최대 체력으로 초기화
    }

    
    public bool ModifyCurrentHp(float amount)
    {
        mHpCurrent += amount;
        mHpCurrent = Mathf.Clamp(mHpCurrent, 0, hpMax); // 최소값 0으로 설정
        return mHpCurrent <= 0; // 죽음 여부 반환
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

            case StatType.HP: // 체력
                hpMax += value;
                ModifyCurrentHp(hpMax); // 현재 체력을 추가된 체력에 맞게 수정
                break;

            case StatType.ATTACK: // 공격력
                baseAttack += value;
                break;

            case StatType.MOVEMENT_SPEED: // 이동 속도
                baseMovementSpeed += value;
                break;

            case StatType.DEFENSE: // 방어력
                baseDefense += value;
                break;

            default:
                Debug.LogError($"인덱스 {statType}은(는) 없습니다!");
                break;
        }
    }

}
