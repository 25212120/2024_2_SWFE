using UnityEngine;
using Cinemachine;
using System.Collections;

public class MeteorHandler : MonoBehaviour
{
    private GameObject novaEffect0;
    private GameObject novaEffect1;
    private GameObject novaEffect2;
    private Rigidbody rb;
    private Vector3 explosionPoint;
    private CinemachineImpulseSource impulseSource;

    private bool collisionDetected = false;

    public PlayerStat playerStat;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        
    }

    private void Start()
    {
        LoadMeteorExplosionEffect0("Prefabs/Magic/Fire/Meteor/NovaFireRed");
        LoadMeteorExplosionEffect1("Prefabs/Magic/Fire/Meteor/NovaFireRed 1");
        LoadMeteorExplosionEffect2("Prefabs/Magic/Fire/Meteor/NovaFireRed 2");

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionDetected == true) return;
        collisionDetected = true;
        Vector3 collisionPoint = collision.contacts[0].point;
        explosionPoint = collisionPoint + new Vector3(0, 0.3f, 0);

        StartCoroutine(InstantiateNovaEffect());

        impulseSource.GenerateImpulse();
    }

    private void LoadMeteorExplosionEffect0(string prefabAddress)
    {
        novaEffect0 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadMeteorExplosionEffect1(string prefabAddress)
    {
        novaEffect1 = Resources.Load<GameObject>(prefabAddress);
    }

    private void LoadMeteorExplosionEffect2(string prefabAddress)
    {
        novaEffect2 = Resources.Load<GameObject>(prefabAddress);
    }

    private IEnumerator InstantiateNovaEffect()
    {
        GameObject instantiatedNovaEffect0 = Instantiate(novaEffect0, explosionPoint, Quaternion.Euler(-90, 0, 0));
        instantiatedNovaEffect0.GetComponent<MeteorNova>().playerStat = playerStat;
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedNovaEffect1 = Instantiate(novaEffect1, explosionPoint, Quaternion.Euler(-90, 0, 0));
        instantiatedNovaEffect1.GetComponent<MeteorNova>().playerStat = playerStat;
        yield return new WaitForSeconds(0.2f);
        GameObject instantiatedNovaEffect2 = Instantiate(novaEffect2, explosionPoint, Quaternion.Euler(-90, 0, 0));
        instantiatedNovaEffect2.GetComponent<MeteorNova>().playerStat = playerStat;

        Destroy(gameObject);

        yield return new WaitForSeconds(0.5f);
        Destroy(instantiatedNovaEffect0);
        Destroy(instantiatedNovaEffect1);
        Destroy(instantiatedNovaEffect2);
    }
}
