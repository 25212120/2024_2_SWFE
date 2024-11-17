public class MagicTower : BaseUnit
{
    protected override void Awake()
    {
        base.Awake();
    }
    public override void Attack(BaseMonster target)
    {
        base.Attack(target);
    }

    public override void Heal(float amount)
    {
        base.Heal(amount);
    }
}