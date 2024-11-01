using UnityEngine;

public class BaseMonster : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); // �θ��� Awake() ȣ��
        InitializeMonsterStats();
    }

    protected virtual void InitializeMonsterStats()
    {
        statData.SetHpMax(100); // ������ �ִ� ü�� �ʱ�ȭ
        statData.baseAttack = 10; // ������ �⺻ ���ݷ� �ʱ�ȭ
    }

    public void Attack(BaseEntity target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }
}
