using UnityEngine;

public class BaseMonster : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); // 부모의 Awake() 호출
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
}
