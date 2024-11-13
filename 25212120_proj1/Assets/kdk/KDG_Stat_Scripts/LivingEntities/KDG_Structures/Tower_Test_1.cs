
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Tower_Test_1 : BaseStructure
{
    protected override void Awake()
    {
        base.Awake();
        statData.SetHpMax(50); 
        statData.baseAttack = 10; 
    }
}
