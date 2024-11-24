using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    LayerMask spawnLayer;

    private void Awake()
    {
        spawnLayer = LayerMask.GetMask("Spawn");
    }

    private void Start()
    {
        GenerateSpawnPoints();

        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 80f, spawnLayer);
        foreach(Collider collider in colliders)
        {
            if (Vector3.Distance(transform.position, collider.transform.position) >= 60f)
            {
                collider.gameObject.SetActive(true);
            }
        }
    }

    public GameObject spawnPrefab; // ������ ������
    public float radius = 30f;     // ���� ������
    public int angleStep = 20;     // ���� ����


    void GenerateSpawnPoints()
    {
        // 360���� ���� �������� ����
        for (int angle = 0; angle < 360; angle += angleStep)
        {
            // ������ �������� ��ȯ
            float radian = angle * Mathf.Deg2Rad;

            // �� ���� ��ǥ ���
            Vector3 spawnPosition = new Vector3(
                Mathf.Cos(radian) * radius,
                0f, // Y���� 0���� ���� (2D ��� ��)
                Mathf.Sin(radian) * radius
            );

            // ���� ����Ʈ ����
            GameObject instantiatedSpawner = Instantiate(spawnPrefab, spawnPosition + transform.position, Quaternion.identity);
            instantiatedSpawner.transform.SetParent(transform, false);
        }
    }

    private void OnDrawGizmos()
    {
        // ���� �� (������ 30)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 30f);

        // ū �� (������ 60)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 60f);


        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 80f);
    }
}
