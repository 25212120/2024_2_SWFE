
using System.Collections.Generic;

public class Monster_1 : BaseMonster
{
    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Update()
    {
        base.Update();
    }

    public float GetCurrentHP()
    {
        return statData.HpCurrent;
    }

    public float GetMaxHP()
    {
        return statData.hpMax;
    }

}
