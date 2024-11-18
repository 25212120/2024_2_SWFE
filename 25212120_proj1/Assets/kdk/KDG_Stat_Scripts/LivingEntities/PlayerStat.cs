using UnityEngine;

public class PlayerStat : BaseEntity
{
    protected override void Awake()
    {
        base.Awake(); // �θ��� Awake() ȣ��
        InitializePlayerStats();
    }

    private void InitializePlayerStats()
    {
        statData.SetHpMax(150); // �÷��̾��� �ִ� ü�� �ʱ�ȭ
        statData.baseAttack = 15; // �÷��̾��� �⺻ ���ݷ� �ʱ�ȭ
        // ��Ÿ �ʱ�ȭ
    }

    public void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // ���� ���ݷ� ���
        target.TakeDamage(damage);
    }

    

}
