using UnityEngine;

public class Tower_Base : MonoBehaviour
{
    public Transform target;
    private BaseStructure tower;

    [Header("Attributes")]
    public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public Transform partToRotate;
    public float turnSpeed = 10f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float rotationTolerance = 5f;  // ��ǥ�� �ٶ󺸰� �־�� �ϴ� �ּ� ���� ���� (degrees)

    private void Awake()
    {
        tower = GetComponent<BaseStructure>();
        if (tower == null )
        {
            return;
        }
    }
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null)
            return;

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // ��ǥ�� �ٶ󺸰� �ִ��� Ȯ�� (���� ���� üũ)
        float angleToTarget = Vector3.Angle(transform.forward, dir);

        if (angleToTarget <= rotationTolerance) // ��ǥ�� ��Ȯ�� �ٶ󺸰� ������
        {
            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var bulletScript = bulletGO.GetComponent<IBullet>(); // IBullet �������̽��� ����� �������� ã��

        if (bulletScript != null)
        {
            bulletScript.SetTower(tower); // Ÿ���� �Ѿ˿� ����
            bulletScript.Seek(target); // ��ǥ ����
            bulletScript.SetTargetPosition(target.position);  // ��ǥ ��ġ�� �Ѿ˿� ����

        }
    }

}
