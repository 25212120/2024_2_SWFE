using System.Collections.Generic;
using UnityEngine;
using static BaseStructure;

public abstract class BaseUnit : BaseEntity
{
    [Header("업그레이드에 필요한 자원")]
    [SerializeField] private List<ResourceRequirement> upgradeRequirements = new List<ResourceRequirement>(); // 업그레이드에 필요한 자원 리스트

    protected override void Awake()
    {
        base.Awake();
    }

    // 자원 소모하여 업그레이드 수행
    public bool Upgrade()
    {
        // 업그레이드에 필요한 자원들을 모두 소모할 수 있는지 확인
        foreach (var requirement in upgradeRequirements)
        {
            if (!MaterialManager.Instance.ConsumeResource(requirement.resourceType, requirement.amount))
            {
                // 자원이 부족하면 업그레이드 실패
                Debug.LogWarning($"{requirement.resourceType} 자원이 부족합니다. 업그레이드 실패.");
                return false;
            }
        }

        // 모든 자원 소비가 성공하면 업그레이드 진행
        PerformUpgrade();

        return true;
    }

    // 업그레이드 진행 (스텟을 증가시킴)
    private void PerformUpgrade()
    {
        // 스텟을 증가시키는 로직 (예시: 공격력과 체력 증가)
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, 5); // 예시: 공격력 +5
        statData.UpgradeBaseStat(StatData.StatType.HP, 50);   // 예시: 체력 +50
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 1);

        Debug.Log("업그레이드 완료: 공격력 +5, 체력 +50");
    }
    public void UpdateAllUnitsStats(List<BaseUnit> units)
    {
        // 전달받은 리스트에 있는 각 유닛에 대해 스탯을 업데이트
        foreach (BaseUnit unit in units)
        {
            if (unit is Unit_Test)
            {
                unit.PerformUpgrade();  // PerformUpgrade를 호출하여 해당 유닛의 스탯을 업데이트
            }
        }
    }
    public void Heal(float amount)
    {
        statData.ModifyCurrentHp(amount);
    }

    public virtual void Attack(BaseMonster target)
    {

        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);
    }


    protected override void Die()
    {
        base.Die();
    }
}
