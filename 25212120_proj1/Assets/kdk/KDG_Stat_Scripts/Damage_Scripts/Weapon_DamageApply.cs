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
            Debug.Log("HANDLEATTACK");
            playerStat.Attack(monster);
        }

    }
}
