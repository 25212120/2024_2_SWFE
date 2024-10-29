using UnityEngine;

public class ArrowProjectile : PlayerProjectileBase
{
    public Transform arrowSpawnPoint;

    protected override void Start()
    {
        base.Start();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 0.4f;
        rb.centerOfMass = new Vector3(0, 0, -1.2f);
    }

    public override void Launch(Vector3 direction, float arrowSpeed)
    {
        rb.velocity = direction.normalized * arrowSpeed;
        rb.isKinematic = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = true;

        Vector3 hitNormal = collision.contacts[0].normal;
        Quaternion rotation = Quaternion.FromToRotation(transform.forward, -hitNormal);
        transform.rotation = rotation * transform.rotation;
        transform.position = collision.contacts[0].point;
    }
}