public class Monster_1 : BaseMonster
{
    protected override void Awake()
    {
        base.Awake();
        statData.SetHpMax(50); // 고블린의 최대 체력 설정
        statData.baseAttack = 10; // 고블린의 공격력 설정
    }
}
