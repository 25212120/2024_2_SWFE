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
    public int angleStep = 40;     // ���� ����


    void GenerateSpawnPoints()
    {

        for (int angle = 0; angle < 360; angle += angleStep)
        {
            float radian = angle * Mathf.Deg2Rad;

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
}
