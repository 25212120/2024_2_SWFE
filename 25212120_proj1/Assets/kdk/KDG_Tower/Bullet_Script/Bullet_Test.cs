using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_Test : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private PlayerStat stat;
    private Vector3 targetPosition;  // 목표 위치 저장

    public float speed = 70f;
    public GameObject impactEffect;

    public void SetTower(BaseStructure _tower)
    {
        tower = _tower;
    }
    public void SetPlayer(PlayerStat _player)
    {
        stat = _player;
    }
    public void Seek(Transform _target)
    {
        target = _target;
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

        // 목표 위치로 가는 방향 계산
        Vector3 dir = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;



        // 목표 방향으로 이동
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }


    void OnTriggerEnter(Collider other)
    {
        BaseMonster targetMonster = other.gameObject.GetComponent<BaseMonster>();
        if (targetMonster != null)
        {
            stat.Attack(targetMonster); // 구조물이 BaseMonster를 공격하도록 함
        }
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);


        Destroy(gameObject);
    }

}