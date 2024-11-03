using JetBrains.Annotations;
using UnityEngine;

public class PlayerProjectileManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject chargedArrowPrefab;
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

    // 이후에 넉백 + 관통 있는 화살로 교체
    public void ChargedShot()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        ArrowProjectile arrowProjectile = arrow.GetComponent<ArrowProjectile>();

        Vector3 shootDirection = transform.forward;
        arrowProjectile.Launch(shootDirection, 150f);
    }
}
