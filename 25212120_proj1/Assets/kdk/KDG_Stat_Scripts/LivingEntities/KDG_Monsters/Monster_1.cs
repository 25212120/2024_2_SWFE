public class Monster_1 : BaseMonster
{
    protected override void Awake()
    {
        base.Awake();
        statData.SetHpMax(50); // ����� �ִ� ü�� ����
        statData.baseAttack = 10; // ����� ���ݷ� ����
    }
}
