using UnityEngine;

public class PlayerStat : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); // 부모의 Awake() 호출
        InitializePlayerStats();
    }

    private void InitializePlayerStats()
    {
        statData.SetHpMax(150); // 플레이어의 최대 체력 초기화
        statData.baseAttack = 15; // 플레이어의 기본 공격력 초기화
        // 기타 초기화
    }

    public void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);
    }

    

}
