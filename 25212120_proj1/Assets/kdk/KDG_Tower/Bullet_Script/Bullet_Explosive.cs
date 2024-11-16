using UnityEngine;

public class Bullet_Explosive : MonoBehaviour, IBullet
{
    public float explosionRadius = 5f;  // 폭발 범위
    public int explosionDamage = 10;    // 폭발 피해

    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // 목표 위치 저장

    public float speed = 70f;         // 발사 속도
    public float gravity = -9.81f;   // 중력 (필요 시 조정)
    public float rotationSpeed = 5f; // 회전 속도 조정
    public GameObject impactEffect;  // 충돌 이펙트

    private Rigidbody rb;

    private bool hasExploded = false; // 폭발이 한번만 발생하도록 체크하는 변수

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
        // Rigidbody를 가져오고 중력을 적용
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // Rigidbody에서 물리 엔진을 사용하도록 설정

        // 목표를 향해 초기 속도 계산
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        float velocityY = Mathf.Sqrt(2 * Mathf.Abs(gravity) * distance);  // y축 초기 속도 계산

        // 포물선 궤적을 위해 초기 속도를 계산
        Vector3 velocity = direction.normalized * speed;
        velocity.y = velocityY;  // y축 속도 설정

        rb.velocity = velocity;  // Rigidbody에 초기 속도 설정
    }

    void Update()
    {
        if (targetPosition == null)
        {
            Destroy(gameObject);  // 목표가 없으면 총알 삭제
            return;
        }

        Vector3 direction = targetPosition - transform.position;
        float distanceToTarget = direction.magnitude;

        // 목표에 가까워지면 속도 조정
        if (distanceToTarget < 1f)
        {
            rb.velocity = Vector3.zero; // 속도 0으로 설정하여 정지
            transform.position = targetPosition; // 목표 위치로 정확히 이동

            // 목표 위치에 도달했을 때, 타겟이 없으면 그래도 폭발 처리
            ExplodeAtPosition();
        }
        else
        {
            // 목표를 향해 계속 이동 (방향만 업데이트)
            Vector3 newVelocity = direction.normalized * speed;
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;  // 새로운 속도 적용
        }

        RotateTowardsTarget(direction);
        if (distanceToTarget > 50f)
        {
            Destroy(gameObject);
        }
    }

    // 목표를 향해 회전하는 함수
    private void RotateTowardsTarget(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            // 목표 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // 목표 위치에 도달했을 때 폭발 처리
    private void ExplodeAtPosition()
    {
        // 폭발 효과 생성
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

        // 폭발 범위 내의 적들 처리 (직접 맞은 적 제외)
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        bool explodedOnTarget = false; // 폭발이 타겟에 대해 발생했는지 체크

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseMonster targetMonster = collider.GetComponent<BaseMonster>();
                if (targetMonster != null)
                {
                    // 폭발 피해는 따로 계산
                    targetMonster.TakeDamage(explosionDamage);

                    // 만약 이 적이 우리가 설정한 타겟이라면, tower.Attack()을 호출
                    if (target != null && collider.transform == target && !hasExploded)
                    {
                        tower.Attack(targetMonster);
                        explodedOnTarget = true;
                        hasExploded = true; // 타겟에 대한 폭발 처리가 한 번만 발생하도록 설정
                    }
                }
            }
        }

        // 타겟이 없거나 폭발이 타겟에 대해 발생하지 않았다면, 그냥 폭발만 발생
        if (!explodedOnTarget)
        {
            // 타겟이 없으면, 폭발만 발생시키고 Attack은 하지 않음
            if (target == null)
            {
                tower = null; // 타워가 null인 경우 더 이상 공격을 하지 않도록 설정
            }
        }

        // 총알 삭제
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        // 충돌한 적이 있으면
        if (collision.CompareTag("Enemy"))
        {
            // 충돌한 적을 찾고 처리
            BaseMonster targetMonster = collision.GetComponent<BaseMonster>();
            if (targetMonster != null)
            {
                // 직격 피해 처리 (Attack 처리)
                if (!hasExploded)
                {
                    tower.Attack(targetMonster);
                    hasExploded = true; // 직격 처리 후, hasExploded를 true로 설정하여 폭발과 중복 처리 방지
                }

                // 충돌한 적은 폭발 피해도 받음
                targetMonster.TakeDamage(explosionDamage);
            }
        }

        // 충돌 효과 생성
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

        // 총알 삭제
        Destroy(gameObject);
    }
}
