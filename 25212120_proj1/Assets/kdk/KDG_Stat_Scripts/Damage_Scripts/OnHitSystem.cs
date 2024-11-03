using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitSystem : MonoBehaviour
{
    private void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null )
        {
            //damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(this.gameObject);
    }
}
