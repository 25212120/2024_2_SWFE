        using UnityEngine;

public class BaseMonster : BaseEntity
{
    [Header("���� ����ġ")]
    [SerializeField] protected int expAmount; /// �� ���� Ŭ�������� �������ֱ�
    protected override void Awake()
    {
        base.Awake(); 
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
    protected override void Die()
    {
        base.Die();  // �⺻ ��� ó��

        AwardExperienceToPlayer();
    }
    private void AwardExperienceToPlayer()
    {
        ExpManager.Instance.AddExp(expAmount);
    }
}
