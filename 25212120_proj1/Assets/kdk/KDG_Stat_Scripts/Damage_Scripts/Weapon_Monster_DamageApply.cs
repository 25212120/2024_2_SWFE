using UnityEngine;

public class Weapon_Monster_DamageApply : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // �浹�� ����� PlayerStat�̳� BaseStructure���� Ȯ��
        PlayerInputManager playerInputManager = collision.gameObject.GetComponent<PlayerInputManager>();
        PlayerStat playerStat = collision.gameObject.GetComponent<PlayerStat>();
        BaseStructure structure = collision.gameObject.GetComponent<BaseStructure>();
        BaseUnit unit = collision.gameObject.GetComponent<BaseUnit>();

        if (playerStat != null)
        {
            // ���Ͱ� �÷��̾�� ������ �ֱ�
            playerInputManager.isHit = true;
            HandleAttack_1(playerStat);
            return;
        }

        else if (structure != null)
        {
            // ���Ͱ� ���������� ������ �ֱ�
            HandleAttack_2(structure);
            return;
        }

        else if (unit != null)
        {
            // ���Ͱ� ���������� ������ �ֱ�
            HandleAttack_3(unit);
            return;
        }
        // �÷��̾ �������� �ƴ� �ٸ� ��ü���� �浹���� �������� ���� ����
    }

    private void HandleAttack_1(PlayerStat playerStat)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(playerStat);  // ���Ͱ� �÷��̾ ����
        }
    }

    private void HandleAttack_2(BaseStructure structure)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(structure);  // ���Ͱ� �������� ����
        }
    }
    private void HandleAttack_3(BaseUnit unit)
    {
        BaseMonster monster = GetComponentInParent<BaseMonster>();
        if (monster != null)
        {
            monster.Attack(unit);  // ���Ͱ� ������ ����
        }
    }
}
