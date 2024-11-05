using UnityEngine;
using UnityEngine.AI;

public class AgentAttack : MonoBehaviour
{
    public float attackRange = 5f;         // 공격 범위
    public float attackDamage = 10f;       // 공격 데미지
    public float attackCooldown = 1f;      // 공격 쿨다운 시간

    private float lastAttackTime = 0f;     // 마지막 공격 시간
    private AgentAI agentAI;               // 에이전트 AI 스크립트
    private NavMeshAgent navAgent;         // NavMeshAgent 컴포넌트

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
                // 공격 범위 내에 있으므로 에이전트 멈춤
                if (!navAgent.isStopped)
                {
                    navAgent.isStopped = true;
                    navAgent.velocity = Vector3.zero;
                }

                Attack(target);
            }
            else
            {
                // 공격 범위 밖이므로 에이전트 이동 재개
                if (navAgent.isStopped)
                {
                    navAgent.isStopped = false;
                }
            }
        }
        else
        {
            // 타겟이 없으면 에이전트 이동 재개
            if (navAgent.isStopped)
            {
                navAgent.isStopped = false;
            }
        }
    }

    private void Attack(Transform target)
    {
        // 공격 쿨다운 확인
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;

            // 타겟에 데미지 적용
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth.currentHealth >0)
            {
                targetHealth.TakeDamage(attackDamage);
                Debug.Log($"타겟 {target.name}에게 {attackDamage} 데미지를 입혔습니다. 남은 체력: {targetHealth.currentHealth}");
            }
            else
            {
                Debug.LogWarning($"타겟 {target.name}에 Health 컴포넌트가 없습니다.");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // 공격 범위 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
