using UnityEngine;

public class Weapon_Monster_DamageApply : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // �浹�� ����� PlayerStat�̳� BaseStructure���� Ȯ��
        PlayerInputManager playerInputManager = collision.gameObject.GetComponent<PlayerInputManager>();
        PlayerStat playerStat = collision.gameObject.GetComponent<PlayerStat>();
        if (playerStat != null)
        {
            // ���Ͱ� �÷��̾�� ������ �ֱ�
            playerInputManager.isHit = true;
            HandleAttack(playerStat);
            return;
        }

        BaseStructure structure = collision.gameObject.GetComponent<BaseStructure>();
        if (structure != null)
        {
            // ���Ͱ� ���������� ������ �ֱ�
            HandleAttack(structure);
            return;
        }

        BaseUnit unit = collision.gameObject.GetComponent<BaseUnit>();
        if (structure != null)
        {
            // ���Ͱ� ���������� ������ �ֱ�
            HandleAttack(unit);
            return;
        }
        // �÷��̾ �������� �ƴ� �ٸ� ��ü���� �浹���� �������� ���� ����
    }

    private void HandleAttack(PlayerStat playerStat)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(playerStat);  // ���Ͱ� �÷��̾ ����
        }
    }

    private void HandleAttack(BaseStructure structure)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(structure);  // ���Ͱ� �������� ����
        }
    }
    private void HandleAttack(BaseUnit unit)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(unit);  // ���Ͱ� ������ ����
        }
    }
}
