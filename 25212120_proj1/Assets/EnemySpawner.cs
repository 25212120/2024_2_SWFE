using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool canSpawn = true;
    public GameObject enemyPrefab;

    private void OnEnable()
    {
        GameManager.OnEnemySpawnRequested += HandleEnemySpawnRequest;
    }

    private void OnDisable()
    {
        GameManager.OnEnemySpawnRequested -= HandleEnemySpawnRequest;
    }

    private void HandleEnemySpawnRequest(GameObject spawnerObject)
    {
        if (spawnerObject == gameObject && canSpawn)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }

    public void SetSpawner(bool enabled)
    {
        canSpawn = enabled;
    }


    public bool CanSpawn()
    {
        return canSpawn;
    }

}
