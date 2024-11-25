using UnityEngine;
using System.Collections.Generic;

public class BaseMonster : BaseEntity
{
    [Header("몬스터 경험치")]
    [SerializeField] protected int expAmount;

    [Header("몬스터 자원 드랍")]
    [SerializeField] protected List<ResourceDrop> resourceDrops = new List<ResourceDrop>();  // 자원 드랍 리스트

    [System.Serializable]
    public class ResourceDrop
    {
        public MaterialManager.ResourceType resourceType;  // 자원 타입
        public int amount;  // 자원 수량
        public float dropChance;  // 드랍 확률 (0~1)

        // 드랍 확률을 설정
        public ResourceDrop(MaterialManager.ResourceType resourceType, int amount, float dropChance)
        {
            this.resourceType = resourceType;
            this.amount = amount;
            this.dropChance = dropChance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeMonsterStats();
    }

    public void PerformUpgrade()
    {
        // 스텟을 증가시키는 로직 (예시: 공격력과 체력 증가)
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, 5); // 예시: 공격력 +5
        statData.UpgradeBaseStat(StatData.StatType.HP, 50);   // 예시: 체력 +50

        Debug.Log("업그레이드 완료: 공격력 +5, 체력 +50");
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
            MaterialManager.Instance.GainResourceWithChance(resourceDrop.resourceType, resourceDrop.amount, resourceDrop.dropChance);
            Debug.Log($"{gameObject.name} 드랍: {resourceDrop.amount} {resourceDrop.resourceType} 확률: {resourceDrop.dropChance * 100}%");
        }
    }
}
