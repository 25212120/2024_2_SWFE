using UnityEngine;

public class Bullet_Explosive : MonoBehaviour, IBullet
{
    private Vector3 targetPosition;
    private BaseStructure tower;

    public float speed = 70f;
    public GameObject impactEffect;
    public float explosionRadius = 5f;  // ���� ����
    public int explosionDamage = 50;    // ���� ����

    public void SetTower(BaseStructure _tower)
    {
        tower = _tower;
    }

    public void Seek(Transform _target)
    {
        // �������� �����Ƿ� ������ ����
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



    void OnTriggerEnter(Collider collision)
    {
        // �浹 ȿ�� ����
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

        // ���� ���� ���� ���� ó��
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseMonster targetMonster = collider.GetComponent<BaseMonster>();
                if (targetMonster != null)
                {
                    //���� �¾��� �� ������
                    tower.Attack(targetMonster);
                    // ���� ���ش� ���� ���
                    targetMonster.TakeDamage(explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }
}
