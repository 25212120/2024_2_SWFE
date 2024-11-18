using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_Test : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private PlayerStat stat;
    private Vector3 targetPosition;  // ��ǥ ��ġ ����

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
            Destroy(gameObject);  // ��ǥ�� ������ �Ѿ� ����
            return;
        }

        // ��ǥ ��ġ�� ���� ���� ���
        Vector3 dir = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;



        // ��ǥ �������� �̵�
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }


    void OnTriggerEnter(Collider other)
    {
        BaseMonster targetMonster = other.gameObject.GetComponent<BaseMonster>();
        if (targetMonster != null)
        {
            stat.Attack(targetMonster); // �������� BaseMonster�� �����ϵ��� ��
        }
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);


        Destroy(gameObject);
    }

}