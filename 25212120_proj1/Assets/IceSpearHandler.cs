using UnityEngine;

public class IceSpearHandler : MonoBehaviour
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
        LoadIceSpearExplosionEffect("Prefabs/Magic/Water/IceSpear/NovaFrost");
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

        Instantiate(explosionEffect, explosionPoint, Quaternion.Euler(-90, 0, 0));

        Destroy(gameObject);
    }

    private void LoadIceSpearExplosionEffect(string prefabAddress)
    {
        explosionEffect = Resources.Load<GameObject>(prefabAddress);
    }
}
