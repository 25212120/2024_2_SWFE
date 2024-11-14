using UnityEngine;

public class Bullet_Explosive : MonoBehaviour, IBullet
{
    private Vector3 targetPosition;
    private BaseStructure tower;

    public float speed = 70f;
    public GameObject impactEffect;
    public float explosionRadius = 5f;  // 폭발 범위
    public int explosionDamage = 50;    // 폭발 피해

    public void SetTower(BaseStructure _tower)
    {
        tower = _tower;
    }

    public void Seek(Transform _target)
    {
        // 추적하지 않으므로 사용되지 않음
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void Update()
    {
        if (targetPosition == null)
        {
            Destroy(gameObject);  // 목표가 없으면 총알 삭제
            return;
        }

        Vector3 dir = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;


        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }



    void OnTriggerEnter(Collider collision)
    {
        // 충돌 효과 생성
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

        // 폭발 범위 내의 적들 처리
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                BaseMonster targetMonster = collider.GetComponent<BaseMonster>();
                if (targetMonster != null)
                {
                    //직접 맞았을 때 데미지
                    tower.Attack(targetMonster);
                    // 폭발 피해는 따로 계산
                    targetMonster.TakeDamage(explosionDamage);
                }
            }
        }

        Destroy(gameObject);
    }
}
