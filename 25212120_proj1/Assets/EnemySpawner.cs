using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool canSpawn = false;

    public void ActivateSpawner()
    {
        canSpawn = true;
    }

    public void DeactivateSpawner()
    {
        canSpawn = false;
    }


}
