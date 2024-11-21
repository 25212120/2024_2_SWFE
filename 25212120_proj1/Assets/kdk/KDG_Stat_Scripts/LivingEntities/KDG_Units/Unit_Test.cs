public class Unit_Test : BaseUnit
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }

    public float GetCurrentHP()
    {
        return statData.HpCurrent;   
    }

    public float GetCurrentAtk()
    {
        return statData.AttackCurrent;
    }
}