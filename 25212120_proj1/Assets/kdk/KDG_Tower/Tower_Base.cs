using UnityEngine;
using System.Linq; // LINQ를 사용하여 리스트 정렬
using Photon.Pun;
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

    [Header("Targeting Settings")]
    public int maxTargetsToAttack = 3;  // 한 번에 공격할 최대 적의 수

    private void Awake()
    {
        tower = GetComponent<BaseStructure>();
        if (tower == null)
        {
            return;
        }
    }

    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
        PhotonNetwork.ConnectUsingSettings();

    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        var enemiesInRange = new System.Collections.Generic.List<GameObject>();

        // 범위 내의 모든 적을 찾아 리스트에 추가
        foreach (GameObject enemy in enemies)
        {
            Vector3 flatEnemyPosition = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z);
            float distanceToEnemy = Vector3.Distance(transform.position, flatEnemyPosition);

            if (distanceToEnemy <= range)
            {
                enemiesInRange.Add(enemy);
            }
        }

        // 가까운 적부터 정렬 (거리 기준)
        enemiesInRange = enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).ToList();

        // 가장 가까운 적들을 target에 할당
        if (enemiesInRange.Count > 0)
        {
            target = enemiesInRange[0].transform; // 가장 가까운 적을 우선적으로 설정
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

        // Y축을 무시하고 X, Z 평면에서만 회전
        Vector3 dir = new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position;

        // 목표를 바라보는 회전 (Y축을 무시한 회전)
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        partToRotate.rotation = Quaternion.Slerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed);

        // 목표를 바라보고 있는지 확인 (각도 차이 체크)
        float angleToTarget = Vector3.Angle(partToRotate.forward, dir);

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
        // 범위 내의 적들을 찾기
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        var enemiesInRange = new System.Collections.Generic.List<GameObject>();

        // 범위 내의 적들만 필터링
        foreach (GameObject enemy in enemies)
        {
            Vector3 flatEnemyPosition = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z);
            float distanceToEnemy = Vector3.Distance(transform.position, flatEnemyPosition);

            if (distanceToEnemy <= range)
            {
                enemiesInRange.Add(enemy);
            }
        }

        // 가까운 적부터 정렬
        enemiesInRange = enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).ToList();

        // 최대 공격할 적의 수만큼 Shoot 함수 발동
        for (int i = 0; i < Mathf.Min(maxTargetsToAttack, enemiesInRange.Count); i++)
        {
            GameObject enemy = enemiesInRange[i];
            if (GameSettings.IsMultiplayer == false)
            {
                // 총알 발사
                GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

                var bulletScript = bulletGO.GetComponent<IBullet>();

                if (bulletScript != null)
                {
                    bulletScript.SetTower(tower); // 타워를 총알에 전달
                    bulletScript.Seek(enemy.transform); // 목표 설정
                    bulletScript.SetTargetPosition(enemy.transform.position); // 목표 위치 설정
                }
            }
            if(GameSettings.IsMultiplayer == true)
            {
                // 총알 발사
                GameObject bulletGO = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

                var bulletScript = bulletGO.GetComponent<IBullet>();

                if (bulletScript != null)
                {
                    bulletScript.SetTower(tower); // 타워를 총알에 전달
                    bulletScript.Seek(enemy.transform); // 목표 설정
                    bulletScript.SetTargetPosition(enemy.transform.position); // 목표 위치 설정
                }
            }
        }
    }
}
