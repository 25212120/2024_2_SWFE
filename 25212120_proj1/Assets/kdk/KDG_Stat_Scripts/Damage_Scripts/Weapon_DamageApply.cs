using UnityEngine;

public class Weapon_DamageApply : MonoBehaviour
{
    private float knockBackForce = 5f;

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
            Vector3 knockBackDirection = (monster.transform.position - playerStat.transform.position).normalized;
            monster.GetComponent<Enemy>().agent.enabled = false;
            monster.GetComponent<Rigidbody>().AddForce(knockBackDirection * knockBackForce, ForceMode.Impulse);
            monster.GetComponent<Enemy>().agent.enabled = true;

            monster.GetComponent<Enemy>().isHit = true;
            playerStat.Attack(monster);
        }
        BaseUnit unit = GetComponentInParent<BaseUnit>();
        if (unit != null)
        {
            unit.Attack(monster);
        }

    }
}
