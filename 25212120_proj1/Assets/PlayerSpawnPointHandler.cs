using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawnPointHandler : MonoBehaviour
{
    public float coreDistanceThreshold = 30f;
    private EnemySpawner[] playerSpawners;

    private void Start()
    {
        playerSpawners = GetComponentsInChildren<EnemySpawner>();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerProximityToCore += HandleCoreProximity;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerProximityToCore -= HandleCoreProximity;
    }
    public void HandleCoreProximity(GameObject nexus)
    {
        float distance = Vector3.Distance(transform.position, nexus.transform.position);
        bool isClose = distance <= coreDistanceThreshold;

        foreach (var spawnPoint in playerSpawners)
        {
            spawnPoint.SetSpawner(!isClose);
        }
    }
}
