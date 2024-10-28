using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    public float lifetime = 10f;
    public Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public abstract void Launch(Vector3 direction, float projectileSpeed);
}
