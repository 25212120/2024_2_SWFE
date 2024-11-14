using UnityEngine;

public class Weapon_DamageApply : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        BaseMonster monster = collision.gameObject.GetComponent<BaseMonster>();
        if (monster != null)
        {
            HandleAttack(monster);
        }
    }

    private void HandleAttack(BaseMonster monster)
    {
        PlayerStat playerStat = GetComponentInParent<PlayerStat>(); 
        if (playerStat != null)
        {
            playerStat.Attack(monster);
        }

    }
}
