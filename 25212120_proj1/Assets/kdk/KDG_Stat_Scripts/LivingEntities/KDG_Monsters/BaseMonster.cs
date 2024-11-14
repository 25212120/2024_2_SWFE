        using UnityEngine;

public class BaseMonster : BaseEntity
{
    [Header("몬스터 경험치")]
    [SerializeField] protected int expAmount; /// 각 몬스터 클래스마다 설정해주기
    protected override void Awake()
    {
        base.Awake(); 
        InitializeMonsterStats();
    }

    protected virtual void InitializeMonsterStats()
    {
        statData.SetHpMax(100); // 몬스터의 최대 체력 초기화
        statData.baseAttack = 10; // 몬스터의 기본 공격력 초기화
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
    }
    private void AwardExperienceToPlayer()
    {
        ExpManager.Instance.AddExp(expAmount);
    }
}
