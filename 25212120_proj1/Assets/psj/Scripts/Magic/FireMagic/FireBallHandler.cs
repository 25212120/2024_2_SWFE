using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBallHandler : MonoBehaviour
{
    private bool hasCollided = false;

    private GameObject explosionEffect;
    private GameObject flameThrower;
    public PlayerStat playerStat;
    Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        LoadFireballExplosionEffect("Prefabs/Magic/Fire/FireBall/ExplosionFireBallFire");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided)
        {
            return;
        }

        hasCollided = true;
        Vector3 collisionPoint = collision.contacts[0].point;
        Vector3 explosionPoint = collisionPoint + new Vector3(0, 0.3f, 0);

        GameObject instantiatedFexplosion = Instantiate(explosionEffect, explosionPoint, Quaternion.identity);
        instantiatedFexplosion.GetComponent<ExplosionFireballFire>().playerStat = playerStat;

        Destroy(gameObject);
    }

    private void LoadFireballExplosionEffect(string prefabAddress)
    {
        explosionEffect = Resources.Load<GameObject>(prefabAddress);
    }

}
