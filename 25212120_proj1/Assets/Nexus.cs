using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    LayerMask spawnLayer;

    private void Awake()
    {
        spawnLayer = LayerMask.GetMask("Spawn");
    }

    private void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, 80f, spawnLayer);
        foreach(Collider collider in colliders)
        {
            if (Vector3.Distance(transform.position, collider.transform.position) >= 60f)
            {
                collider.gameObject.SetActive(true);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 작은 구 (반지름 30)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 30f);

        // 큰 구 (반지름 60)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 60f);


        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, 80f);
    }
}
