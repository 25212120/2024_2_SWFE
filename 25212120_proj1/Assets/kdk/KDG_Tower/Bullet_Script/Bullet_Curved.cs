using TMPro;
using UnityEngine;

public class Bullet_Curved : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // 목표 위치 저장

    public float speed = 70f;         // 발사 속도
    public float gravity = -9.81f;   // 중력 (필요 시 조정)
    public float rotationSpeed = 5f; // 회전 속도 조정
    public GameObject impactEffect;  // 충돌 이펙트

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
            Destroy(gameObject, 1);
        }
        else
        {
            // 목표를 향해 계속 이동 (방향만 업데이트)
            Vector3 newVelocity = direction.normalized * speed;
            newVelocity.y = rb.velocity.y;  
            rb.velocity = newVelocity;  // 새로운 속도 적용
        }

        RotateTowardsTarget(direction);
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
                // BaseMonster가 없으면 디버그 메시지 출력
                Debug.LogWarning("No BaseMonster component found on " + collision.gameObject.name);
            }

            // 충돌 효과 생성
            GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectIns, 1f);

            Destroy(gameObject);
        }
    }

}
