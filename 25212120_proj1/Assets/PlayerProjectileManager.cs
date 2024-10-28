using JetBrains.Annotations;
using UnityEngine;

public class PlayerProjectileManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    public void ShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        ArrowProjectile arrowProjectile = arrow.GetComponent<ArrowProjectile>();

        Vector3 shootDirection = transform.forward;
        arrowProjectile.Launch(shootDirection, 100f);
    }

    public void ShootLastArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        ArrowProjectile arrowProjectile = arrow.GetComponent <ArrowProjectile>();

        Vector3 shootDirection = transform.forward;
        arrowProjectile.Launch(shootDirection, 150f);
    }
}
