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
    public float rotationTolerance = 5f;  // 목표를 바라보고 있어야 하는 최소 각도 차이 (degrees)

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

        // 목표를 바라보고 있는지 확인 (각도 차이 체크)
        float angleToTarget = Vector3.Angle(transform.forward, dir);

        if (angleToTarget <= rotationTolerance) // 목표를 정확히 바라보고 있으면
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
        var bulletScript = bulletGO.GetComponent<IBullet>(); // IBullet 인터페이스를 사용해 동적으로 찾음

        if (bulletScript != null)
        {
            bulletScript.SetTower(tower); // 타워를 총알에 전달
            bulletScript.Seek(target); // 목표 설정
            bulletScript.SetTargetPosition(target.position);  // 목표 위치를 총알에 전달

        }
    }

}
