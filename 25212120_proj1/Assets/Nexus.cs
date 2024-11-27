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

    public GameObject spawnPrefab; // 생성할 프리팹
    public float radius = 30f;     // 원의 반지름
    public int angleStep = 40;     // 각도 간격


    void GenerateSpawnPoints()
    {

        for (int angle = 0; angle < 360; angle += angleStep)
        {
            float radian = angle * Mathf.Deg2Rad;

            Vector3 spawnPosition = new Vector3(
                Mathf.Cos(radian) * radius,
                0f, // Y축은 0으로 고정 (2D 평면 상)
                Mathf.Sin(radian) * radius
            );

            // 스폰 포인트 생성
            GameObject instantiatedSpawner = Instantiate(spawnPrefab, spawnPosition + transform.position, Quaternion.identity);
            instantiatedSpawner.transform.SetParent(transform, false);
        }
    }
}
