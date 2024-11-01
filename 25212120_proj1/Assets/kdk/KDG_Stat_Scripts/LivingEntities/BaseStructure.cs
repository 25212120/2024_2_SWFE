using UnityEngine;

public class BaseStructure : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); // �θ��� Awake() ȣ��
        InitializeStructureStats();
    }

    protected void InitializeStructureStats()
    {
        statData.SetHpMax(200); // �������� �ִ� ü�� �ʱ�ȭ
    }
    public virtual void Repair(float amount)
    {
        // �������� ü���� �����ϴ� ����
        statData.ModifyCurrentHp(amount);
    }

    public void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} has been destroyed.");
        // ������ �ı� ���� �߰�
        Destroy(gameObject);
    }
}
