using TMPro;
using UnityEngine;

public class Bullet_Curved : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // ��ǥ ��ġ ����

    public float speed = 70f;         // �߻� �ӵ�
    public float gravity = -9.81f;   // �߷� (�ʿ� �� ����)
    public float rotationSpeed = 5f; // ȸ�� �ӵ� ����
    public GameObject impactEffect;  // �浹 ����Ʈ

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
            Destroy(gameObject, 1);
        }
        else
        {
            // ��ǥ�� ���� ��� �̵� (���⸸ ������Ʈ)
            Vector3 newVelocity = direction.normalized * speed;
            newVelocity.y = rb.velocity.y;  
            rb.velocity = newVelocity;  // ���ο� �ӵ� ����
        }

        RotateTowardsTarget(direction);
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
                // BaseMonster�� ������ ����� �޽��� ���
                Debug.LogWarning("No BaseMonster component found on " + collision.gameObject.name);
            }

            // �浹 ȿ�� ����
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 1f);

            Destroy(gameObject);
        }
    }

}
