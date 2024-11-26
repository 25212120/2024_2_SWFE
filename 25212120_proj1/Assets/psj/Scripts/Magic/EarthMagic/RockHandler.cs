using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RockHandler : MonoBehaviour
{
    private bool hasCollided = false;
    public PlayerStat playerStat;

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

    private void OnTriggerEnter(Collider collision)
    {
        if (hasCollided)
        {
            return;
        }

        hasCollided = true;

        BaseMonster enemy = collision.gameObject.GetComponent<BaseMonster>();
        if (enemy != null)
        {
            playerStat.MagicAttack(enemy, 3);
        }


        Vector3 collisionPoint = collision.ClosestPoint(transform.position);

        Instantiate(explosionEffect, collisionPoint, Quaternion.Euler(-90, 0, 0));

        Destroy(gameObject);
    }

    private void LoadRockExplosionEffect(string prefabAddress)
    {
        explosionEffect = Resources.Load<GameObject>(prefabAddress);
    }

}
