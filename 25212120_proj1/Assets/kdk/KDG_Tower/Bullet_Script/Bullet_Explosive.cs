using UnityEngine;

public class Bullet_Explosive : MonoBehaviour, IBullet
{
    public float explosionRadius = 5f;  // ���� ����
    public int explosionDamage = 10;    // ���� ����

    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // ��ǥ ��ġ ����

    public float speed = 70f;         // �߻� �ӵ�
    public float gravity = -9.81f;   // �߷� (�ʿ� �� ����)
    public float rotationSpeed = 5f; // ȸ�� �ӵ� ����
    public GameObject impactEffect;  // �浹 ����Ʈ

    private Rigidbody rb;

    private bool hasExploded = false; // ������ �ѹ��� �߻��ϵ��� üũ�ϴ� ����

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
        // Rigidbody�� �������� �߷��� ����
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // Rigidbody���� ���� ������ ����ϵ��� ����

        // ��ǥ�� ���� �ʱ� �ӵ� ���
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        float velocityY = Mathf.Sqrt(2 * Mathf.Abs(gravity) * distance);  // y�� �ʱ� �ӵ� ���

        // ������ ������ ���� �ʱ� �ӵ��� ���
        Vector3 velocity = direction.normalized * speed;
        velocity.y = velocityY;  // y�� �ӵ� ����

        rb.velocity = velocity;  // Rigidbody�� �ʱ� �ӵ� ����
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

        // ��ǥ�� ��������� �ӵ� ����
        if (distanceToTarget < 1f)
        {
            rb.velocity = Vector3.zero; // �ӵ� 0���� �����Ͽ� ����
            transform.position = targetPosition; // ��ǥ ��ġ�� ��Ȯ�� �̵�

            // ��ǥ ��ġ�� �������� ��, Ÿ���� ������ �׷��� ���� ó��
            ExplodeAtPosition();
        }
        else
        {
            // ��ǥ�� ���� ��� �̵� (���⸸ ������Ʈ)
            Vector3 newVelocity = direction.normalized * speed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;  // ���ο� �ӵ� ����
        }

        RotateTowardsTarget(direction);
        if (distanceToTarget > 50f)
        {
            Destroy(gameObject);
        }
    }

    // ��ǥ�� ���� ȸ���ϴ� �Լ�
    private void RotateTowardsTarget(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            // ��ǥ �������� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ��ǥ ��ġ�� �������� �� ���� ó��
    private void ExplodeAtPosition()
    {
        // ���� ȿ�� ����
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

        // ���� ���� ���� ���� ó�� (���� ���� �� ����)
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        bool explodedOnTarget = false; // ������ Ÿ�ٿ� ���� �߻��ߴ��� üũ

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseMonster targetMonster = collider.GetComponent<BaseMonster>();
                if (targetMonster != null)
                {
                    // ���� ���ش� ���� ���
                    targetMonster.TakeDamage(explosionDamage);

                    // ���� �� ���� �츮�� ������ Ÿ���̶��, tower.Attack()�� ȣ��
                    if (target != null && collider.transform == target && !hasExploded)
                    {
                        tower.Attack(targetMonster);
                        explodedOnTarget = true;
                        hasExploded = true; // Ÿ�ٿ� ���� ���� ó���� �� ���� �߻��ϵ��� ����
                    }
                }
            }
        }

        // Ÿ���� ���ų� ������ Ÿ�ٿ� ���� �߻����� �ʾҴٸ�, �׳� ���߸� �߻�
        if (!explodedOnTarget)
        {
            // Ÿ���� ������, ���߸� �߻���Ű�� Attack�� ���� ����
            if (target == null)
            {
                tower = null; // Ÿ���� null�� ��� �� �̻� ������ ���� �ʵ��� ����
            }
        }

        // �Ѿ� ����
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        // �浹�� ���� ������
        if (collision.CompareTag("Enemy"))
        {
            // �浹�� ���� ã�� ó��
            BaseMonster targetMonster = collision.GetComponent<BaseMonster>();
            if (targetMonster != null)
            {
                // ���� ���� ó�� (Attack ó��)
                if (!hasExploded)
                {
                    tower.Attack(targetMonster);
                    hasExploded = true; // ���� ó�� ��, hasExploded�� true�� �����Ͽ� ���߰� �ߺ� ó�� ����
                }

                // �浹�� ���� ���� ���ص� ����
                targetMonster.TakeDamage(explosionDamage);
            }
        }

        // �浹 ȿ�� ����
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

        // �Ѿ� ����
        Destroy(gameObject);
    }
}
