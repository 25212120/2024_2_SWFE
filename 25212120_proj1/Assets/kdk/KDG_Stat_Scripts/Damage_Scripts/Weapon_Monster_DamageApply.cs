using UnityEngine;

public class Weapon_Monster_DamageApply : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // �浹�� ����� PlayerStat�̳� BaseStructure���� Ȯ��
        PlayerStat playerStat = collision.gameObject.GetComponent<PlayerStat>();
        if (playerStat != null)
        {
            // ���Ͱ� �÷��̾�� ������ �ֱ�
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
}
