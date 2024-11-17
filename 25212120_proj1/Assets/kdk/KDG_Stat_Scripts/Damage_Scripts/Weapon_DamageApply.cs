using UnityEngine;

public class Weapon_DamageApply : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BaseMonster monster = other.GetComponent<BaseMonster>();
        if (monster != null) HandleAttack(monster);
    }

    private void HandleAttack(BaseMonster monster)
    {
        PlayerStat playerStat = GetComponentInParent<PlayerStat>(); 
        if (playerStat != null)
        {
            playerStat.Attack(monster);
        }

    }
    private void HandleAttack(BaseUnit unit)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(unit);  
        }
    }
}
