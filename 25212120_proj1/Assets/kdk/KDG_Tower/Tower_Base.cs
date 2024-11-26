using UnityEngine;
using System.Linq; // LINQ�� ����Ͽ� ����Ʈ ����
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
    public float rotationTolerance = 5f;  // ��ǥ�� �ٶ󺸰� �־�� �ϴ� �ּ� ���� ���� (degrees)

    [Header("Targeting Settings")]
    public int maxTargetsToAttack = 3;  // �� ���� ������ �ִ� ���� ��

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

        // ���� ���� ��� ���� ã�� ����Ʈ�� �߰�
        foreach (GameObject enemy in enemies)
        {
            Vector3 flatEnemyPosition = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z);
            float distanceToEnemy = Vector3.Distance(transform.position, flatEnemyPosition);

            if (distanceToEnemy <= range)
            {
                enemiesInRange.Add(enemy);
            }
        }

        // ����� ������ ���� (�Ÿ� ����)
        enemiesInRange = enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).ToList();

        // ���� ����� ������ target�� �Ҵ�
        if (enemiesInRange.Count > 0)
        {
            target = enemiesInRange[0].transform; // ���� ����� ���� �켱������ ����
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

        // Y���� �����ϰ� X, Z ��鿡���� ȸ��
        Vector3 dir = new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position;

        // ��ǥ�� �ٶ󺸴� ȸ�� (Y���� ������ ȸ��)
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        partToRotate.rotation = Quaternion.Slerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed);

        // ��ǥ�� �ٶ󺸰� �ִ��� Ȯ�� (���� ���� üũ)
        float angleToTarget = Vector3.Angle(partToRotate.forward, dir);

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
        // ���� ���� ������ ã��
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        var enemiesInRange = new System.Collections.Generic.List<GameObject>();

        // ���� ���� ���鸸 ���͸�
        foreach (GameObject enemy in enemies)
        {
            Vector3 flatEnemyPosition = new Vector3(enemy.transform.position.x, transform.position.y, enemy.transform.position.z);
            float distanceToEnemy = Vector3.Distance(transform.position, flatEnemyPosition);

            if (distanceToEnemy <= range)
            {
                enemiesInRange.Add(enemy);
            }
        }

        // ����� ������ ����
        enemiesInRange = enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).ToList();

        // �ִ� ������ ���� ����ŭ Shoot �Լ� �ߵ�
        for (int i = 0; i < Mathf.Min(maxTargetsToAttack, enemiesInRange.Count); i++)
        {
            GameObject enemy = enemiesInRange[i];
            if (GameSettings.IsMultiplayer == false)
            {
                // �Ѿ� �߻�
                GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

                var bulletScript = bulletGO.GetComponent<IBullet>();

                if (bulletScript != null)
                {
                    bulletScript.SetTower(tower); // Ÿ���� �Ѿ˿� ����
                    bulletScript.Seek(enemy.transform); // ��ǥ ����
                    bulletScript.SetTargetPosition(enemy.transform.position); // ��ǥ ��ġ ����
                }
            }
            if(GameSettings.IsMultiplayer == true)
            {
                // �Ѿ� �߻�
                GameObject bulletGO = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);

                var bulletScript = bulletGO.GetComponent<IBullet>();

                if (bulletScript != null)
                {
                    bulletScript.SetTower(tower); // Ÿ���� �Ѿ˿� ����
                    bulletScript.Seek(enemy.transform); // ��ǥ ����
                    bulletScript.SetTargetPosition(enemy.transform.position); // ��ǥ ��ġ ����
                }
            }
        }
    }
}
