using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    public Transform coreTarget;        // �ھ��� Transform
    public float detectionRadius = 30f; // ���� �ݰ�
    public float attackRange = 5f;      // ���� ����
    public float rotationSpeed = 100f;    // ȸ�� �ӵ�

    private NavMeshAgent agent;
    private Transform target;
    private bool isAttacking = false;   // ���� ������ ����

    private string[] priorityTags = { "unit", "tower", "Player" }; // Ÿ�� ���

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // �ڵ� ȸ�� ��Ȱ��ȭ
        target = null;                  // �ʱ� Ÿ�� ����
    }

    void Update()
    {
        // ���� Ÿ���� ���ų� �ı��� ���
        if (target == null)
        {
            isAttacking = false;
            FindPriorityTarget();

            // ������ Ÿ���� ������ �ھ�� ����
            if (target == null)
            {
                target = coreTarget;
                Debug.Log("Ÿ���� �����Ƿ� �ھ�� �����մϴ�.");
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // ���� ���� ���� ������ ���� ������ ����
            if (distanceToTarget <= attackRange)
            {
                isAttacking = true;
            }
            else
            {
                isAttacking = false;
                FindPriorityTarget();
            }
        }

        // NavMeshAgent�� Ÿ�� ��ġ�� �̵�
        agent.SetDestination(target.position);

        // Ÿ���� ���� ȸ��
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // �������� �������� �� �̲����� ����
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.velocity = Vector3.zero;
        }
    }

    private void FindPriorityTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (string tag in priorityTags)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag(tag))
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = hitCollider.transform;
                    }
                }
            }

            
        }
        target = closestTarget;
        if ((target))
        {
            return;
        }

        // Ÿ���� ������ null�� �����Ͽ� �ھ�� �̵��ϵ��� ��
        target = null;

    }

    void OnDrawGizmosSelected()
    {
        // ���� �ݰ� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // ���� ���� �ð�ȭ
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // ���� Ÿ���� ��ȯ�ϴ� �Լ� (���� ��ũ��Ʈ���� ���)
    public Transform GetCurrentTarget()
    {
        return target;
    }
}
