using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class PlayerProjectileManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    public GameObject chargedArrowPrefab;
    public Transform arrowSpawnPoint;
    public Transform playerTransform;

    private void Awake()
    {
        playerTransform = GetComponent<Transform>();
    }

    public void ShootArrow()
    {
        StartCoroutine(SA());
    }

    private IEnumerator SA()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        ArrowProjectile arrowProjectile = arrow.GetComponent<ArrowProjectile>();
        arrow.transform.SetParent(playerTransform, true);
        yield return new WaitForSeconds(0.1f);
        Vector3 shootDirection = transform.forward;
        arrowProjectile.Launch(shootDirection, 100f);
    }

    public void ShootLastArrow()
    {
        StartCoroutine(SLA());
    }

    private IEnumerator SLA()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        ArrowProjectile arrowProjectile = arrow.GetComponent<ArrowProjectile>();
        arrow.transform.SetParent(playerTransform, true);
        yield return new WaitForSeconds(0.1f);
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
