using UnityEngine;

public class BaseStructure : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); // 부모의 Awake() 호출
        InitializeStructureStats();
    }

    protected void InitializeStructureStats()
    {
        statData.SetHpMax(200); // 구조물의 최대 체력 초기화
    }
    public virtual void Repair(float amount)
    {
        // 구조물의 체력을 수리하는 로직
        statData.ModifyCurrentHp(amount);
    }

    public void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);
    }
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed.");
        // 구조물 파괴 로직 추가
        Destroy(gameObject);
    }
}
