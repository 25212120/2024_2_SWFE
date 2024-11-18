using System.Collections.Generic;
using UnityEngine;

public class BaseStructure : BaseEntity
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

        Debug.Log("업그레이드 완료: 공격력 +5, 체력 +50");
    }

    public virtual void Repair(float amount)
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

    // 업그레이드에 필요한 자원 클래스
    [System.Serializable]
    public class ResourceRequirement
    {
        public MaterialManager.ResourceType resourceType; // 자원 타입
        public int amount;                                // 자원 수량
    }
}
