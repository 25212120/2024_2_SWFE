using UnityEngine;

public class Bullet_Heal : MonoBehaviour, IBullet
{
    private Vector3 targetPosition;
    private BaseStructure tower;

    public float speed = 70f;
    public GameObject impactEffect;
    public float healAmount = 10f; // �� ��
    public float healRadius = 5f; // �� ����

    public void SetTower(BaseStructure _tower)
    {
        tower = _tower;
    }

    public void Seek(Transform _target)
    {
        // ��ǥ�� �����ϴ� ����� �ʿ� �����Ƿ� �������� ����
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        if (targetPosition == null)
        {
            Destroy(gameObject);  // ��ǥ�� ������ �Ѿ� ����
            return;
        }

        Vector3 dir = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;



        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }


    void OnCollisionEnter(Collision collision)
    {
        BaseMonster targetMonster = collision.gameObject.GetComponent<BaseMonster>();
        if (targetMonster != null)
        {
            tower.Attack(targetMonster); // �������� BaseMonster�� �����ϵ��� ��
        }
        Destroy(gameObject);

        // �� ���� ���� �÷��̾�鿡�� ���� �ִ� ����
        Collider[] colliders = Physics.OverlapSphere(transform.position, healRadius);
        foreach (var collider in colliders)
        {
            // �÷��̾� ĳ���͸� ã��
            PlayerStat playerStat = collider.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                playerStat.Heal(healAmount); // �� ����
                Debug.Log($"���� ���� �÷��̾�: {playerStat.name} | �� ��: {healAmount}");
            }
        }
        // �浹 ȿ�� ����
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

    }
}
