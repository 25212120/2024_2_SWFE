using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusSpawnPointHandler : MonoBehaviour
{
    public float spawnHideDistance = 5f;

    private EnemySpawner[] nexusSpawners;

    private void Start()
    {
        nexusSpawners = GetComponentsInChildren<EnemySpawner>();
    }

    private void OnEnable()
    {
        GameManager.OnPlayerProximityToCore += HandlePlayerProximity;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerProximityToCore -= HandlePlayerProximity;
    }


    public void HandlePlayerProximity(GameObject player)
    {
        foreach (var spawner in GetComponentsInChildren<EnemySpawner>())
        {
            float distance = Vector3.Distance(player.transform.position, spawner.transform.position);

            spawner.SetSpawner(distance > 5f);
        }
    }
}
