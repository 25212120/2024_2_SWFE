using UnityEngine;

public class Bullet_Heal : MonoBehaviour, IBullet
{
    private Vector3 targetPosition;
    private BaseStructure tower;

    public float speed = 70f;
    public GameObject impactEffect;
    public float healAmount = 10f; // 힐 양
    public float healRadius = 5f; // 힐 범위

    public void SetTower(BaseStructure _tower)
    {
        tower = _tower;
    }

    public void Seek(Transform _target)
    {
        // 목표를 추적하는 기능은 필요 없으므로 구현하지 않음
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


    void OnCollisionEnter(Collision collision)
    {
        BaseMonster targetMonster = collision.gameObject.GetComponent<BaseMonster>();
        if (targetMonster != null)
        {
            tower.Attack(targetMonster); // 구조물이 BaseMonster를 공격하도록 함
        }
        Destroy(gameObject);

        // 힐 범위 내의 플레이어들에게 힐을 주는 로직
        Collider[] colliders = Physics.OverlapSphere(transform.position, healRadius);
        foreach (var collider in colliders)
        {
            // 플레이어 캐릭터만 찾기
            PlayerStat playerStat = collider.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                playerStat.Heal(healAmount); // 힐 적용
                Debug.Log($"힐을 받은 플레이어: {playerStat.name} | 힐 양: {healAmount}");
            }
        }
        // 충돌 효과 생성
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);

    }
}
