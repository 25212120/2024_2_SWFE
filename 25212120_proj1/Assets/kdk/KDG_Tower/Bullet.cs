using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;
    public GameObject impactEffect;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFram = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFram)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFram, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            HitTarget();
        }
        BaseStructure structure = collision.gameObject.GetComponent<BaseStructure>();
      
        BaseMonster targetMonster = collision.gameObject.GetComponent<BaseMonster>();
        if (targetMonster != null)
        {
            structure.Attack(targetMonster); 
        }
        

        Destroy(gameObject);
    }
   
    void HitTarget()
    {
        GameObject effectIns = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectIns, 2f);
    }
    
}