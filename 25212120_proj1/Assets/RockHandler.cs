using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RockHandler : MonoBehaviour
{
    private bool hasCollided = false;

    private GameObject explosionEffect;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        LoadRockExplosionEffect("Prefabs/Magic/Earth/RockFall/earthNova");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided)
        {
            return;
        }

        hasCollided = true;
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 explosionPoint = collisionPoint;

        GameObject instantiatedNova = Instantiate(explosionEffect, explosionPoint, Quaternion.Euler(-90, 0, 0));
        StartCoroutine(destroyNova(instantiatedNova));

        StartCoroutine(destroyRock());
    }

    private void LoadRockExplosionEffect(string prefabAddress)
    {
        explosionEffect = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator destroyRock()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private IEnumerator destroyNova(GameObject nova)
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(nova);
    }
}
