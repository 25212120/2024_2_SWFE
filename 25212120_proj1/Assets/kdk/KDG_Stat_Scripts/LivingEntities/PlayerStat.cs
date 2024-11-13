using UnityEngine;

public class PlayerStat : BaseEntity
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void Attack(BaseMonster target)
    {
        float damage = statData.AttackCurrent; // 현재 공격력 사용
        target.TakeDamage(damage);
    }
    public void LevelUp()
    {
        statData.UpgradeBaseStat(StatData.StatType.HP, 1);
        statData.UpgradeBaseStat(StatData.StatType.ATTACK, 1);
        statData.UpgradeBaseStat(StatData.StatType.DEFENSE, 1); // 추후 세부 조정
    }
    public void Heal(float healAmount)
    {
        statData.ModifyCurrentHp(healAmount);
    }
}
