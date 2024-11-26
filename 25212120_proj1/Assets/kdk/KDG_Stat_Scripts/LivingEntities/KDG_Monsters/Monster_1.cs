
using System.Collections.Generic;
using System.Diagnostics;

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
    
    public float GetMaxHp()
    {
        return statData.hpMax;
    }

    public float GetMaxHP()
    {
        return statData.hpMax;
    }

}
