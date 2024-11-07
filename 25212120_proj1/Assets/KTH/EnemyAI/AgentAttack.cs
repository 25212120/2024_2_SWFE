using UnityEngine;
using UnityEngine.AI;

public class AgentAttack : MonoBehaviour
{
    public float attackRange = 5f;         // ���� ����
    public float attackDamage = 10f;       // ���� ������
    public float attackCooldown = 1f;      // ���� ��ٿ� �ð�

    private float lastAttackTime = 0f;     // ������ ���� �ð�
    private AgentAI agentAI;               // ������Ʈ AI ��ũ��Ʈ
    private NavMeshAgent navAgent;         // NavMeshAgent ������Ʈ

    void Start()
    {
        agentAI = GetComponent<AgentAI>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        Transform target = agentAI.GetCurrentTarget();

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange)
            {
                // ���� ���� ���� �����Ƿ� ������Ʈ ����
                if (!navAgent.isStopped)
                {
                    navAgent.isStopped = true;
                    navAgent.velocity = Vector3.zero;
                }

                Attack(target);
            }
            else
            {
                // ���� ���� ���̹Ƿ� ������Ʈ �̵� �簳
                if (navAgent.isStopped)
                {
                    navAgent.isStopped = false;
                }
            }
        }
        else
        {
            // Ÿ���� ������ ������Ʈ �̵� �簳
            if (navAgent.isStopped)
            {
                navAgent.isStopped = false;
            }
        }
    }

    private void Attack(Transform target)
    {
        // ���� ��ٿ� Ȯ��
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // Ÿ�ٿ� ������ ����
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth.currentHealth >0)
            {
                targetHealth.TakeDamage(attackDamage);
                Debug.Log($"Ÿ�� {target.name}���� {attackDamage} �������� �������ϴ�. ���� ü��: {targetHealth.currentHealth}");
            }
            else
            {
                Debug.LogWarning($"Ÿ�� {target.name}�� Health ������Ʈ�� �����ϴ�.");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // ���� ���� ǥ��
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
