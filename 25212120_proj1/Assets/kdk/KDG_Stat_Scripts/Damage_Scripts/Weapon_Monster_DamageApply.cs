using UnityEngine;

public class Weapon_Monster_DamageApply : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // 충돌한 대상이 PlayerStat이나 BaseStructure인지 확인
        PlayerInputManager playerInputManager = collision.gameObject.GetComponent<PlayerInputManager>();
        PlayerStat playerStat = collision.gameObject.GetComponent<PlayerStat>();
        if (playerStat != null)
        {
            // 몬스터가 플레이어에게 데미지 주기
            playerInputManager.isHit = true;
            HandleAttack(playerStat);
            return;
        }

        BaseStructure structure = collision.gameObject.GetComponent<BaseStructure>();
        if (structure != null)
        {
            // 몬스터가 구조물에게 데미지 주기
            HandleAttack(structure);
            return;
        }

        BaseUnit unit = collision.gameObject.GetComponent<BaseUnit>();
        if (structure != null)
        {
            // 몬스터가 구조물에게 데미지 주기
            HandleAttack(unit);
            return;
        }
        // 플레이어나 구조물이 아닌 다른 객체와의 충돌에는 데미지를 주지 않음
    }

    private void HandleAttack(PlayerStat playerStat)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(playerStat);  // 몬스터가 플레이어를 공격
        }
    }

    private void HandleAttack(BaseStructure structure)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(structure);  // 몬스터가 구조물을 공격
        }
    }
    private void HandleAttack(BaseUnit unit)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(unit);  // 몬스터가 유닛을 공격
        }
    }
}
