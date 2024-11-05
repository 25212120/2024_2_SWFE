using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    public Transform coreTarget;        // 코어의 Transform
    public float detectionRadius = 30f; // 감지 반경
    public float attackRange = 5f;      // 공격 범위
    public float rotationSpeed = 100f;    // 회전 속도

    private NavMeshAgent agent;
    private Transform target;
    private bool isAttacking = false;   // 공격 중인지 여부

    private string[] priorityTags = { "unit", "tower", "Player" }; // 타겟 목록

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // 자동 회전 비활성화
        target = null;                  // 초기 타겟 없음
    }

    void Update()
    {
        // 현재 타겟이 없거나 파괴된 경우
        if (target == null)
        {
            isAttacking = false;
            FindPriorityTarget();

            // 여전히 타겟이 없으면 코어로 설정
            if (target == null)
            {
                target = coreTarget;
                Debug.Log("타겟이 없으므로 코어로 설정합니다.");
            }
        }
        else
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 공격 범위 내에 있으면 공격 중으로 설정
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

        // NavMeshAgent를 타겟 위치로 이동
        agent.SetDestination(target.position);

        // 타겟을 향해 회전
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // 목적지에 도달했을 때 미끄러짐 방지
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

        // 타겟이 없으면 null로 설정하여 코어로 이동하도록 함
        target = null;

    }

    void OnDrawGizmosSelected()
    {
        // 감지 반경 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 공격 범위 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // 현재 타겟을 반환하는 함수 (공격 스크립트에서 사용)
    public Transform GetCurrentTarget()
    {
        return target;
    }
}
