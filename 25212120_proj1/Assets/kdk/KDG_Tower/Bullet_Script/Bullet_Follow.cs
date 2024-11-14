using TMPro;
using UnityEngine;

public class Bullet_Follow : MonoBehaviour, IBullet
{
    private Transform target;
    private BaseStructure tower;
    private Vector3 targetPosition;  // ��ǥ ��ġ ����

    public float speed = 70f;
    public GameObject impactEffect;

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

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;



        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }



    // �浹 �� ȣ��Ǵ� �Լ�
    void OnTriggerEnter(Collider collision) 
    {
        BaseMonster targetMonster = collision.gameObject.GetComponent<BaseMonster>();
        if (targetMonster != null)
        {
            tower.Attack(targetMonster); // �������� BaseMonster�� �����ϵ��� ��
        }
        // �浹 ȿ�� ����
        GameObject effectIns = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 1f);


        Destroy(gameObject);
    }
}
