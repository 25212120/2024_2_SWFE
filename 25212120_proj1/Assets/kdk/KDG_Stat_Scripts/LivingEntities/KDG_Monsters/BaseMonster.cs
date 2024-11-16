using UnityEngine;
using System.Collections.Generic;

public class BaseMonster : BaseEntity
{
    [Header("몬스터 경험치")]
    [SerializeField] protected int expAmount;

    [Header("몬스터 자원 드랍")]
    [SerializeField] protected List<ResourceDrop> resourceDrops = new List<ResourceDrop>();  // 자원 드랍 리스트

    // ResourceDrop 클래스를 BaseMonster 클래스 내부에 정의
    [System.Serializable]
    public class ResourceDrop
    {
        public MaterialManager.ResourceType resourceType;  // 자원 타입
        public int amount;  // 자원 수량
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeMonsterStats();
    }

    protected virtual void InitializeMonsterStats()
    {
        statData.SetHpMax(100);  // 몬스터의 최대 체력 초기화
        statData.baseAttack = 10;  // 몬스터의 기본 공격력 초기화
    }

    public void Attack(BaseEntity target)
    {
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);
    }

    protected override void Die()
    {
        base.Die();  // 기본 사망 처리
        AwardExperienceToPlayer();
        DropResources();  // 자원 드랍 처리
    }

    private void AwardExperienceToPlayer()
    {
        ExpManager.Instance.AddExp(expAmount);  // 경험치 지급
    }

    private void DropResources()
    {
        // 자원 드랍 처리
        foreach (var resourceDrop in resourceDrops)
        {
            MaterialManager.Instance.GainResource(resourceDrop.resourceType, resourceDrop.amount);
            Debug.Log($"{gameObject.name} 드랍: {resourceDrop.amount} {resourceDrop.resourceType}");
        }
    }
}
