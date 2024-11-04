using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    public Transform coreTarget;  // �ھ��� Transform
    public float detectionRadius = 30f; // �����̳� ������Ʈ�� ������ �Ÿ�
    public string enemyTag = "unit";  // ������ ������ �±�
    public float rotationSpeed = 5f;   // ȸ�� �ӵ�

    private NavMeshAgent agent;
    private Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;  // �ڵ� ȸ�� ��Ȱ��ȭ
        target = coreTarget;           // �⺻ ��ǥ�� �ھ�� ����
    }

    void Update()
    {
        // �ֺ����� ������ ������ ����
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // �����: ������ ��� �ݶ��̴� �̸� ���
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("Detected Object: " + hitCollider.name);
        }

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.transform;
                }
            }
        }

        // ����� ���� �����Ǹ� �������� �̵�, ������ �ھ�� �̵�
        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
        else
        {
            target = coreTarget;
        }

        Debug.Log("Current Target: " + target.name);

        // Ÿ�� ��ġ�� NavMeshAgent�� �̵�
        agent.SetDestination(target.position);

        // Ÿ���� ���� ȸ��
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // ��ǥ ������ �����ϸ� �̲����� ����
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.velocity = Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        // ���� �ݰ� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
