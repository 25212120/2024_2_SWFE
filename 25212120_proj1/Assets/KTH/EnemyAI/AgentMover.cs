using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    public Transform coreTarget;  // 코어의 Transform
    public float detectionRadius = 30f; // 유닛이나 오브젝트를 감지할 거리
    public string enemyTag = "unit";  // 공격할 유닛의 태그
    public float rotationSpeed = 5f;   // 회전 속도

    private NavMeshAgent agent;
    private Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;  // 자동 회전 비활성화
        target = coreTarget;           // 기본 목표를 코어로 설정
    }

    void Update()
    {
        // 주변에서 공격할 유닛을 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        // 디버그: 감지된 모든 콜라이더 이름 출력
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

        // 가까운 적이 감지되면 그쪽으로 이동, 없으면 코어로 이동
        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
        else
        {
            target = coreTarget;
        }

        Debug.Log("Current Target: " + target.name);

        // 타겟 위치로 NavMeshAgent를 이동
        agent.SetDestination(target.position);

        // 타겟을 향해 회전
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // 목표 지점에 도달하면 미끄러짐 방지
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.velocity = Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 감지 반경 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
