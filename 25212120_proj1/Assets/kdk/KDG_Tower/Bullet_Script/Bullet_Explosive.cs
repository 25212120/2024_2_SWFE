using UnityEngine;

public class Bullet_Explode : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // ��ǥ ��ġ ����

    public float speed = 70f;           // �߻� �ӵ�
    public float explosionRadius = 5f;  // ���� �ݰ�
    public float explosionDamage = 50f; // ���� ������
    public float rotationSpeed = 5f;    // ȸ�� �ӵ�
    public GameObject impactEffect;     // �浹 ����Ʈ
    public float gravity = -9.81f;      // �߷� (�ʿ�� ����)
    public float maxDistanceToTarget = 100f; // ��ǥ ������ �ʹ� �־����� ����

    private Rigidbody rb;

    public void SetTower(BaseStructure _tower)
    {
        tower = _tower;
    }

    public void Seek(Transform _target)
    {
        target = _target;
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;  // �߷� ����

        // ��ǥ ���� ���
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        // ������ ������ ���� y�� �ʱ� �ӵ� ���
        float velocityY = Mathf.Sqrt(2 * Mathf.Abs(gravity) * distance);  // y�� �ӵ� ���

        // ������ ������ ���� �ʱ� �ӵ� ����
        Vector3 velocity = direction.normalized * speed;
        velocity.y = velocityY;  // y�� �ӵ� ����

        rb.velocity = velocity;  // �ʱ� �ӵ� ����
    }

    void Update()
    {
        if (targetPosition == null)
        {
            Destroy(gameObject);  // ��ǥ�� ������ �Ѿ� ����
            return;
        }

        Vector3 direction = targetPosition - transform.position;
        float distanceToTarget = direction.magnitude;

        // ��ǥ�� ��������� �ӵ� 0���� �����ϰ� ����
        if (distanceToTarget < 1f)
        {
            Explode();
            Destroy(gameObject);
            return;
        }

        // ��ǥ�� �ʹ� �־����� ����
        if (distanceToTarget > maxDistanceToTarget)
        {
            Explode();
            Destroy(gameObject);
            return;
        }

        // ��ǥ�� ���� ��� �̵�
        Vector3 newVelocity = direction.normalized * speed;
        newVelocity.y = rb.velocity.y;  // y�� �ӵ� ����
        rb.velocity = newVelocity;

        // ��ǥ�� ���� ȸ��
        RotateTowardsTarget(direction);
    }

    // ��ǥ�� ���� ȸ���ϴ� �Լ�
    private void RotateTowardsTarget(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ���� ó��
    private void Explode()
    {
        // �ֺ� ������ �����Ͽ� ���� ���� �� ��� ���鿡�� ������ ����
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        bool explodedOnTarget = false;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseMonster targetMonster = collider.GetComponent<BaseMonster>();
                if (targetMonster != null)
                {
                    // �⺻ ���� ������ ����
                    targetMonster.TakeDamage(explosionDamage);

                    // Ÿ�ٿ� ���ؼ��� ���� ó��
                    if (target != null && collider.transform == target)
                    {
                        tower.Attack(targetMonster);
                        explodedOnTarget = true;
                    }
                }
            }
        }

        // ���� ȿ�� ����
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);
    }

    // �浹 �� ���� ó��
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            BaseMonster targetMonster = collision.gameObject.GetComponent<BaseMonster>();

            if (targetMonster != null)
            {
                // Ÿ���� ���͸� �����ϵ��� ��
                tower.Attack(targetMonster);
            }
            else
            {
                Debug.LogWarning("No BaseMonster component found on " + collision.gameObject.name);
            }

            // �浹 �� ���� ó��
            Explode();

            // �Ѿ� ����
            Destroy(gameObject);
        }
    }
}
