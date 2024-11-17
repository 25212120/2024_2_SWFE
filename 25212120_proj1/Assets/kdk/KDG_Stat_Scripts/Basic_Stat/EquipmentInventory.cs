using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentEffect
{
    public float Attack;           // 공격력
    public float MovementSpeed;    // 이동 속도
    public float Defense;          // 방어력
}

[System.Serializable]
public class EquipmentItem
{
    public string itemName;        // 아이템 이름
    public EquipmentEffect effect; // 아이템의 효과
    public int level = 1;             // 장비의 레벨 (기본 레벨은 1)
    public float experience = 0f;     // 장비의 경험치


    [Header("레벨업 시 증가되는 효과")]
    public float attackIncreasePerLevel = 2f;           // 레벨업 시 공격력 증가량
    public float movementSpeedIncreasePerLevel = 0.5f;  // 레벨업 시 이동 속도 증가량
    public float defenseIncreasePerLevel = 1f;          // 레벨업 시 방어력 증가량
    public float maxExperience = 100f;                   // 장비의 최대 경험치


    public EquipmentItem(string name, EquipmentEffect effect)
    {
        itemName = name;
        this.effect = effect;
    }

    public void LevelUp()
    {
        level++; // 장비의 레벨 증가

        // 장비의 효과를 레벨업 시 증가
        effect.Attack += attackIncreasePerLevel;            // 공격력 증가
        effect.MovementSpeed += movementSpeedIncreasePerLevel;  // 이동속도 증가
        effect.Defense += defenseIncreasePerLevel;           // 방어력 증가

        maxExperience *= 2.0f;
        experience = 0f;

        Debug.Log($"{itemName} 레벨업! 현재 레벨: {level}, 공격력: {effect.Attack}, 이동속도: {effect.MovementSpeed}, 방어력: {effect.Defense}");
    }



    public void AddExperience(float amount)
    {
        experience += amount;

        // 경험치가 최대치 이상으로 증가했을 때
        if (experience >= maxExperience)
        {
            experience = 0f;  // 경험치 초기화
            LevelUp();  // 레벨업
            ApplyLevelEffect();
        }
    }
    public void ApplyLevelEffect()
    {
        // 각 레벨마다 증가되는 효과 계산
        effect.Attack = level * attackIncreasePerLevel;
        effect.MovementSpeed = level * movementSpeedIncreasePerLevel;
        effect.Defense = level * defenseIncreasePerLevel;
    }
}


public class EquipmentInventory : MonoBehaviour
{
    [Header("초기 장비 목록")]
    [SerializeField] public EquipmentItem[] availableEquipments = new EquipmentItem[4];  // 4개의 장비 목록
    public EquipmentItem currentEquipment;  // 현재 장착된 장비
    [SerializeField] private GameObject[] rightHandWeapons;  // 오른쪽 손에 장착할 무기들
    public int currentRightHandIndex = 0;  // 현재 오른쪽 손에 장착된 무기의 인덱스

    private PlayerStat playerstat;
    private int currentIndex = 0;  // 현재 장착된 장비의 인덱스 (순차적으로 교체)

    private void Start()
    {
        playerstat = GetComponent<PlayerStat>();  // PlayerStat 컴포넌트 찾기

        if (playerstat == null || playerstat.statData == null)
        {
            Debug.LogError("PlayerStat 또는 StatData 컴포넌트를 찾을 수 없습니다! 장비 효과를 적용할 수 없습니다.");
            return;
        }
        SetupInitialEquipments();

        // 기본 장비를 첫 번째 아이템으로 설정
        Equip(availableEquipments[currentIndex]);
    }

    public void SetupInitialEquipments()
    {
        // 각 장비의 초기 효과와 레벨업 시 증가하는 수치 설정
        availableEquipments[0] = new EquipmentItem("SwordAndSheild", new EquipmentEffect { Attack = 10, MovementSpeed = 3, Defense = 5 })
        {
            attackIncreasePerLevel = 1f, // 공격력 증가량
            movementSpeedIncreasePerLevel = 1f, // 이동속도 증가량
            defenseIncreasePerLevel = 1f // 방어력 증가량
        };

        availableEquipments[1] = new EquipmentItem("SingleTwoHandeSword", new EquipmentEffect { Attack = 15, MovementSpeed = 1, Defense = 2 })
        {
            attackIncreasePerLevel = 1f, // 공격력 증가량
            movementSpeedIncreasePerLevel = 1f, // 이동속도 증가량
            defenseIncreasePerLevel = 1f // 방어력 증가량
        };

        availableEquipments[2] = new EquipmentItem("DoubleSwords", new EquipmentEffect { Attack = 20, MovementSpeed = 5, Defense = -5 })
        {
            attackIncreasePerLevel = 1f, // 공격력 증가량
            movementSpeedIncreasePerLevel = 1f, // 이동속도 증가량
            defenseIncreasePerLevel = 1f // 방어력 감소량
        };

        availableEquipments[3] = new EquipmentItem("BowAndArrow", new EquipmentEffect { Attack = 15, MovementSpeed = 7, Defense = -5 })
        {
            attackIncreasePerLevel = 1f, // 공격력 증가량
            movementSpeedIncreasePerLevel = 1f, // 이동속도 증가량
            defenseIncreasePerLevel = 1f // 방어력 감소량
        };

        Debug.Log("장비가 초기화되었습니다.");
    }


    // 장비 교체 메서드 (스왑)
    public void Equip(EquipmentItem newEquipment)
    {
        if (currentEquipment != null)
        {
            // 기존 장비 효과 제거
            RemoveEquipmentEffect(currentEquipment.effect);
        }

        // 새로운 장비 효과 반영
        currentEquipment = newEquipment;
        ApplyEquipmentEffect(currentEquipment.effect);
        Debug.Log($"{currentEquipment.itemName} 장비를 장착했습니다.");
    }

    public void EquipByIndex(int index)
    {
        if (index >= 0 && index < availableEquipments.Length)
        {
            Equip(availableEquipments[index]);
        }
    }

    // 장비 효과를 StatData에 반영하는 메서드
    private void ApplyEquipmentEffect(EquipmentEffect equipmentEffect)
    {
        // BaseEntity에서 StatData를 가져와 장비 효과 반영
        playerstat.statData.baseAttack += equipmentEffect.Attack;
        playerstat.statData.baseMovementSpeed += equipmentEffect.MovementSpeed;
        playerstat.statData.baseDefense += equipmentEffect.Defense;

        // 장비 효과 반영 후 스탯 갱신
        Debug.Log($"장비 효과 반영: 공격력: {playerstat.statData.baseAttack}, 이동속도: {playerstat.statData.baseMovementSpeed}, 방어력: {playerstat.statData.baseDefense}");
    }

    // 장비 효과를 제거하는 메소드
    private void RemoveEquipmentEffect(EquipmentEffect equipmentEffect)
    {
        // 기존 장비 효과를 제거
        playerstat.statData.baseAttack -= equipmentEffect.Attack;
        playerstat.statData.baseMovementSpeed -= equipmentEffect.MovementSpeed;
        playerstat.statData.baseDefense -= equipmentEffect.Defense;

        // 장비 효과 제거 후 스탯 갱신
        Debug.Log($"장비 효과 제거: 공격력: {playerstat.statData.baseAttack}, 이동속도: {playerstat.statData.baseMovementSpeed}, 방어력: {playerstat.statData.baseDefense}");
    }


    // 현재 장착된 장비를 확인하는 메서드
    public void PrintCurrentEquipment()
    {
        if (currentEquipment != null)
        {
            Debug.Log($"현재 장착된 장비: {currentEquipment.itemName}, 공격력: {currentEquipment.effect.Attack}, 이동속도: {currentEquipment.effect.MovementSpeed}, 방어력: {currentEquipment.effect.Defense}");
        }
        else
        {
            Debug.Log("현재 장비가 없습니다.");
        }
    }
}


