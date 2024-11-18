using UnityEngine;

public class Bullet_Explode : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // 목표 위치 저장

    public float speed = 70f;           // 발사 속도
    public float explosionRadius = 5f;  // 폭발 반경
    public float explosionDamage = 50f; // 폭발 데미지
    public float rotationSpeed = 5f;    // 회전 속도
    public GameObject impactEffect;     // 충돌 이펙트
    public float gravity = -9.81f;      // 중력 (필요시 조정)
    public float maxDistanceToTarget = 100f; // 목표 지점과 너무 멀어지면 폭발

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
        rb.useGravity = true;  // 중력 적용

        // 목표 방향 계산
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        // 포물선 궤적을 위한 y축 초기 속도 계산
        float velocityY = Mathf.Sqrt(2 * Mathf.Abs(gravity) * distance);  // y축 속도 계산

        // 포물선 궤적을 위한 초기 속도 설정
        Vector3 velocity = direction.normalized * speed;
        velocity.y = velocityY;  // y축 속도 설정

        rb.velocity = velocity;  // 초기 속도 적용
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

        // 목표에 가까워지면 속도 0으로 설정하고 폭발
        if (distanceToTarget < 1f)
        {
            Explode();
            Destroy(gameObject);
            return;
        }

        // 목표와 너무 멀어지면 폭발
        if (distanceToTarget > maxDistanceToTarget)
        {
            Explode();
            Destroy(gameObject);
            return;
        }

        // 목표를 향해 계속 이동
        Vector3 newVelocity = direction.normalized * speed;
        newVelocity.y = rb.velocity.y;  // y축 속도 유지
        rb.velocity = newVelocity;

        // 목표를 향해 회전
        RotateTowardsTarget(direction);
    }

    // 목표를 향해 회전하는 함수
    private void RotateTowardsTarget(Vector3 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // 폭발 처리
    private void Explode()
    {
        // 주변 적들을 감지하여 폭발 범위 내 모든 적들에게 데미지 적용
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        bool explodedOnTarget = false;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseMonster targetMonster = collider.GetComponent<BaseMonster>();
                if (targetMonster != null)
                {
                    // 기본 폭발 데미지 적용
                    targetMonster.TakeDamage(explosionDamage);

                    // 타겟에 대해서도 공격 처리
                    if (target != null && collider.transform == target)
                    {
                        tower.Attack(targetMonster);
                        explodedOnTarget = true;
                    }
                }
            }
        }

        // 폭발 효과 생성
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);
    }

    // 충돌 시 폭발 처리
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            BaseMonster targetMonster = collision.gameObject.GetComponent<BaseMonster>();

            if (targetMonster != null)
            {
                // 타워가 몬스터를 공격하도록 함
                tower.Attack(targetMonster);
            }
            else
            {
                Debug.LogWarning("No BaseMonster component found on " + collision.gameObject.name);
            }

            // 충돌 시 폭발 처리
            Explode();

            // 총알 삭제
            Destroy(gameObject);
        }
    }
}
