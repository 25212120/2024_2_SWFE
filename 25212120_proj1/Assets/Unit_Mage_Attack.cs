using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Mage_Attack : MonoBehaviour
{
    BaseUnit unitStat;

    private void Awake()
    {
        unitStat = GetComponentInParent<BaseUnit>();
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseMonster monster = other.gameObject.GetComponent<BaseMonster>();
        if (monster != null)
        {
            unitStat.Attack(monster);
        }
    }
}
