using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool canSpawn = true;
    public GameObject enemyPrefab_Mage;
    public GameObject enemyPrefab_Warrior;

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
        if (enemyPrefab_Mage != null && enemyPrefab_Warrior)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        Instantiate(enemyPrefab_Warrior, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(enemyPrefab_Mage, transform.position, Quaternion.identity);
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
