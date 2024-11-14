using UnityEngine;

public class BaseStructure : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); 
    }

    public virtual void Repair(float amount)
    {
        statData.ModifyCurrentHp(amount);
    }

    public virtual void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }
    protected override void Die()
    {
        base.Die();
    }
}
